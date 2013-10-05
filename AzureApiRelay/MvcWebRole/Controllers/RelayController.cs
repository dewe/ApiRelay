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

        public async Task<HttpResponseMessage> GetRelay(string path)
        {
            var uri = new Uri(RelayServiceUri, (path == "<root>" ? "" : path));
            using (var serviceResponse = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead))
            {
                return CreateResponse(serviceResponse);
            }
        }

        //public async Task<HttpResponseMessage> PutRelay()
        //{
        //    // Get content (which hasn't been read yet) from incoming request.
        //    HttpContent content = Request.Content;
        //    Request.Content = null;

        //    // Submit request to our service which we relay. 
        //    using (HttpResponseMessage serviceResponse = await _client.PutAsync(RelayToUri, content))
        //    {
        //        // Return response
        //        return CreateResponse(serviceResponse);
        //    }
        //}

        private HttpResponseMessage CreateResponse(HttpResponseMessage serviceResponse)
        {
            HttpResponseMessage relayResponse = Request.CreateResponse(serviceResponse.StatusCode);
            relayResponse.Content = serviceResponse.Content;
            serviceResponse.Content = null;
            return relayResponse;
        }
    }
}