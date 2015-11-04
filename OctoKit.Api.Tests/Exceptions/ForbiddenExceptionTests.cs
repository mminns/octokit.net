using System.Collections.Generic;
using System.Net;
using NSubstitute;
using Octokit.Internal;
using Xunit;

namespace Octokit.Tests.Exceptions
{
    public class ForbiddenExceptionTests
    {
        public class TheConstructor
        {
            [Fact]
            public void IdentifiesMaxLoginAttepmtsExceededReason()
            {
                const string responseBody = "{\"message\":\"YOU SHALL NOT PASS!\"," +
                                            "\"documentation_url\":\"http://developer.github.com/v3\"}";
                var response = new Response(
                    HttpStatusCode.Forbidden,
                    responseBody,
                    new Dictionary<string, string>(),
                    "application/json");

                var serializer = Substitute.For<IDataFormatSerializer>();
                serializer.Deserialize<ApiError>(response.Body.ToString())
                    .Returns(new ApiError("YOU SHALL NOT PASS!", "http://developer.github.com/v3", new List<ApiErrorDetail>()));

                var forbiddenException = new ForbiddenException(response, serializer);

                Assert.Equal("YOU SHALL NOT PASS!", forbiddenException.ApiError.Message);
            }

            [Fact]
            public void HasDefaultMessage()
            {
                var response = new Response(HttpStatusCode.Forbidden , null, new Dictionary<string, string>(), "application/json");
                var forbiddenException = new ForbiddenException(response, Substitute.For<IDataFormatSerializer>());

                Assert.Equal("Request Forbidden", forbiddenException.Message);
            }
        }
    }
}
