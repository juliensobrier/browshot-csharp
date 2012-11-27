using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Browshot;

namespace BrowshotClientTests
{
    [TestClass]
    public class APIVersion
    {
        private BrowshotClient browshot;

        [TestInitialize]
        public void SetUp()
        {
            browshot = new BrowshotClient("API_KEY");
        }

        [TestMethod]
        public void TestAPIVersion()
        {
            Assert.AreEqual(browshot.APIVersion, new Version(1,10));
        }
    }
}
