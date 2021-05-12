using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Moq;
using NUnit.Framework;
using RepairsApi.V2.Exceptions;
using RepairsApi.V2.MiddleWare;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace RepairsApi.Tests.V2.Middleware
{
    public class ExceptionMiddlewareTest
    {
        [TestCaseSource(nameof(_cases))]
        public async Task TranslatesExceptionsToCodes(Exception ex, int code, string message)
        {
            var classUnderTest = new ExceptionMiddleware(httpContext => throw ex);
            var responseStream = new MemoryStream();
            var mockResponse = new MockResponse(responseStream);
            var mockContext = new MockHttpContext(mockResponse);

            await classUnderTest.Invoke(mockContext);

            mockResponse.StatusCode.Should().Be(code);
            var responseString = await ReadAll(new MemoryStream(responseStream.GetBuffer()));
            responseString.Should().Contain(message);
        }

        [Test]
        public async Task CallsNextInChain()
        {
            bool called = false;
            var classUnderTest = new ExceptionMiddleware(httpContext =>
            {
                called = true;
                return Task.CompletedTask;
            });
            var responseStream = new MemoryStream();
            var mockResponse = new MockResponse(responseStream);
            var mockContext = new MockHttpContext(mockResponse);

            await classUnderTest.Invoke(mockContext);

            called.Should().BeTrue();
        }

        public static Task<string> ReadAll(Stream stream)
        {
            stream.Position = 0;

            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEndAsync();
            }
        }

        static object[] _cases =
        {
            new object[] { new ResourceNotFoundException("not found message"), 404, "not found message" },
            new object[] { new ApiException(400, "api message"), 502, "api message. Upstream Sent 400" },
            new object[] { new NotSupportedException("not supported message"), 400, "not supported message" },
            new object[] { new UnauthorizedAccessException("un authorised message"), 401, "un authorised message" },
        };
    }

    internal class MockResponse : HttpResponse
    {
        public MockResponse(Stream stream)
        {
            Body = stream;
        }

        public override Stream Body { get; set; }
        public override long? ContentLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string ContentType { get; set; }

        public override IResponseCookies Cookies => throw new NotImplementedException();

        public override bool HasStarted => throw new NotImplementedException();

        public override IHeaderDictionary Headers => throw new NotImplementedException();

        public override HttpContext HttpContext => throw new NotImplementedException();

        public override int StatusCode { get; set; }

        public override void OnCompleted(Func<object, Task> callback, object state)
        {
            throw new NotImplementedException();
        }

        public override void OnStarting(Func<object, Task> callback, object state)
        {
            throw new NotImplementedException();
        }

        public override void Redirect(string location, bool permanent)
        {
            throw new NotImplementedException();
        }
    }

    internal class MockHttpContext : HttpContext
    {
        private readonly HttpResponse _response;

        public MockHttpContext(HttpResponse response)
        {
            _response = response;
        }

        public override ConnectionInfo Connection => throw new NotImplementedException();

        public override IFeatureCollection Features => throw new NotImplementedException();

        public override IDictionary<object, object> Items { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override HttpRequest Request => throw new NotImplementedException();

        public override CancellationToken RequestAborted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override IServiceProvider RequestServices { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override HttpResponse Response => _response;

        public override ISession Session { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override string TraceIdentifier { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override ClaimsPrincipal User { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override WebSocketManager WebSockets => throw new NotImplementedException();

        public override void Abort()
        {
            throw new NotImplementedException();
        }
    }
}
