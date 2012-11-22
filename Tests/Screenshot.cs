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
    public class Screenshot
    {
        private BrowshotClient browshot;

        [TestInitialize]
        public void SetUp()
        {
            browshot = new BrowshotClient("vPTtKKLBtPUNxVwwfEKlVvekuxHyTXyi"); // test1
        }

        #region ScreenshotCreate

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ScreenshotCreateIncomplete()
        {
            browshot.ScreenshotCreate("", null);
        }

        [TestMethod]
        public void ScreenshotCreateGoogle()
        {
            Hashtable arguments = new Hashtable();
            arguments.Add("cache", 8600);
            Dictionary<string, object> result = browshot.ScreenshotCreate("http://www.google.com/", arguments);

            Assert.IsNotNull(result);

            Assert.IsTrue(result.ContainsKey("status"));

            if (result["status"].ToString() == "ok")
            {
                Assert.IsTrue(result.ContainsKey("url"));
                Assert.AreEqual("http://www.google.com/", result["url"].ToString());

                Assert.IsTrue(result.ContainsKey("size"));
                Assert.AreEqual("screen", result["size"].ToString());
            }

            Assert.IsTrue(result.ContainsKey("priority"));
            Assert.IsTrue(int.Parse(result["priority"].ToString()) >= 1);
        }

        #endregion

        [TestMethod]
        public void ScreenshotInfo()
        {
            Hashtable arguments = new Hashtable();
            arguments.Add("cache", 8600);
            Dictionary<string, object> result = browshot.ScreenshotCreate("http://www.google.com/", arguments);

            Assert.IsNotNull(result);

            Assert.IsTrue(result.ContainsKey("id"));
            int id = int.Parse(result["id"].ToString());
            Assert.IsTrue(id > 1);

            result = browshot.ScreenshotInfo(id);
            Assert.IsTrue(result.ContainsKey("url"));
            //Assert.AreEqual("http://www.google.com/", result["url"].ToString());

            Assert.IsTrue(result.ContainsKey("size"));
            Assert.AreEqual("screen", result["size"].ToString());

            Assert.IsTrue(result.ContainsKey("id"));
            Assert.AreEqual(id, int.Parse(result["id"].ToString()));
        }

        [TestMethod]
        public void ScreenshotList()
        {
            Dictionary<string, object> result = browshot.ScreenshotList();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            string keyId = String.Empty;
            foreach (string key in result.Keys)
            {
                keyId = key;
                continue;
            }

            Assert.AreNotEqual(String.Empty, keyId);

            /*int id = int.Parse(keyId);
            Assert.AreNotEqual(0, id);*/

            Dictionary<string, object> screenshot = (Dictionary<string, object>)result[keyId];

            Assert.IsTrue(screenshot.ContainsKey("url"));
            Assert.IsTrue(screenshot.ContainsKey("size"));
        }

        [TestMethod]
        public void ScreenshotHost()
        {
            Dictionary<string, object> result = browshot.ScreenshotList();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            string keyId = String.Empty;
            foreach (string key in result.Keys)
            {
                Dictionary<string, object> screenshot = (Dictionary<string, object>)result[key];

                if (screenshot["status"].ToString() == "finished")
                {
                    keyId = key;
                    break;
                }
            }

            Assert.AreNotEqual(String.Empty, keyId);

            Dictionary<string, object> host = browshot.ScreenshotHost(int.Parse(keyId), null);
            Assert.IsTrue( host.ContainsKey("status") );
            //Assert.AreEqual("ok", host["status"].ToString());
        }

        [TestMethod]
        public void Thumbnail()
        {
            Dictionary<string, object> result = browshot.ScreenshotList();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            string keyId = String.Empty;
            foreach (string key in result.Keys)
            {
                Dictionary<string, object> screenshot = (Dictionary<string, object>)result[key];

                if (screenshot["status"].ToString() == "finished")
                {
                    keyId = key;
                    break;
                }
            }

            Assert.AreNotEqual(String.Empty, keyId);

            Image thumbnail = browshot.Thumbnail(int.Parse(keyId), null);
            Assert.IsNotNull(thumbnail);
            Assert.IsTrue(thumbnail.Size.Height >= 768, thumbnail.Size.Height.ToString());
            Assert.IsTrue(thumbnail.Size.Width >= 1024, thumbnail.Size.Width.ToString());

            /*string folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string file = folder + "/browshot.temp.png";
            thumbnail.Save(file);*/
        }

        [TestMethod]
        public void ScreenshotShare()
        {
            Dictionary<string, object> result = browshot.ScreenshotList();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            string keyId = String.Empty;
            foreach (string key in result.Keys)
            {
                Dictionary<string, object> screenshot = (Dictionary<string, object>)result[key];

                if (screenshot["status"].ToString() == "finished")
                {
                    keyId = key;
                    break;
                }
            }

            Assert.AreNotEqual(String.Empty, keyId);

            Dictionary<string, object> host = browshot.ScreenshotShare(int.Parse(keyId), null);
            Assert.IsTrue(host.ContainsKey("status"));
            Assert.AreEqual("ok", host["status"].ToString());

            Assert.IsTrue(host.ContainsKey("url"));
        }

        [TestMethod]
        public void ScreenshotDelete()
        {
            Dictionary<string, object> result = browshot.ScreenshotList();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            string keyId = String.Empty;
            foreach (string key in result.Keys)
            {
                Dictionary<string, object> screenshot = (Dictionary<string, object>)result[key];

                if (screenshot["status"].ToString() == "finished")
                {
                    keyId = key;
                    break;
                }
            }

            Assert.AreNotEqual(String.Empty, keyId);

            Dictionary<string, object> host = browshot.ScreenshotDelete(int.Parse(keyId), null);
            Assert.IsTrue(host.ContainsKey("status"));
            Assert.AreEqual("ok", host["status"].ToString());

            Assert.IsTrue(host.ContainsKey("id"));
            Assert.AreEqual(keyId, host["id"].ToString());
        }

    }
}
