
using Newtonsoft.Json.Linq;
using Synology.DataTypes;
using System.Diagnostics;

namespace Synology
{
    public class API : BaseAPI
    {
        string SID = "";

        internal API(Client InClient) : base(InClient) { }

        public bool Connect()
        {
            string request = OwningClient.ConstructRequest(
                "query.cgi", "SYNO.API.Info", 1,
                "&method=query" +
                "&query=SYNO.API.Auth");

            Response<string> response = OwningClient.Request(request);
            if (!response.success)
                return false;

            // Parse json manually, because the format is weird
            try
            {
                JObject json = JObject.Parse(response.data);
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
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                Trace.WriteLine("Failed to parse response");
                return false;
            }
        }

        public bool Login(string InUser, string InPassword, string InSession = "DotNet")
        {
            string request = OwningClient.ConstructRequest("auth.cgi", "SYNO.API.Auth", 3,
                "&method=login" +
                "&account=" + InUser +
                "&passwd=" + InPassword +
                "&session=" + InSession + 
                "&format=cookie");

            var result = OwningClient.Request<Login>(request);
            if (!result.success || result.data == null)
                return false;
            SID = result.data.sid;
            return true;
        }

        public bool Logout(string InSession = "DotNet")
        {
            string request = OwningClient.ConstructRequest("auth.cgi", "SYNO.API.Auth", 1,
                "&method=logout" +
                "&session=" + InSession);

            Response<string> result = OwningClient.Request(request);
            if (result.success)
                SID = ""; 
            return result.success;
        }
    }
}
