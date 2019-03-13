using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;

namespace Essenbee.Alexa.Lib.Middleware
{
    public class AlexaRequestValidation
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AlexaRequestValidation> _logger;
        private const string ValidHostName = "s3.amazonaws.com";

        public AlexaRequestValidation(RequestDelegate next, ILogger<AlexaRequestValidation> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            BufferingHelper.EnableRewind(httpContext.Request);

            if (!httpContext.Request.Headers.Keys.Contains("Signature") ||
                !httpContext.Request.Headers.Keys.Contains("SignatureCertChainUrl"))
            {
                _logger.LogError("Bad Request - Signature or SignatureCertChainUrl missing");

                httpContext.Response.StatusCode = 400; // Bad Request
                await httpContext.Response.WriteAsync("Signature or SignatureCertChainUrl missing");

                return;
            }

            var sigCertChainUrl = httpContext.Request.Headers["SignatureCertChainUrl"];
            var certChainUrl = sigCertChainUrl.Any()
                ? sigCertChainUrl.First()
                : string.Empty;

            if (string.IsNullOrWhiteSpace(certChainUrl))
            {
                _logger.LogError("Bad Request - SignatureCertChainUrl was null");

                httpContext.Response.StatusCode = 400; // Bad Request
                await httpContext.Response.WriteAsync("SignatureCertChainUrl was null");

                return;
            }

            // https://s3.amazonaws.com/echo.api/echo-api-cert-6-ats.pem
            var certUri = new Uri(certChainUrl);

            if (!((certUri.Port == 443 || certUri.IsDefaultPort) &&
                certUri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogError("Bad Request - must use https");

                httpContext.Response.StatusCode = 400; // Bad Request
                await httpContext.Response.WriteAsync("Must use https");

                return;
            }

            if (!certUri.Host.Equals(ValidHostName, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogError("Bad Request - certificate url has invalid host name");

                httpContext.Response.StatusCode = 400; // Bad Request
                await httpContext.Response.WriteAsync("Certificate url has invalid host name");

                return;
            }

            if (!certUri.AbsolutePath.StartsWith("/echo.api/"))
            {
                _logger.LogError("Bad Request - certificate url has invalid path");

                httpContext.Response.StatusCode = 400; // Bad Request
                await httpContext.Response.WriteAsync("Certificate url has invalid path");

                return;
            }

            using (var client = new WebClient())
            {
                var certData = client.DownloadData(certUri);
                var certificate = new X509Certificate2(certData);

                var foundEffectiveDate = DateTime.TryParse(certificate.GetEffectiveDateString(), out var certEffectiveDate);
                var foundExpiryDate = DateTime.TryParse(certificate.GetExpirationDateString(), out var certExpiryDate);

                if (!foundEffectiveDate || certEffectiveDate > DateTime.UtcNow)
                {
                    _logger.LogError("Bad Request - certificate is not yet effective");

                    httpContext.Response.StatusCode = 400; // Bad Request
                    await httpContext.Response.WriteAsync("Certificate is not yet effective");

                    return;
                }

                if (!foundExpiryDate || certExpiryDate <= DateTime.UtcNow)
                {
                    _logger.LogError("Bad Request - certificate has expired");

                    httpContext.Response.StatusCode = 400; // Bad Request
                    await httpContext.Response.WriteAsync("Certificate has expired");

                    return;
                }

                if (!certificate.Subject.Contains("CN=echo-api.amazon.com"))
                {
                    _logger.LogError("Bad Request - certificate has invalid issuer");

                    httpContext.Response.StatusCode = 400; // Bad Request
                    await httpContext.Response.WriteAsync("Certificate has invalid issuer");

                    return;
                }

                // ToDo: All certificates in the chain combine to create a 
                // chain of trust to a trusted root CA certificate

                try
                {
                    var publicKeyProvider = certificate.GetRSAPublicKey();

                    var signature = httpContext.Request.Headers["Signature"];
                    var signatureString = signature.Any()
                        ? signature.First()
                        : string.Empty;

                    _logger.LogInformation($"Sig = >>{signatureString}<<");

                    byte[] signatureDecoded = Convert.FromBase64String(signatureString);

                    var hasher = SHA1.Create();
                    var body = await httpContext.Request.ReadHttpRequestBodyAsync();
                    httpContext.Request.Body.Position = 0; // Reset

                    var hashedBody = hasher.ComputeHash(Encoding.UTF8.GetBytes(body));

                    using (var rsaPublicKey = certificate.GetRSAPublicKey())
                    {
                        var isHashValid = rsaPublicKey.VerifyHash(hashedBody, signatureDecoded, HashAlgorithmName.SHA1, 
                            RSASignaturePadding.Pkcs1);

                        _logger.LogInformation($"Is Hash Valid Response: {isHashValid}");

                        if (rsaPublicKey == null || !isHashValid)
                        {
                            _logger.LogError("Bad Request - invalid certificate key");

                            httpContext.Response.StatusCode = 400; // Bad Request
                            await httpContext.Response.WriteAsync("Certificate key was invalid");

                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    httpContext.Response.StatusCode = 400; // Bad Request
                    await httpContext.Response.WriteAsync(ex.Message);

                    return;
                }
            }

            _logger.LogInformation("******************* VALIDATED *******************");

            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AlexaRequestValidationExtensions
    {
        public static IApplicationBuilder UseAlexaRequestValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AlexaRequestValidation>();
        }

        public static async Task<string> ReadHttpRequestBodyAsync(this HttpRequest req)
        {

            var body = string.Empty;
            var reader = new StreamReader(req.Body);
            body = await reader.ReadToEndAsync();

            return body;
        }
    }
}
