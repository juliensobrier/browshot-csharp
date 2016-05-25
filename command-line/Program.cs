using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Configuration;

using Browshot;
using NDesk.Options;

namespace cmd
{
    class Program
    {
        static int verbosity = 0;
        static BrowshotClient browshot;
        static int tried = 1;

        static void Main(string[] args)
        {
            string url = LoadSetting("url", String.Empty);
            string key = LoadSetting("key", String.Empty);

            string size = LoadSetting("size", "screen");
            int delay = LoadSetting("delay", 5);
            int flash_delay = LoadSetting("flash_delay", 5);
            int quality = LoadSetting("quality", 90);
            int instance_id = LoadSetting("instance", 65);
            int cache = LoadSetting("cache", 8600);
            int shots = LoadSetting("shots", 1);
            int shot_interval = LoadSetting("shot_interval", 5);
            int screen_width = LoadSetting("screen_width", 0);
            int screen_height = LoadSetting("screen_height", 0);
            string referrer = LoadSetting("referrer", String.Empty);
            string script = LoadSetting("script", String.Empty);
            string post_data = LoadSetting("post_data", String.Empty);

            // thumbnail
            int width = LoadSetting("width", 0);
            int height = LoadSetting("height", 0);
            string file = LoadSetting("file", String.Empty);
            string format = LoadSetting("format", "png");

            bool show_help = false;

            //OptionSet option_set = new OptionSet()
            //.Add("?|help|h", "Prints out the options.", v => show_help = v != null)
            //.Add("u|url=", "the {URL} for the screenshot", v => url = v )
            //.Add("s|shots=", "the number of {SHOTS} of the same page.", v => shots = v );

            var p = new OptionSet() {
                { "u|url=", "the {URL} for the screenshot",
                  v =>  url = v },
                { "k|key=", "your API {KEY}\n" + String.Format("default = {0}", key),
                   v =>  key = v },
                { "f|file=", "{FILENAME} to use for the screenshto images\n" + String.Format("default = {0}", file),
                   v =>  file = v },
                { "i|instance=", 
                    "{INSTANCE_ID} to use\n" + String.Format("default = {0}", instance_id),
                  (int v) => instance_id = v },
                { "z|size=", 
                    "the screenshot {SIZE}, 'page' or 'screen'\n" + String.Format("default = {0}", size),
                  v => size = v },
                { "d|delay=", 
                    "number of {SECONDS} to wait after the page is loaded\n" + String.Format("default = {0}", delay),
                  (int v )=> delay = v },
                { "fd|flash_delay=", 
                    "number of {SECONDS} to wait if Flash objects are present\n" + String.Format("default = {0}", flash_delay),
                  (int v )=> flash_delay = v },
                { "o|format=", 
                    "the image {FORMAT} of the screenshot, 'png' or 'jpeg'\n" + String.Format("default = {0}", format),
                  v => format = v },
                { "q|quality=", 
                    "JPEG image {QUALITY} of the screenshot\n" + String.Format("Optional, default = {0}", quality),
                  (int v) => quality = v },
                { "s|shots=", 
                    "the number of {SHOTS} of the same page\n" + String.Format("Optional, default = {0}", shots),
                  (int v) => shots = v },
                { "si|shot_interval=", 
                    "the number of {SECONDS} between 2 screenshots of the same page\n" + String.Format("Optional, default = {0}", shot_interval),
                  (int v) => shot_interval = v },
                { "c|cache=", 
                    "get a previous screenshots if done within {SECONDS}\n" + String.Format("default = {0}", cache),
                  (int v) => cache = v },
                { "w|width=", 
                    "thumbnail {WIDTH}\n" + String.Format("default = {0}", width),
                  (int v) => width = v },
               { "e|height=", 
                    "thumbnail {HEIGHT}\n" + String.Format("default = {0}", height),
                  (int v) => height = v },
               { "sw|screen_width=", 
                    "browser screen {WIDTH}\n" + String.Format("default = {0}", screen_width),
                  (int v) => screen_width = v },
               { "sh|screen_height=", 
                    "browser screen {HEIGHT}\n" + String.Format("default = {0}", screen_height),
                  (int v) => screen_height = v },
               { "referrer=", 
                    "{REFERRER} Url\n" + String.Format("default = {0}", referrer),
                  v => referrer = v },
               { "script=", 
                    "{JAVASCRIPT} Url to inject in the page\n" + String.Format("default = {0}", script),
                  v => script = v },
               { "post_data=", 
                    "{POST DATA} for a POST request\n" + String.Format("default = {0}", post_data),
                  v => script = v },
                /*{ "v", "increase debug message verbosity",
                  v => { if (v != null) ++verbosity; } },*/
                { "h|help|?",  "show the list of options", 
                  v => show_help = v != null },
            };

            List<string> extra;
            try {
                extra = p.Parse (args);
            }
            catch (OptionException e) {
                Console.Write ("browshot.exe: ");
                Console.WriteLine (e.Message);
                Console.WriteLine ("Try `browshot.exe --help' for more information");
                return;
            }

            if (show_help)
            {
                ShowHelp(p);
                return;
            }

            if(url == String.Empty)
            {
                Console.WriteLine("Missing URL");
                ShowHelp(p);
                return;
            }

            if (key == String.Empty)
            {
                Console.WriteLine("Missing API key");
                ShowHelp(p);
                return;
            }

            /*if (file == String.Empty)
            {
                Console.WriteLine("Missing file name");
                ShowHelp(p);
                return;
            }*/

            browshot = new BrowshotClient(key, true);

            Hashtable arguments = new Hashtable();
            arguments.Add("instance_id", instance_id);
            arguments.Add("size", size);
            arguments.Add("cache", cache);
            arguments.Add("shots", shots);
            arguments.Add("delay", delay);
            arguments.Add("flash_delay", flash_delay);
            arguments.Add("screen_width", screen_width);
            arguments.Add("screen_height", screen_height);
            arguments.Add("shot_interval", shot_interval);
            arguments.Add("format", format);
            arguments.Add("quality", quality);
            arguments.Add("referrer", referrer);
            arguments.Add("script", script);
            arguments.Add("post_data", post_data);
            // ...
            arguments.Add("width", width);
            arguments.Add("height", height);
            arguments.Add("file", file);

            start(url, arguments);
        }

        static string LoadSetting(string name, string value)
        {
            if (ConfigurationManager.AppSettings[name] != null)
                return ConfigurationManager.AppSettings[name];

            return value;
        }

        static int LoadSetting(string name, int value)
        {
            if (ConfigurationManager.AppSettings[name] != null)
            {
                int result = 0;
                bool ok = int.TryParse(ConfigurationManager.AppSettings[name], out result);
                if(ok)
                    return result;
            }

            return value;
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: browshot.exe: OPTIONS");
            Console.WriteLine("Interact wit the Browshot API.");
            Console.WriteLine("https://browshot.com/api/documentation");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        static void start(String url, Hashtable arguments)
        {
            Dictionary<string, object> results = new Dictionary<string, object>();

            while(true)
            {
                Console.WriteLine("Screenshot for " + url + " - attempt: " + tried);
                results = browshot.ScreenshotCreate(url, arguments);
                tried++;
                if (results.ContainsKey("id"))
                     Console.WriteLine("ID: " + results["id"]);
                if (results.ContainsKey("status"))
                    Console.WriteLine("Status: " + results["status"]);
                if(results.ContainsKey("error") && results["error"].ToString().Length > 0)
                {
                    Console.WriteLine("Status: " + results["error"]);
                    if (tried > 3)
                    {
                        Console.WriteLine("Too many retry, give up");
                        return;
                    }
                    if (results.ContainsKey("id") == false)
                        return;

                    // try again
                    continue;
                }

                // finished or in_process
                if (results["status"].ToString().StartsWith("finished") == false)
                {
                    int wait = (int)arguments["delay"] + (int)arguments["shots"] * (int)arguments["shot_interval"] + 10;
                    Console.WriteLine(String.Format("Waiting {0} seconds...", wait));
                    Thread.Sleep(wait * 1000);  
                }
                break;
            }

            while (results["status"].ToString().StartsWith("finished") == false && results["status"].ToString().StartsWith("error") == false)
            {
                results = browshot.ScreenshotInfo(int.Parse(results["id"].ToString()));

                int wait = (int)arguments["delay"] + (int)arguments["shots"] * (int)arguments["shot_interval"] + 10;
                Console.WriteLine(String.Format("Waiting {0} seconds...", wait));
                Thread.Sleep(wait * 1000);
                
                if(results["status"].ToString().StartsWith("error"))
                {
                    Console.WriteLine("Screenshot failed");
                    if (results.ContainsKey("error") && results["error"].ToString().Length > 0)
                        Console.WriteLine("Status: " + results["error"]);

                    start(url, arguments);
                    return;
                }
            }

            Console.WriteLine("Screenshot ID: " + results["id"].ToString());

            // finished
            arguments.Add("shot", 1);
            for(int i = 1; i <= (int)arguments["shots"]; i++)
            {
                arguments["shot"] = i;
                Image thumbnail = browshot.Thumbnail(int.Parse(results["id"].ToString()), arguments);
                if (thumbnail == null)
                {
                    Console.WriteLine("Could not retrieve image for shot " + arguments["shot"]);
                    continue;
                }

                //thumbnail.Save(arguments["file"].ToString());
                string saved = Save(thumbnail, arguments["file"].ToString(), int.Parse(results["id"].ToString()), i, arguments["format"].ToString());
                Console.WriteLine("Screenshot saved to " + saved);
            }
        }

        static string Save(Image image, string path, int id = 0, int shot = 1, string format = "png")
        {
            if(path == null || path == String.Empty)
                path = String.Format("{0}.{1}", id, format);

            string filename = path;

            if(Directory.Exists(path))
            {
                if (shot <= 1)
                    filename = System.IO.Path.Combine(path, String.Format("{0}.{1}", id, format));
                else
                    filename = System.IO.Path.Combine(path, String.Format("{0}.{1}.{2}", id, shot, format));
            }
            else if(shot > 1)
            {
                // rename the server to include the shot number
                FileInfo file = new FileInfo(path);
                filename = String.Format("{0}\\{1}.{2}{3}", file.DirectoryName, file.Name.Substring(0, file.Name.Length - file.Extension.Length), shot, file.Extension);
                   
            }

            image.Save(filename);
            /*if (format == "png")
                image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
            else
                image.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);*/
            
            return filename;
        }

    }
}
