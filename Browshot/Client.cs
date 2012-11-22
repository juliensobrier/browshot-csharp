using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
//using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Drawing;

using System.Diagnostics;


namespace Browshot
{
    public class BrowshotClient
    {
        private Version version = new Version(1,10,0);

        #region Constructors

        public BrowshotClient(string key)
            :this(key, false) {}

        public BrowshotClient(string key, bool debug)
            :this(key, debug, "https://api.browshot.com/api/v1/") {}

        public BrowshotClient(string key, bool debug, string baseUrl)
        {
            this.Key = key;
            this.Debug = debug;
            this.BaseUrl = baseUrl;
        }


        #endregion


        #region Properties

        private string key = String.Empty;
        public string Key
        {
            get
            {
                return this.key;
            }

            set
            {
                this.key = value;
            }
        }

        public string baseUrl = "https://api.browshot.com/api/v1/";
        public string BaseUrl
        {
            get
            {
                return this.baseUrl;
            }

            set
            {
                this.baseUrl = value;
            }
        }



        private bool debug = false;
        public bool Debug
        {
            get
            {
                return this.debug;
            }

            set
            {
                this.debug = value;
            }
        }

        #endregion


        #region Version

        public Version APIVersion
        {
            get
            {
                return new Version(this.version.Major, this.version.Minor);
            }
        }

        #endregion


        #region Screenshot API

        public  Dictionary<string, object> ScreenshotCreate(string url, Hashtable arguments)
        {
            if(arguments == null)
                arguments = new Hashtable();

            if (arguments.ContainsKey("url"))
                throw new Exception("URL can be added in the list of arguments");
            if(url == null || url == String.Empty)
                throw new Exception("URL is invalid");

            arguments.Add("url", url);

            return  (Dictionary<string, object>)Reply("screenshot/create", arguments);
        }


        public Dictionary<string, object> ScreenshotInfo(int id)
        {
            if (id == 0)
                throw new Exception("ID is invalid");

            Hashtable arguments = new Hashtable();
            arguments.Add("id", id);

            return (Dictionary<string, object>)Reply("screenshot/info", arguments);
        }


        public Dictionary<string, object> ScreenshotList(int limit = 100)
        {
            if (limit < 0 || limit > 100)
                throw new Exception("limit is invalid (0 to 100)");

            Hashtable arguments = new Hashtable();
            arguments.Add("limit", limit);

            return (Dictionary<string, object>)Reply("screenshot/list", arguments);
        }


        public Dictionary<string, object> ScreenshotHost(int id, Hashtable arguments)
        {
            if (id <= 0)
                throw new Exception("ID is invalid");

            if (arguments == null)
                arguments = new Hashtable();

            if (arguments.ContainsKey("id"))
                throw new Exception("ID can be added in the list of arguments");

            arguments.Add("id", id);


            return (Dictionary<string, object>)Reply("screenshot/host", arguments);
        }


        public Image Thumbnail(int id, Hashtable arguments)
        {
            if (id <= 0)
                throw new Exception("ID is invalid");

            if (arguments == null)
                arguments = new Hashtable();

            if (arguments.ContainsKey("id"))
                throw new Exception("ID can be added in the list of arguments");

            arguments.Add("id", id);


            Uri url = MakeUrl("screenshot/thumbnail", arguments);
            HttpWebRequest request = HttpWebRequest.CreateHttp(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Image image = null;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        do
                        {
                            bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                            memoryStream.Write(buffer, 0, bytesRead);
                        } while (bytesRead != 0);

                        image = Image.FromStream(memoryStream);
                    }
                }
            }

            return image;
        }

        public Dictionary<string, object> ScreenshotShare(int id, Hashtable arguments)
        {
            if (id <= 0)
                throw new Exception("ID is invalid");

            if (arguments == null)
                arguments = new Hashtable();

            if (arguments.ContainsKey("id"))
                throw new Exception("ID can be added in the list of arguments");

            arguments.Add("id", id);


            return (Dictionary<string, object>)Reply("screenshot/share", arguments);
        }

        public Dictionary<string, object> ScreenshotDelete(int id, Hashtable arguments)
        {
            if (id <= 0)
                throw new Exception("ID is invalid");

            if (arguments == null)
                arguments = new Hashtable();

            if (arguments.ContainsKey("id"))
                throw new Exception("ID can be added in the list of arguments");

            arguments.Add("id", id);


            return (Dictionary<string, object>)Reply("screenshot/delete", arguments);
        }

        #endregion

        #region Account API

        public Dictionary<string, object> AccountInfo()
        {
            return (Dictionary<string, object>)Reply("account/info", new Hashtable());
        }


        #endregion


        #region Instance API

        public Dictionary<string, object> InstanceList()
        {
            return (Dictionary<string, object>)Reply("instance/list", new Hashtable());
        }

        public Dictionary<string, object> InstanceInfo(int id)
        {
            if (id <= 0)
                throw new Exception("ID is invalid");

            Hashtable arguments = new Hashtable();
            arguments.Add("id", id);

            return (Dictionary<string, object>)Reply("instance/info", arguments);
        }

        public Dictionary<string, object> InstanceCreate(Hashtable arguments)
        {
            if (arguments == null)
                throw new Exception("Arguments are invalid");

            return (Dictionary<string, object>)Reply("instance/create", arguments);
        }

        #endregion

        #region Browser API

        public Dictionary<string, object> BrowserList()
        {
            return (Dictionary<string, object>)Reply("browser/list", new Hashtable());
        }

        public Dictionary<string, object> BrowserInfo(int id)
        {
            if (id <= 0)
                throw new Exception("ID is invalid");

            Hashtable arguments = new Hashtable();
            arguments.Add("id", id);

            return (Dictionary<string, object>)Reply("browser/info", arguments);
        }

        public Dictionary<string, object> BrowserCreate(Hashtable arguments)
        {
            if (arguments == null)
                throw new Exception("Arguments are invalid");

            return (Dictionary<string, object>)Reply("browser/create", arguments);
        }

        #endregion

        #region Simple API

        public Image Simple(string url, Hashtable arguments)
        {
            if(url == String.Empty || url == null)
                throw new Exception("URL is missing");

            if (arguments == null)
                arguments = new Hashtable();

            arguments.Add("url", url);

            Uri uri = MakeUrl("simple", arguments);
            HttpWebRequest request = HttpWebRequest.CreateHttp(uri);
            request.AllowAutoRedirect = true;
            request.MaximumAutomaticRedirections = 15;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Image image = null;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        do
                        {
                            bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                            memoryStream.Write(buffer, 0, bytesRead);
                        } while (bytesRead != 0);

                        image = Image.FromStream(memoryStream);
                    }
                }
            }

            return image;
        }

        #endregion

        #region Private methods

        private Object Reply(string action, Hashtable arguments)
        {
            Uri url = MakeUrl(action, arguments);

            HttpWebRequest request = HttpWebRequest.CreateHttp(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string result = String.Empty;

                using (Stream responseStream = response.GetResponseStream())
                {
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

                    StreamReader readStream = new StreamReader(responseStream, encode);
                    Char[] read = new Char[256];
                    int count = readStream.Read(read, 0, 256);
                    while (count > 0)
                    {
                        String str = new String(read, 0, count);
                        result += str;
                        count = readStream.Read(read, 0, 256);
                    }


                    response.Close();
                    readStream.Close();
                }
                //Trace.WriteLine(result);
                    
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.DeserializeObject(result);
            }
            else
                return null;
        }

        public Object GenericError(string message)
        {
            string json = String.Format("{{\"error\" : 1, \"message\": \"{0}\" }}", message); // Need to JSOn encode
 
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.DeserializeObject(json);
        }

        public Uri MakeUrl(string action, Hashtable arguments)
        {
            UriBuilder builder = new UriBuilder(this.BaseUrl);
            builder.Path = builder.Path + action;

            if (arguments == null)
                arguments = new Hashtable();

            if(arguments.ContainsKey("key"))
                throw new Exception("Do not add the API key in the parameters");

            arguments.Add("key", this.Key);

            if (arguments != null)
            {
                StringBuilder query = new StringBuilder();
                foreach (DictionaryEntry pair in arguments)
                {
                    query.Append("&");
                    query.Append(HttpUtility.UrlEncode(pair.Key.ToString()));
                    query.Append("=");
                    query.Append(HttpUtility.UrlEncode(pair.Value.ToString()));
                }

                builder.Query = query.ToString();
            }

            return builder.Uri;
        }

        #endregion
    }
}
