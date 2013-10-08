using System.Net.Http.Headers;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcWebRole.Controllers;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;

namespace MvcWebRole.Tests.Controllers
{
    [TestClass]
    public class RelayControllerTest
    {
        private IHttpClient _client;
        private RelayController _controller;

        [TestInitialize]
        public void Setup()
        {
            _client = A.Fake<IHttpClient>();
            _controller = new RelayController(_client);
        }

        [TestMethod]
        public void Should_relay_get_request()
        {
            SetupControllerForTest(_controller, new HttpRequestMessage(HttpMethod.Get, "http://localhost/"));
            
            var response = _controller.RelayGet("/").Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public void Should_forward_accept_header()
        {
            SetupControllerForTest(_controller, new HttpRequestMessage());

            var contentType = new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/x-test");
            _controller.Request.Headers.Accept.Add(contentType);

            var response = _controller.RelayGet("/").Result;

            A.CallTo(() => 
                _client.SendAsync(
                    A<HttpRequestMessage>.That.Matches(r => r.Headers.Accept.Contains(contentType)),
                    A<HttpCompletionOption>.Ignored))
                .MustHaveHappened();
        }

        [TestMethod]
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
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary {{"controller", "apirelay"}});

            controller.ControllerContext = new HttpControllerContext(config, routeData, request);
            controller.Request = request;
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
        }
    }
}
