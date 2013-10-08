using FakeItEasy;
using MvcWebRole.Controllers;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;

namespace MvcWebRole.Tests.Controllers
{
    [TestFixture]
    public class RelayControllerTest
    {
        private IHttpClient _client;
        private RelayController _controller;

        [SetUp]
        public void Setup()
        {
            _client = A.Fake<IHttpClient>();
            _controller = new RelayController(_client);

            SetupControllerForTest(_controller, new HttpRequestMessage());
        }

        [Test]
        public void Should_relay_get_request()
        {
            var response = _controller.RelayGet("/").Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public void Should_forward_accept_header()
        {
            var contentType = new MediaTypeWithQualityHeaderValue("text/x-test");
            _controller.Request.Headers.Accept.Add(contentType);

            var response = _controller.RelayGet("/").Result;

            A.CallTo(() =>
                _client.SendAsync(
                    A<HttpRequestMessage>.That.Matches(r => r.Headers.Accept.Contains(contentType)),
                    A<HttpCompletionOption>.Ignored))
                .MustHaveHappened();
        }

        [TestCase("Accept", "text/html")]
        [TestCase("Accept-Charset", "iso-8859-1")]
        [TestCase("Accept-Encoding", "*")]
        [TestCase("Accept-Language", "en-us")]
        [TestCase("Authorization", "credentials")]
        [TestCase("Expect", "100-continue")]
        [TestCase("From", "webmaster@example.com")]
        [TestCase("If-Match", "\"qwerty\"")]
        [TestCase("If-Modified-Since", "Sat, 29 Oct 1994 19:43:31 GMT")]
        [TestCase("If-None-Match", "\"qwerty\"")]
        [TestCase("If-Range", "\"qwerty\"")]
        [TestCase("If-Unmodified-Since", "Sat, 29 Oct 1994 19:43:31 GMT")]
        [TestCase("Range", "bytes=4711-")]
        [TestCase("Referer", "http://www.example.com")]
        [TestCase("TE", "deflate")]
        [TestCase("User-Agent", "dummy")]
        public void Should_forward_rfc2616_request_headers(string name, string value)
        {
            _controller.Request.Headers.Add(name, value);

            var response = _controller.RelayGet("/").Result;

            ShouldHaveForwardedHeader(name);
        }

        [Test]
        public void Should_not_forward_host_header()
        {
            _controller.Request.Headers.Host = "www.example.com";

            var response = _controller.RelayGet("/").Result;

            ShouldNotHaveForwardedHeader("Host");
        }

        [Test]
        public void Should_relay_basic_authentication()
        {
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", "dummyvalue");
            SetupControllerForTest(_controller, request);
            SetupFakeResponses(_client);

            var response = _controller.RelayGet("/").Result;

            A.CallTo(() =>
                _client.SendAsync(
                    A<HttpRequestMessage>.That.Matches(r => r.Headers.Contains("Authorization")),
                    A<HttpCompletionOption>.Ignored))
                .MustHaveHappened();

            Assert.IsTrue(response.Headers.WwwAuthenticate.Count > 0);
        }

        private void SetupFakeResponses(IHttpClient client)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Basic", "realm=\"Dummy\""));

            var taskSource = new TaskCompletionSource<HttpResponseMessage>();
            taskSource.SetResult(response);
            var completedTask = taskSource.Task;

            A.CallTo(() =>
                client.SendAsync(A<HttpRequestMessage>.Ignored, A<HttpCompletionOption>.Ignored))
                .Returns(completedTask);
        }

        private void SetupControllerForTest(RelayController controller, HttpRequestMessage request)
        {
            var config = new HttpConfiguration();
            var route = config.Routes.MapHttpRoute("DefaultApi", "test/{controller}");
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", "apirelay" } });

            controller.ControllerContext = new HttpControllerContext(config, routeData, request);
            controller.Request = request;
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
        }

        private void ShouldHaveForwardedHeader(string name)
        {
            A.CallTo<Task<HttpResponseMessage>>(() =>
                _client.SendAsync(A<HttpRequestMessage>.That.Matches(request => HasHeader(name, request)), A<HttpCompletionOption>.Ignored))
                .MustHaveHappened();
        }

        private void ShouldNotHaveForwardedHeader(string name)
        {
            A.CallTo<Task<HttpResponseMessage>>(() => 
                _client.SendAsync(A<HttpRequestMessage>.That.Matches(request => HasHeader(name, request)), A<HttpCompletionOption>.Ignored))
                .MustNotHaveHappened();
        }

        private bool HasHeader(string name, HttpRequestMessage request)
        {
            return request.Headers.Contains(name);
        }
    }
}
