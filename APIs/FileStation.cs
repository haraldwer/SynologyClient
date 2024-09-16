
using Synology.DataTypes;

namespace Synology
{
    public class FileStation : BaseAPI
    {
        internal FileStation(Client InClient) : base(InClient) { }

        string FormatParameters(List<string> InParameters)
        {
            string formatted = "";
            formatted += "%5B"; // {
            bool first = true;
            foreach (var param in InParameters)
            {
                if (!first)
                    formatted += "%2C"; // ,
                first = false;
                formatted += "%22" + param + "%22"; // "param"
            }
            formatted += "%5D"; // }
            return formatted;
        }

        public Response<FileList> List(string InFolderPath, List<string> InAdditionalParameters)
        {
            InFolderPath.Replace("/", "%2F");
            InFolderPath.Replace("\\", "%2F");
            InFolderPath = "%22" + InFolderPath + "%22";

            // Example: /webapi/entry.cgi?api=SYNO.FileStation.List&version=2&method=list&additional=%5B%22real_path%22%2C%22size%22%2C%22owner%22%2C%22time%2Cperm%22%2C%22type%22%5D&folder_path=%22%2Fvideo%22
            string request = OwningClient.ConstructRequest("entry.cgi", "SYNO.FileStation.List", 2,
                "&method=list" +
                "&folder_path=" + InFolderPath +
                "&additional=" + FormatParameters(InAdditionalParameters));

            return OwningClient.Request<FileList>(request);
        }

        public Response<SharedDriveList> ListSharedDrives(List<string> InAdditionalParameters)
        {
            // Example: /webapi/entry.cgi?api=SYNO.FileStation.List&version=2&method=list_share&additional=%5B%22real_path%22%2C%22owner%2Ctime%22%5D
            string request = OwningClient.ConstructRequest("entry.cgi", "SYNO.FileStation.List", 2,
                "&method=list_share" +
                "&additional=" + FormatParameters(InAdditionalParameters));

            return OwningClient.Request<SharedDriveList>(request);
        }
    }
}
