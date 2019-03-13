using Essenbee.Alexa.Lib.Middleware;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xunit;

namespace Essenbee.Alexa.Lib.Tests
{
    public class AlexaRequestValidationShould
    {
        [Fact]
        public async void NotReturnBadRequestIfHeadersPresent()
        {
            // Arrange
            var fakeRequest = new Fake<HttpRequest>();
            var fakeLogger = new Fake<ILogger<AlexaRequestValidation>>();
            var fakeContext = new Fake<HttpContext>();

            var headers = new HeaderDictionary
            {
                { "Signature", "YLGXuQcL6KjIk/H2G86zuDyvvkIX/iFQGOm4WmwHB8mzFn10CQ/DPOmkpRfX3RwrFqVrO7j//RMics5D903hERwiOIux+OpnPA/T3EITQPZrTQTYoCEodDa5cjSCFkoRyqVfCT8jGxER3L6JvOVFMzJ/kpslsy1Vd9keE4CEpC21lUCOQIHgKBtAd/coWjN8WM8Yz8W1ACbqIzZ4vA1V4Kvnz+bmLDH6qsMDYcotcJek8Bsb3PQ4YxtqGNK04R/pGWIcoKNFHMqe+rYqPPz8/7kkuX8aCe0xcxsG0eGcLpvK4zlTO4tSavOsCsKONEJj0mvlPMo7ZK4N074WVWurAQ=="},
                { "SignatureCertChainUrl", "https://s3.amazonaws.com/echo.api/echo-api-cert-6-ats.pem"}

            };

            A.CallTo(() => fakeRequest.FakedObject.Scheme).Returns("https");
            A.CallTo(() => fakeRequest.FakedObject.Headers).Returns(headers);

            A.CallTo(() => fakeContext.FakedObject.Request).Returns(fakeRequest.FakedObject);

            var sit = new AlexaRequestValidation(next: (innerHttpContext) => Task.FromResult(0),
                logger: fakeLogger.FakedObject);

            // Action
            await sit.Invoke(fakeContext.FakedObject);

            // Assert
            Assert.False(fakeContext.FakedObject.Response.StatusCode == 400); // Bad Request
        }

        [Fact]
        public async void ReturnBadRequestIfHeadersNotPresent()
        {
            // Arrange
            var fakeRequest = new Fake<HttpRequest>();
            var fakeLogger = new Fake<ILogger<AlexaRequestValidation>>();
            var fakeContext = new Fake<HttpContext>();

            var headers = new HeaderDictionary
            {
                { "Signature", "Sig"},
            };

            A.CallTo(() => fakeRequest.FakedObject.Scheme).Returns("https");
            A.CallTo(() => fakeRequest.FakedObject.Headers).Returns(headers);

            A.CallTo(() => fakeContext.FakedObject.Request).Returns(fakeRequest.FakedObject);

            var sit = new AlexaRequestValidation(next: (innerHttpContext) => Task.FromResult(0),
                logger: fakeLogger.FakedObject);

            // Action
            await sit.Invoke(fakeContext.FakedObject);

            // Assert
            Assert.True(fakeContext.FakedObject.Response.StatusCode == 400); // Bad Request
        }

        [Fact]
        public async void ReturnBadRequestIfNotCertUrlHttps()
        {
            // Arrange
            var fakeRequest = new Fake<HttpRequest>();
            var fakeLogger = new Fake<ILogger<AlexaRequestValidation>>();
            var fakeContext = new Fake<HttpContext>();

            var headers = new HeaderDictionary
            {
                { "Signature", "YLGXuQcL6KjIk/H2G86zuDyvvkIX/iFQGOm4WmwHB8mzFn10CQ/DPOmkpRfX3RwrFqVrO7j//RMics5D903hERwiOIux+OpnPA/T3EITQPZrTQTYoCEodDa5cjSCFkoRyqVfCT8jGxER3L6JvOVFMzJ/kpslsy1Vd9keE4CEpC21lUCOQIHgKBtAd/coWjN8WM8Yz8W1ACbqIzZ4vA1V4Kvnz+bmLDH6qsMDYcotcJek8Bsb3PQ4YxtqGNK04R/pGWIcoKNFHMqe+rYqPPz8/7kkuX8aCe0xcxsG0eGcLpvK4zlTO4tSavOsCsKONEJj0mvlPMo7ZK4N074WVWurAQ=="},
                { "SignatureCertChainUrl", "http://s3.amazonaws.com/echo.api/echo-api-cert-6-ats.pem"}

            };

            A.CallTo(() => fakeRequest.FakedObject.Scheme).Returns("https");
            A.CallTo(() => fakeRequest.FakedObject.Headers).Returns(headers);

            A.CallTo(() => fakeContext.FakedObject.Request).Returns(fakeRequest.FakedObject);

            var sit = new AlexaRequestValidation(next: (innerHttpContext) => Task.FromResult(0),
                logger: fakeLogger.FakedObject);

            // Action
            await sit.Invoke(fakeContext.FakedObject);

            // Assert
            Assert.True(fakeContext.FakedObject.Response.StatusCode == 400); // Bad Request
        }

        [Fact]
        public async void ReturnBadRequestIfInvalidCertHostname()
        {
            // Arrange
            var fakeRequest = new Fake<HttpRequest>();
            var fakeLogger = new Fake<ILogger<AlexaRequestValidation>>();
            var fakeContext = new Fake<HttpContext>();

            var headers = new HeaderDictionary
            {
                { "Signature", "YLGXuQcL6KjIk/H2G86zuDyvvkIX/iFQGOm4WmwHB8mzFn10CQ/DPOmkpRfX3RwrFqVrO7j//RMics5D903hERwiOIux+OpnPA/T3EITQPZrTQTYoCEodDa5cjSCFkoRyqVfCT8jGxER3L6JvOVFMzJ/kpslsy1Vd9keE4CEpC21lUCOQIHgKBtAd/coWjN8WM8Yz8W1ACbqIzZ4vA1V4Kvnz+bmLDH6qsMDYcotcJek8Bsb3PQ4YxtqGNK04R/pGWIcoKNFHMqe+rYqPPz8/7kkuX8aCe0xcxsG0eGcLpvK4zlTO4tSavOsCsKONEJj0mvlPMo7ZK4N074WVWurAQ=="},
                { "SignatureCertChainUrl", "http://s3.amazonaws.dev/echo.api/echo-api-cert-6-ats.pem"}

            };

            A.CallTo(() => fakeRequest.FakedObject.Scheme).Returns("https");
            A.CallTo(() => fakeRequest.FakedObject.Headers).Returns(headers);

            A.CallTo(() => fakeContext.FakedObject.Request).Returns(fakeRequest.FakedObject);

            var sit = new AlexaRequestValidation(next: (innerHttpContext) => Task.FromResult(0),
                logger: fakeLogger.FakedObject);

            // Action
            await sit.Invoke(fakeContext.FakedObject);

            // Assert
            Assert.True(fakeContext.FakedObject.Response.StatusCode == 400); // Bad Request
        }

        [Fact]
        public async void ReturnBadRequestIfInvalidCertPath()
        {
            // Arrange
            var fakeRequest = new Fake<HttpRequest>();
            var fakeLogger = new Fake<ILogger<AlexaRequestValidation>>();
            var fakeContext = new Fake<HttpContext>();

            var headers = new HeaderDictionary
            {
                { "Signature", "YLGXuQcL6KjIk/H2G86zuDyvvkIX/iFQGOm4WmwHB8mzFn10CQ/DPOmkpRfX3RwrFqVrO7j//RMics5D903hERwiOIux+OpnPA/T3EITQPZrTQTYoCEodDa5cjSCFkoRyqVfCT8jGxER3L6JvOVFMzJ/kpslsy1Vd9keE4CEpC21lUCOQIHgKBtAd/coWjN8WM8Yz8W1ACbqIzZ4vA1V4Kvnz+bmLDH6qsMDYcotcJek8Bsb3PQ4YxtqGNK04R/pGWIcoKNFHMqe+rYqPPz8/7kkuX8aCe0xcxsG0eGcLpvK4zlTO4tSavOsCsKONEJj0mvlPMo7ZK4N074WVWurAQ=="},
                { "SignatureCertChainUrl", "http://s3.amazonaws.com/alexa.api/echo-api-cert-6-ats.pem"}

            };

            A.CallTo(() => fakeRequest.FakedObject.Scheme).Returns("https");
            A.CallTo(() => fakeRequest.FakedObject.Headers).Returns(headers);

            A.CallTo(() => fakeContext.FakedObject.Request).Returns(fakeRequest.FakedObject);

            var sit = new AlexaRequestValidation(next: (innerHttpContext) => Task.FromResult(0),
                logger: fakeLogger.FakedObject);

            // Action
            await sit.Invoke(fakeContext.FakedObject);

            // Assert
            Assert.True(fakeContext.FakedObject.Response.StatusCode == 400); // Bad Request
        }
    }
}
