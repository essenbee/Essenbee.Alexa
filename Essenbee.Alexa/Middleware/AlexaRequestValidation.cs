using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Essenbee.Alexa.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AlexaRequestValidation
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AlexaRequestValidation> _logger;

        public AlexaRequestValidation(RequestDelegate next, ILogger<AlexaRequestValidation> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
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
    }
}
