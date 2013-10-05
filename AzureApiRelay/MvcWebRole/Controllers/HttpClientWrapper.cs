using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MvcWebRole.Controllers
{
    public class HttpClientWrapper : IHttpClient
    {
        private readonly HttpClient _client;

        public HttpClientWrapper(HttpClient client)
        {
            _client = client;
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption)
        {
            return _client.GetAsync(requestUri, completionOption);
        }
    }
}