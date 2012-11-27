using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Browshot;

using System.Collections;

namespace BrowshotClientTests
{
    [TestClass]
    public class Base
    {
        private BrowshotClient browshot;

        [TestInitialize]
        public void SetUp()
        {
            browshot = new BrowshotClient("API_KEY");
        }

        [TestMethod]
        public void MakeUrlNoArgument()
        {
            Uri url = browshot.MakeUrl("screenshot/create", new Hashtable());

            Assert.AreEqual("https://api.browshot.com/api/v1/screenshot/create?&key=API_KEY", url.ToString());
        }

        [TestMethod]
        public void MakeUrlNullArgument()
        {
            Uri url = browshot.MakeUrl("screenshot/create", null);

            Assert.AreEqual("https://api.browshot.com/api/v1/screenshot/create?&key=API_KEY", url.ToString());
        }

        [TestMethod]
        public void MakeUrlArgumentEscapeValue()
        {
            Hashtable arguments = new Hashtable();
            arguments.Add("foo", "+=&%");

            Uri url = browshot.MakeUrl("screenshot/create", arguments);

            Assert.AreEqual("https://api.browshot.com/api/v1/screenshot/create?&key=API_KEY&foo=%2b%3d%26%25", url.ToString());
        }

        [TestMethod]
        public void MakeUrlArgumentEscapeKey()
        {
            Hashtable arguments = new Hashtable();
            arguments.Add("+=&%", "bar");

            Uri url = browshot.MakeUrl("screenshot/create", arguments);

            Assert.AreEqual("https://api.browshot.com/api/v1/screenshot/create?&key=API_KEY&%2b%3d%26%25=bar", url.ToString());
        }



        [TestMethod]
        public void GenericError()
        {
            Object test = browshot.GenericError("This is a test");
            
        }
    }
}
