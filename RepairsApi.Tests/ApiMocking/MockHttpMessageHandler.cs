using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace RepairsApi.Tests.ApiMocking
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly MockMessageHandlerConfig _config;

        public MockHttpMessageHandler(MockMessageHandlerConfig config)
        {
            _config = config;
        }

        public static MockHttpMessageHandler FromClass<T>()
            where T : class
        {
            return BuildMock<T>(null, true);
        }

        public static MockHttpMessageHandler FromObject<T>(T oobject)
            where T : class
        {
            return BuildMock(oobject, false);
        }

        private static MockHttpMessageHandler BuildMock<T>(T oobject, bool onlyStatic)
            where T : class
        {
            Type classType = typeof(T);

            var config = new MockMessageHandlerConfig
            {
                Functions = new List<MockRouteHandler>()
            };

            config.Functions.AddRange(classType.GetMethods().Where(m => (!onlyStatic || m.IsStatic) && m.CustomAttributes.Any(attr => attr.AttributeType == typeof(RouteAttribute)))
                .Select(method => new MockRouteHandler(method, oobject)));

            return new MockHttpMessageHandler(config);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method != HttpMethod.Get)
            {
                throw new Exception("Mocking Is only supported for gets at the moment");
            }

            var mockFunction = FindMockFunction(request.RequestUri);

            if (mockFunction != null)
            {
                object result = mockFunction.Execute();

                if (result == null)
                {
                   return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
                }

                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(result));
                return Task.FromResult(httpResponseMessage);
            }

            throw new Exception("Mock Method not found");
        }

        private MockRouteHandler FindMockFunction(Uri requestUri)
        {
            var segments = requestUri.Segments;
            foreach (var potentialMock in _config.Functions)
            {
                var segmentMatches = potentialMock.ParseSegments(segments);

                if (segmentMatches)
                {
                    var query = HttpUtility.ParseQueryString(requestUri.Query);
                    potentialMock.ParseQuery(query);
                    return potentialMock;
                }
            }

            return null;
        }
    }

    public class MockMessageHandlerConfig
    {
        public List<MockRouteHandler> Functions { get; internal set; }
    }

    public class MockRouteHandler
    {
        private static Regex _pathMatcher = new Regex(@"^{(.+)}\/?$");
        private string[] _segments;
        private readonly object _methodObject;
        private NameValueCollection _variables;
        private MethodInfo _method;

        public MockRouteHandler(MethodInfo method, object methodObject = null)
        {
            if (methodObject is null && !method.IsStatic)
            {
                throw new Exception($"{method.Name} Either needs to be static or an instance of the object it is in needs to be passed");
            }

            _methodObject = methodObject;
            _variables = new NameValueCollection();
            _method = method;
            var route = _method.GetCustomAttribute<RouteAttribute>();

            var routeUri = new Uri(route.Template, UriKind.Absolute);
            this._segments = routeUri.Segments.Select(seg => HttpUtility.UrlDecode(seg)).ToArray();
        }

        internal object Execute()
        {
            object[] parameters = _method.GetParameters().Select(param => _variables[param.Name]).ToArray();

            return _method.Invoke(_methodObject, parameters);
        }

        internal void ParseQuery(NameValueCollection queryParams)
        {
            this._variables.Add(queryParams);
        }

        internal bool ParseSegments(string[] segments)
        {
            if (segments.Length != _segments.Length) return false;

            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i] == _segments[i])
                {
                    continue;
                }
                else if (_pathMatcher.IsMatch(_segments[i]))
                {
                    _variables.Add(_pathMatcher.Match(_segments[i]).Groups[1].Value, segments[i].Trim('/'));
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}
