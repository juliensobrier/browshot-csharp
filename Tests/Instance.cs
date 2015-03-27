using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Diagnostics;

using Browshot;

namespace Tests
{
    [TestClass]
    public class Instance
    {
        private BrowshotClient browshot;

        [TestInitialize]
        public void SetUp()
        {
            browshot = new BrowshotClient("vPTtKKLBtPUNxVwwfEKlVvekuxHyTXyi", false); // vPTtKKLBtPUNxVwwfEKlVvekuxHyTXyi=test1
        }

        [TestMethod]
        public void InstanceList()
        {
            Dictionary<string, object> list = browshot.InstanceList();

            Assert.IsNotNull(list);

            Assert.IsTrue(list.Count > 0);
            Assert.IsTrue(list.ContainsKey("shared"));
            Assert.IsTrue(list.ContainsKey("free"));

            Object[] free = (Object[])list["free"];
            Assert.IsTrue(free.Length > 0);

            Dictionary<string, object> first = (Dictionary<string, object>)free[0];
            Assert.IsTrue(first.ContainsKey("type"));
            Assert.AreEqual("public", first["type"].ToString());

            Assert.IsTrue(first.ContainsKey("width"));
            Assert.IsTrue(first.ContainsKey("height"));
        }

        [TestMethod]
        public void InstanceInfo()
        {
            Dictionary<string, object> list = browshot.InstanceList();

            Assert.IsNotNull(list);

            Assert.IsTrue(list.Count > 0);
            Assert.IsTrue(list.ContainsKey("free"));

            Object[] free = (Object[])list["free"];
            Assert.IsTrue(free.Length > 0);

            Dictionary<string, object> first = (Dictionary<string, object>)free[0];
            foreach(string key in first.Keys)
                Trace.WriteLine(key + ": " + first[key].ToString());
            Dictionary<string, object> info = browshot.InstanceInfo(int.Parse(first["id"].ToString()));

            Assert.IsTrue(info.ContainsKey("type"));
            Assert.AreEqual("public", info["type"].ToString());

            Assert.IsTrue(info.ContainsKey("width"));
            Assert.IsTrue(info.ContainsKey("height"));
        }
    }
}
