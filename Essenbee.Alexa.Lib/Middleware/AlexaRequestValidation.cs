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

        public static string SignatureHeadersMissing = "Bad Request - Signature or SignatureCertChainUrl missing";
        public static string SignatureChainUrlEmpty = "Bad Request - SignatureCertChainUrl was null";
        public static string MustUseHttps = "Bad Request - must use https";
        public static string InvalidHostName = "Bad Request - certificate url has invalid host name";
        public static string InvalidCertificatePath = "Bad Request - certificate url has invalid path";
        public static string CertificateNotYetEffective = "Bad Request - certificate is not yet effective";
        public static string CertificateExpired = "Bad Request - certificate has expired";
        public static string CertificateInvalidIssuer = "Bad Request - certificate has invalid issuer";
        public static string InvalidCertificateChain = "Bad Request - invalid certificate chain";
        public static string InvalidCertificateKey = "Bad Request - invalid certificate key";

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
                _logger.LogError(SignatureHeadersMissing);

                httpContext.Response.StatusCode = 400; // Bad Request
                await httpContext.Response.WriteAsync(SignatureHeadersMissing);

                return;
            }

            var sigCertChainUrl = httpContext.Request.Headers["SignatureCertChainUrl"];
            var certChainUrl = sigCertChainUrl.Any()
                ? sigCertChainUrl.First()
                : string.Empty;

            if (string.IsNullOrWhiteSpace(certChainUrl))
            {
                _logger.LogError(SignatureChainUrlEmpty);

                httpContext.Response.StatusCode = 400; // Bad Request
                await httpContext.Response.WriteAsync(SignatureChainUrlEmpty);

                return;
            }

            // https://s3.amazonaws.com/echo.api/echo-api-cert-6-ats.pem
            var certUri = new Uri(certChainUrl);

            if (!((certUri.Port == 443 || certUri.IsDefaultPort) &&
                certUri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogError(MustUseHttps);

                httpContext.Response.StatusCode = 400; // Bad Request
                await httpContext.Response.WriteAsync(MustUseHttps);

                return;
            }

            if (!certUri.Host.Equals(ValidHostName, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogError(InvalidHostName);

                httpContext.Response.StatusCode = 400; // Bad Request
                await httpContext.Response.WriteAsync(InvalidHostName);

                return;
            }

            if (!certUri.AbsolutePath.StartsWith("/echo.api/"))
            {
                _logger.LogError(InvalidCertificatePath);

                httpContext.Response.StatusCode = 400; // Bad Request
                await httpContext.Response.WriteAsync(InvalidCertificatePath);

                return;
            }

            using (var client = new WebClient())
            {
                var certData = client.DownloadData(certUri);
                var certificate = new X509Certificate2(certData);

                if (certificate.NotBefore > DateTime.UtcNow)
                {
                    _logger.LogError(CertificateNotYetEffective);

                    httpContext.Response.StatusCode = 400; // Bad Request
                    await httpContext.Response.WriteAsync(CertificateNotYetEffective);

                    return;
                }

                if (certificate.NotAfter <= DateTime.UtcNow)
                {
                    _logger.LogError(CertificateExpired);

                    httpContext.Response.StatusCode = 400; // Bad Request
                    await httpContext.Response.WriteAsync(CertificateExpired);

                    return;
                }

                if (!certificate.Subject.Contains("CN=echo-api.amazon.com"))
                {
                    _logger.LogError(CertificateInvalidIssuer);

                    httpContext.Response.StatusCode = 400; // Bad Request
                    await httpContext.Response.WriteAsync(CertificateInvalidIssuer);

                    return;
                }

                // ToDo: All certificates in the chain combine to create a 
                // chain of trust to a trusted root CA certificate
                var certificateChain = new X509Chain
                {
                    ChainPolicy =
                    {
                        RevocationMode = X509RevocationMode.NoCheck
                    }
                };

                var hasChainToTrustedCA = certificateChain.Build(certificate);

                if (!hasChainToTrustedCA)
                {
                    _logger.LogError(InvalidCertificateChain);

                    httpContext.Response.StatusCode = 400; // Bad Request
                    await httpContext.Response.WriteAsync(InvalidCertificateChain);

                    return;
                }

                try
                {
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
                            _logger.LogError(InvalidCertificateKey);

                            httpContext.Response.StatusCode = 400; // Bad Request
                            await httpContext.Response.WriteAsync(InvalidCertificateKey);

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
