using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace MvcWebRole.Controllers
{
    public class RelayController : ApiController
    {
        // GET api/values
        public string Get(string path)
        {
            return "get: " + path;
        }
   }
}