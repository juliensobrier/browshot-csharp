using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using Browshot;

namespace Tests
{
    [TestClass]
    public class Account
    {
        private BrowshotClient browshot;

        [TestInitialize]
        public void SetUp()
        {
            browshot = new BrowshotClient("vPTtKKLBtPUNxVwwfEKlVvekuxHyTXyi"); // test1
        }

        [TestMethod]
        public void AccountInfo()
        {
            Dictionary<string, object> info = browshot.AccountInfo();

            Assert.IsNotNull(info);

            Assert.IsTrue(info.ContainsKey("balance"));
            Assert.IsTrue(int.Parse(info["balance"].ToString()) >= 0);

            Assert.IsTrue(info.ContainsKey("private_instances"));
            Assert.AreEqual("0", info["private_instances"].ToString());
        }
    }
}
