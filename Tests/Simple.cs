using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.IO;

using Browshot;

namespace Tests
{
    [TestClass]
    public class Simple
    {
        private BrowshotClient browshot;

        [TestInitialize]
        public void SetUp()
        {
            browshot = new BrowshotClient("vPTtKKLBtPUNxVwwfEKlVvekuxHyTXyi"); // test1
        }

        [TestMethod]
        public void TestSimple()
        {
            Image thumbnail = browshot.Simple("http://www.google.com/", null);
            Assert.IsNotNull(thumbnail);
            Assert.IsTrue(thumbnail.Size.Height >= 768, thumbnail.Size.Height.ToString());
            Assert.IsTrue(thumbnail.Size.Width >= 1024, thumbnail.Size.Width.ToString());

            string folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string file = folder + "/browshot-simple.png";
            thumbnail.Save(file);
        }
    }
}
