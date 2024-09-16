using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using SynologyAPI.Types;
using SynologyAPI.Data;

namespace SynologyAPI
{
    public class Client
    {
        HttpClient HttpClient;

        string SID = "";

        public Client(string InAddress, int InPort, TimeSpan InTimeout)
        {
            HttpClient = new()
            {
                BaseAddress = new Uri(InAddress + ":" + InPort),
                Timeout = InTimeout,
            };

            Trace.WriteLine("HTTP Client created - addr: " + InAddress + ":" + InPort + " timeout:" + InTimeout.Seconds);
        }

        public bool Connect()
        {
            Trace.WriteLine("Connecting...");

            string request = ConstructRequest(
                "query.cgi", "SYNO.API.Info", 1,
                "&method=query" +
                "&query=SYNO.API.Auth");

            Response<string> response = Request(request);
            if (response.data == "" || response.data == null)
            {
                Trace.WriteLine("Failed to connect");
                return false;
            }

            try
            {
                JObject json = JObject.Parse(response.data);
                if (!(bool)json["success"])
                    return false;

                APIInfo info = json["data"]["SYNO.API.Auth"].ToObject<APIInfo>();
                if (info.maxVersion < 1 || info.minVersion < 1)
                {
                    Trace.WriteLine("Invalid Auth API version. Max: " + info.maxVersion + " Min: " + info.minVersion);
                    return false;
                }
                if (info.path == "")
                {
                    Trace.WriteLine("Invalid cgi path: " + info.path);
                    return false;
                }

                Trace.WriteLine("Connected.");
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }

            Trace.WriteLine("Failed to parse response");
            return false;
        }

        public bool Login(string InUser, string InPassword)
        {
            Trace.WriteLine("Logging in...");

            string request = ConstructRequest("auth.cgi", "SYNO.API.Auth", 3,
                "&method=login" +
                "&account=" + InUser +
                "&passwd=" + InPassword +
                "&session=FileStation" +
                "&format=cookie");
            var result = Request<Login>(request);

            if (!result.success || result.data == null)
                return false;

            SID = result.data.sid;

            Trace.WriteLine("Logged in.");
            return true;
        }

        public void Logout()
        {
            Trace.WriteLine("Logging out...");

            string request = ConstructRequest("auth.cgi", "SYNO.API.Auth", 1,
                "&method=logout" +
                "&session=FileStation");

            Response<string> result = Request(request);
            Trace.WriteLine("Logged out.");
        }

        public List<string> List(string InFolderPath, List<string> InAdditionalParameters)
        {
            // /webapi/entry.cgi?api=SYNO.FileStation.List&version=2&method=list&additional=%5B%22real_path%22%2C%22size%22%2C%22owner%22%2C%22time%2Cperm%22%2C%22type%22%5D&folder_path=%22%2Fvideo%22

            InFolderPath.Replace("/", "%2F");
            InFolderPath.Replace("\\", "%2F");
            InFolderPath = "%22" + InFolderPath + "%22";

            string additional = "";
            additional += "%5B"; // {
            bool first = true;
            foreach (var param in InAdditionalParameters)
            {
                if (!first)
                    additional += "%2C"; // ,
                first = false;
                additional += "%22" + param + "%22"; // "param"
            }
            additional += "%5D"; // }


            string request = ConstructRequest("entry.cgi", "SYNO.FileStation.List", 2,
                "&method=list" +
                "&folder_path=" + InFolderPath +
                "&additional=" + additional);

            var result = Request<List>(request);
            if (!result.success || result.data == null)
                return [];

            HashSet<string> dirs = new();
            HashSet<string> files = new();

            foreach (var file in result.data.files)
            {
                if (file.isdir)
                    dirs.Add(file.path);
                else
                    files.Add(file.path);
            }

            foreach (var dir in dirs)
                foreach (var file in List(dir, InAdditionalParameters))
                    files.Add(file);

            return files.ToList();
        }

        string ConstructRequest(string InRequestType, string InAPI, int InVersion, string InRequest)
        {
            return
                "/webapi/" + InRequestType +
                "?api=" + InAPI +
                "&version=" + InVersion.ToString() +
                "" + InRequest;
        }

        Response<T> Request<T>(string InRequest)
        {
            Response<string> jsonResponse = Request(InRequest);
            if (jsonResponse.success && jsonResponse.data != null)
            {
                Response<T>? response = JsonConvert.DeserializeObject<Response<T>>(jsonResponse.data);
                if (response != null)
                {
                    response.success = jsonResponse.success;
                    response.error = jsonResponse.error;
                    return response; 
                }
            }
            return new()
            {
                success = jsonResponse.success,
                error = jsonResponse.error
            };
        }

        Response<string> Request(string InRequest)
        {
            Trace.WriteLine("Request: " + InRequest);
            using HttpResponseMessage response = HttpClient.GetAsync(InRequest).Result;
            if (!response.IsSuccessStatusCode)
            {
                return new()
                {
                    success = false,
                    error = new Error()
                    {
                        code = (int)response.StatusCode
                    }
                };
            }

            var jsonResponse = response.Content.ReadAsStringAsync().Result;
            return new()
            {
                success = true,
                data = jsonResponse
            };
        }
    }
}
