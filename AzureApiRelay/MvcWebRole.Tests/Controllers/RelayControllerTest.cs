using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcWebRole;
using MvcWebRole.Controllers;

namespace MvcWebRole.Tests.Controllers
{
    [TestClass]
    public class RelayControllerTest
    {
        [TestMethod]
        public void Get()
        {
            var controller = new RelayController();
            var path = "dummy_path";
            var expected = "get: " + path;

            var responseMsg = controller.GetRelay(path).Result;

            Assert.AreEqual(expected, responseMsg.Content);
        }
    }
}
