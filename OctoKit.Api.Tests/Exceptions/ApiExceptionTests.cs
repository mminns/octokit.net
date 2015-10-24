﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using NSubstitute;
using Octokit.Http;
using Octokit.Internal;
using Xunit;
using Xunit.Extensions;

namespace Octokit.Tests.Exceptions
{
    public class ApiExceptionTests
    {
        public class TheConstructor
        {
            [Fact]
            public void SetsDefaultExceptionMessage()
            {
                var exception = new ApiException();
                Assert.Equal("An error occurred with this API request", exception.Message);
            }

            [Fact]
            public void SetsSpecifiedExceptionMessageAndInnerException()
            {
                var inner = new InvalidOperationException();
                
                var exception = new ApiException("Shit broke", inner, Substitute.For<IDataFormatSerializer>());
                
                Assert.Equal("Shit broke", exception.Message);
                Assert.Same(inner, exception.InnerException);
            }

            [Fact]
            public void SetsSpecifiedExceptionMessageAndStatusCode()
            {
                var exception = new ApiException("Shit still broke", HttpStatusCode.Gone);

                Assert.Equal("Shit still broke", exception.Message);
                Assert.Equal(HttpStatusCode.Gone, exception.StatusCode);
            }

            [Fact]
            public void CreatesGitHubErrorFromJsonResponse()
            {
                var response = new Response(
                    HttpStatusCode.GatewayTimeout,
                    @"{""errors"":[{""code"":""custom"",""field"":""key"",""message"":""key is " +
                           @"already in use"",""resource"":""PublicKey""}],""message"":""Validation Failed""}",
                    new Dictionary<string, string>(),
                    "application/json"
                );
                var serializer = Substitute.For<IDataFormatSerializer>();
                serializer.Deserialize<ApiError>(response.Body.ToString())
                    .Returns(new ApiError("Validation Failed", null, new List<ApiErrorDetail>() { new ApiErrorDetail("key is already in use", null, null, null) }));

                var exception = new ApiException(response, serializer);

                Assert.Equal("Validation Failed", exception.ApiError.Message);
                Assert.Equal("key is already in use", exception.ApiError.Errors.First().Message);
                Assert.Equal(HttpStatusCode.GatewayTimeout, exception.StatusCode);
            }

            [Theory]
            [InlineData("")]
            [InlineData(null)]
            [InlineData("{{{{{")]
            [InlineData("<html><body><h1>502 Bad Gateway</h1>The server returned an invalid or incomplete response.</body></html>")]
            public void CreatesGitHubErrorIfResponseMessageIsNotValidJson(string responseContent)
            {
                var response = new Response(
                    HttpStatusCode.GatewayTimeout,
                    responseContent,
                    new Dictionary<string, string>(),
                    "application/json");

                var exception = new ApiException(response, Substitute.For<IDataFormatSerializer>());

                Assert.Equal(responseContent, exception.ApiError.Message);
                Assert.Equal(HttpStatusCode.GatewayTimeout, exception.StatusCode);
            }

            [Fact]
            public void CreatesEmptyGitHubErrorWhenResponseBodyIsNull()
            {
                var response = Substitute.For<IResponse>();
                response.Body.Returns("test");

                var exception = new ApiException();
                var anotherException = new ApiException(new Response(HttpStatusCode.ServiceUnavailable, "message1", new Dictionary<string, string>(), "application/json"), Substitute.For<IDataFormatSerializer>());
                var thirdException = new ApiException(new Response(HttpStatusCode.ServiceUnavailable, "message2", new Dictionary<string, string>(), "application/json"), Substitute.For<IDataFormatSerializer>());

                // It's fine if the message is null when there's no response body as long as this doesn't throw.
                Assert.Null(exception.ApiError.Message);
                Assert.Equal("message1", anotherException.ApiError.Message);
                Assert.Equal("message2", thirdException.ApiError.Message);
            }

#if !NETFX_CORE
            [Fact]
            public void CanPopulateObjectFromSerializedData()
            {
                IResponse response = new Response(
                    (HttpStatusCode)422,
                    @"{""errors"":[{""code"":""custom"",""field"":""key"",""message"":""key is " +
                    @"already in use"",""resource"":""PublicKey""}],""message"":""Validation Failed""}",
                    new Dictionary<string, string>(),
                    "application/json");

                var serializer = Substitute.For<IDataFormatSerializer>();
                serializer.Deserialize<ApiError>(response.Body.ToString())
                    .Returns(new ApiError("Validation Failed", null, new List<ApiErrorDetail>() { new ApiErrorDetail("key is already in use", null, null, null) }));
                var exception = new ApiException(response, serializer);

                using (var stream = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, exception);
                    stream.Position = 0;
                    var deserialized = (ApiException)formatter.Deserialize(stream);
                    Assert.Equal("Validation Failed", deserialized.ApiError.Message);
                    Assert.Equal("key is already in use", exception.ApiError.Errors.First().Message);
                }
            }
#endif
        }
    }
}