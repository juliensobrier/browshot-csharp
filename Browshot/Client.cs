using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections;
using System.IO;
using System.Drawing;

using System.Diagnostics;


/*! \mainpage browshot-csharp is a free and open-source library for the Browshot API.
 * <p>You need to get a free API key at https://browshot.com/ to use this library.</p>
 * <p>For examples on how to use the library, take a look at the unit tests./p>
 * <p>Before you use this library, please take a look at the API documentation 
 * at http://browshot.com/api/documentation.<br /></p>
 * <p>The source code can be found at https://github.com/juliensobrier/browshot-csharp. Patches are welcome!/p>
 * <p>This project has been created with Visual Studio Express 2012 (free version).</p>
 * <p>Implementations for Perl, Python, Ruby and PHP are available at https://browshot.com/api/libraries. 
 * Announcements about the API and the libraries are on our blog at http://blog.browshot.com/
 * 
 * */

namespace Browshot
{
    /// <summary>
    /// c# client to interact with the Browshot API. See http://browshot.com/api/documentation for information about the API.
    /// </summary>
    public class BrowshotClient
    {
        private Version version = new Version(1,10,0);

        #region /// @name Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">API Key</param>
        public BrowshotClient(string key)
            :this(key, false) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">API Key</param>
        /// <param name="debug">Debug flag (not used curently)</param>
        public BrowshotClient(string key, bool debug)
            :this(key, debug, "https://api.browshot.com/api/v1/") {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">API Key</param>
        /// <param name="debug">Debug flag (not used curently)</param>
        /// <param name="baseUrl">Change URL to conect to he Browshot API</param>
        public BrowshotClient(string key, bool debug, string baseUrl)
        {
            this.Key = key;
            this.Debug = debug;
            this.BaseUrl = baseUrl;
        }


        #endregion


        #region /// @name Properties

        private string key = String.Empty;
        /// <summary>
        /// API key
        /// </summary>
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
        /// <summary>
        /// Base url of the Browshot API.
        /// </summary>
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
        /// <summary>
        /// Debug flag (not used currently)
        /// </summary>
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


        #region /// @name Version

        /// <summary>
        /// API version managed by this library.
        /// </summary>
        public Version APIVersion
        {
            get
            {
                return new Version(this.version.Major, this.version.Minor);
            }
        }

        #endregion


        #region /// @name Screenshot API

        /// <summary>
        /// Create a custom browser. See http://browshot.com/api/documentation#browser_create for the response format
        /// </summary>
        /// <param name="url">URL of the website to create a screenshot of.</param>
        /// <param name="arguments">See http://browshot.com/api/documentation#screenshot_create for the full list of possible arguments.</param>
        /// <returns>JSON output</returns>
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


        /// <summary>
        /// Get information about a screenshot requested previously. See http://browshot.com/api/documentation#screenshot_info for the response format.
        /// </summary>
        /// <param name="id">Screenshot ID</param>
        /// <returns>JSON output</returns>
        public Dictionary<string, object> ScreenshotInfo(int id)
        {
            if (id == 0)
                throw new Exception("ID is invalid");

            Hashtable arguments = new Hashtable();
            arguments.Add("id", id);

            return (Dictionary<string, object>)Reply("screenshot/info", arguments);
        }

        /// <summary>
        /// Get details about screenshots requested. See http://browshot.com/api/documentation#screenshot_list for the response format.
        /// </summary>
        /// <param name="limit">Number of screenshots tolist.</param>
        /// <returns>JSON output</returns>
        public Dictionary<string, object> ScreenshotList(int limit = 100)
        {
            if (limit < 0 || limit > 100)
                throw new Exception("limit is invalid (0 to 100)");

            Hashtable arguments = new Hashtable();
            arguments.Add("limit", limit);

            return (Dictionary<string, object>)Reply("screenshot/list", arguments);
        }

        /// <summary>
        /// Host a screenshot or thumbnail. See http://browshot.com/api/documentation#screenshot_host for the response format.
        /// </summary>
        /// <param name="id">Screenshot ID</param>
        /// <param name="arguments">See http://browshot.com/api/documentation#screenshot_host for the full list of possible arguments.</param>
        /// <returns>JSON output</returns>
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

        /// <summary>
        /// Retrieve the screenshot, or a thumbnail. See http://browshot.com/api/documentation#screenshot_thumbnail for the response format.
        /// </summary>
        /// <param name="id">Screenshot ID</param>
        /// <param name="arguments">See http://browshot.com/api/documentation#screenshot_thumbnail for the full list of possible arguments.</param>
        /// <returns>Thumbnail bitmap</returns>
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

        /// <summary>
        /// Share a screenshot. See http://browshot.com/api/documentation#screenshot_share for the response format.
        /// </summary>
        /// <param name="id">Screenshot ID</param>
        /// <param name="arguments">See http://browshot.com/api/documentation#screenshot_share for the full list of possible arguments.</param>
        /// <returns>JSON output</returns>
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

        /// <summary>
        /// Delete details of a screenshot. See http://browshot.com/api/documentation#screenshot_delete for the response format.
        /// </summary>
        /// <param name="id">Screenshot ID</param>
        /// <param name="arguments">See http://browshot.com/api/documentation#screenshot_delete for the full list of possible arguments.</param>
        /// <returns>JSON output</returns>
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

        #region /// @name Account API

        /// <summary>
        /// Return information about the user account. See http://browshot.com/api/documentation#account_info for the response format.
        /// </summary>
        /// <returns>JSON output</returns>
        public Dictionary<string, object> AccountInfo()
        {
            return (Dictionary<string, object>)Reply("account/info", new Hashtable());
        }


        #endregion

        #region /// @name Instance API

        /// <summary>
        /// Return the list of instances as a hash reference. See http://browshot.com/api/documentation#instance_list for the response format.
        /// </summary>
        /// <returns>JSON output</returns>
        public Dictionary<string, object> InstanceList()
        {
            return (Dictionary<string, object>)Reply("instance/list", new Hashtable());
        }

        /// <summary>
        /// Return the details of an instance. See http://browshot.com/api/documentation#instance_info for the response format.
        /// </summary>
        /// <param name="id">Instance ID</param>
        /// <returns>JSON output</returns>
        public Dictionary<string, object> InstanceInfo(int id)
        {
            if (id <= 0)
                throw new Exception("ID is invalid");

            Hashtable arguments = new Hashtable();
            arguments.Add("id", id);

            return (Dictionary<string, object>)Reply("instance/info", arguments);
        }

        /// <summary>
        /// Create a private instance. See http://browshot.com/api/documentation#instance_create for the response format.
        /// </summary>
        /// <param name="arguments">See http://browshot.com/api/documentation#instance_create for the full list of possible arguments.</param>
        /// <returns>JSON output</returns>
        public Dictionary<string, object> InstanceCreate(Hashtable arguments)
        {
            if (arguments == null)
                throw new Exception("Arguments are invalid");

            return (Dictionary<string, object>)Reply("instance/create", arguments);
        }

        #endregion

        #region /// @name Browser API

        /// <summary>
        /// Return the list of browsers as a hash reference. See http://browshot.com/api/documentation#browser_list for the response format.
        /// </summary>
        /// <returns>JSON output</returns>
        public Dictionary<string, object> BrowserList()
        {
            return (Dictionary<string, object>)Reply("browser/list", new Hashtable());
        }

        /// <summary>
        /// Return the details of a browser. See http://browshot.com/api/documentation#browser_info for the response format.
        /// </summary>
        /// <param name="id">Browser ID</param>
        /// <returns>JSON output</returns>
        public Dictionary<string, object> BrowserInfo(int id)
        {
            if (id <= 0)
                throw new Exception("ID is invalid");

            Hashtable arguments = new Hashtable();
            arguments.Add("id", id);

            return (Dictionary<string, object>)Reply("browser/info", arguments);
        }

        /// <summary>
        /// Create a custom browser. See http://browshot.com/api/documentation#browser_create for the response format.
        /// </summary>
        /// <param name="arguments">See http://browshot.com/api/documentation#browser_create for the full list of possible arguments.</param>
        /// <returns>JSON output</returns>
        public Dictionary<string, object> BrowserCreate(Hashtable arguments)
        {
            if (arguments == null)
                throw new Exception("Arguments are invalid");

            return (Dictionary<string, object>)Reply("browser/create", arguments);
        }

        #endregion

        #region /// @name Simple API

        /// <summary>
        /// Retrieve a screenshot in one function. 
        /// Note: by default, screenshots are cached for 24 hours. You can tune this value with the cache=X parameter.
        /// </summary>
        /// <param name="url">URL of the website to create a screenshot of.</param>
        /// <param name="arguments">>See http://browshot.com/api/documentation#screenshot_create for the full list of possible arguments.</param>
        /// <returns>Thumbnail image</returns>
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

        #region /// @name Private methods

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
