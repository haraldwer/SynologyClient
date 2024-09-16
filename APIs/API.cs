
using Newtonsoft.Json.Linq;
using Synology.DataTypes;
using System.Diagnostics;

namespace Synology
{
    public class API : BaseAPI
    {
        string SID = "";

        internal API(Client InClient) : base(InClient) { }

        public async Task<Response<string>> Connect()
        {
            string request = OwningClient.ConstructRequest(
                "query.cgi", "SYNO.API.Info", 1,
                "&method=query" +
                "&query=SYNO.API.Auth");

            Response<HttpResponseMessage> response = await OwningClient.Request(request);
            if (!response.success)
            {
                return new()
                {
                    success = false,
                    error = response.error,
                };
            }

            // Parse json manually, because the format is weird
            try
            {
                string content = response.data.Content.ReadAsStringAsync().Result;
                JObject json = JObject.Parse(content);
                APIInfo info = json["data"]["SYNO.API.Auth"].ToObject<APIInfo>();
                if (info.maxVersion < 1 || info.minVersion < 1)
                {
                    Trace.WriteLine("Invalid Auth API version. Max: " + info.maxVersion + " Min: " + info.minVersion);
                    return new()
                    {
                        success = false
                    };
                }
                if (info.path == "")
                {
                    Trace.WriteLine("Invalid cgi path: " + info.path);
                    return new()
                    {
                        success = false
                    };
                }
                return new()
                {
                    success = true,
                    data = ""
                };
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                Trace.WriteLine("Failed to parse response");
                return new()
                {
                    success = false,
                    error = new() 
                    {                         
                        code = 2
                    },
                };
            }
        }

        public async Task<Response<Login>> Login(string InUser, string InPassword, string InSession = "DotNet")
        {
            string request = OwningClient.ConstructRequest("auth.cgi", "SYNO.API.Auth", 3,
                "&method=login" +
                "&account=" + InUser +
                "&passwd=" + InPassword +
                "&session=" + InSession + 
                "&format=cookie");

            var result = await OwningClient.Request<Login>(request);
            if (!result.success || result.data == null)
                return result;
            SID = result.data.sid;
            return result;
        }

        public async Task<Response<string>> Logout(string InSession = "DotNet")
        {
            string request = OwningClient.ConstructRequest("auth.cgi", "SYNO.API.Auth", 1,
                "&method=logout" +
                "&session=" + InSession);

            Response<string> result = await OwningClient.Request<string>(request);
            if (result.success)
                SID = "";
            return result;
        }
    }
}
