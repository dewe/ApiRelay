using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MvcWebRole.Controllers
{
    public class RelayController : ApiController
    {
        private static readonly Uri RelayServiceUri = new Uri("http://www.sunet.se");
        private readonly IHttpClient _client;

        public RelayController()
        {
            var httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(20) };
            _client = new HttpClientWrapper(httpClient);
        }

        public RelayController(IHttpClient httpClient)
        {
            _client = httpClient;
        }

        [HttpGet]
        public async Task<HttpResponseMessage> RelayGet(string path)
        {
            var uri = new Uri(RelayServiceUri, (path == "<root>" ? "" : path));
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            using (var serviceResponse = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                return CreateResponse(serviceResponse);
            }
        }

        private HttpResponseMessage CreateResponse(HttpResponseMessage serviceResponse)
        {
            var relayResponse = Request.CreateResponse(serviceResponse.StatusCode);
            relayResponse.Content = serviceResponse.Content;
            serviceResponse.Content = null;
            return relayResponse;
        }
    }
}