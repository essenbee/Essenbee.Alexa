using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Essenbee.Alexa.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AlexaRequestValidation
    {
        private readonly RequestDelegate _next;

        public AlexaRequestValidation(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (!httpContext.Request.Headers.Keys.Contains("Signature") ||
                !httpContext.Request.Headers.Keys.Contains("SignatureCertChainUrl"))
            {
                httpContext.Response.StatusCode = 400; // Bad Request
                await httpContext.Response.WriteAsync("Signature or SignatureCertChainUrl missing");

                return;
            }

            var foundCertChainUrl = httpContext.Response.Headers.TryGetValue("SignatureCertChainUrl",
                out var sigCertChainUrl);

            var certChainUrl = foundCertChainUrl ? sigCertChainUrl.First()
                : string.Empty;

            System.Diagnostics.Trace.WriteLine($"Certificate Chain Url = >{certChainUrl}<");

            if (string.IsNullOrWhiteSpace(certChainUrl))
            {
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
