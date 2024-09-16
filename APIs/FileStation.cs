
using Synology.DataTypes;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace Synology
{
    public class FileStation : BaseAPI
    {
        internal FileStation(Client InClient) : base(InClient) { }

        string FormatPath(string InPath)
        {
            InPath.Replace("/", "%2F");
            InPath.Replace("\\", "%2F");
            InPath = "%22" + InPath + "%22";
            return InPath;
        }

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

        public async Task<Response<FileList>> List(string InFolderPath, List<string> InAdditionalParameters)
        {
            // Example: /webapi/entry.cgi?api=SYNO.FileStation.List&version=2&method=list&additional=%5B%22real_path%22%2C%22size%22%2C%22owner%22%2C%22time%2Cperm%22%2C%22type%22%5D&folder_path=%22%2Fvideo%22
            string request = OwningClient.ConstructRequest("entry.cgi", "SYNO.FileStation.List", 2,
                "&method=list" +
                "&folder_path=" + FormatPath(InFolderPath) +
                "&additional=" + FormatParameters(InAdditionalParameters));

            return await OwningClient.Request<FileList>(request);
        }

        public async Task<Response<SharedDriveList>> ListSharedDrives(List<string> InAdditionalParameters)
        {
            // Example: /webapi/entry.cgi?api=SYNO.FileStation.List&version=2&method=list_share&additional=%5B%22real_path%22%2C%22owner%2Ctime%22%5D
            string request = OwningClient.ConstructRequest("entry.cgi", "SYNO.FileStation.List", 2,
                "&method=list_share" +
                "&additional=" + FormatParameters(InAdditionalParameters));

            return await OwningClient.Request<SharedDriveList>(request);
        }

        public async Task<Response<HttpResponseMessage>> Upload(string InPath, string InFileName, byte[] InFileContent, bool InCreateParents = true, bool InOverwrite = false)
        {
            // Example: 
            /*
            Content-Length:20326728
            Content-type: multipart/form-data, boundary=AaB03x
            \--AaB03x
            content-disposition: form-data; name="api"
            SYNO.FileStation.Upload
            \--AaB03x
            content-disposition: form-data; name="version"
            2
            \--AaB03x
            content-disposition: form-data; name="method"
            upload
            \--AaB03x
            content-disposition: form-data; name="path"
            /upload/test
            \--AaB03x
            content-disposition: form-data; name="create_parents"
            true
            \--AaB03x
            content-disposition: form-data; name="file"; filename="file1.txt"
            Content-Type: application/octet-stream
            ... contents of file1.txt ...
            \--AaB03x--
            */

            // TODO: Improve this, build using httpcontent instead

            string boundary = "AaB03x";
            string body = "";
            Action<string, string> addParameter = (string paramName, string param) =>
            {
                body += "\\--" + boundary + "\r\n";
                body += "content-disposition: form-data; name=\"" + paramName + "\"\r\n";
                body += param + "\r\n";
            };
            addParameter("api", "SYNO.FileStation.Upload");
            addParameter("version", "2");
            addParameter("method", "upload");
            addParameter("path", InPath);
            addParameter("create_parents", (InCreateParents ? "true" : "false"));
            addParameter("overwrite", (InOverwrite ? "true" : "false"));
            body += "\\--" + boundary + "\r\n";

            body += "content-disposition: form-data; name=\"file\"; filename=\"" + InFileName + "\"\r\n";
            body += "Content-Type: application/octet-stream\r\n";
            body += System.Text.Encoding.Default.GetString(InFileContent);
            body += "\r\n\\--" + boundary + "--\r\n";

            string content = "";
            content += "Content-Length:" + body.Length + "\r\n";
            content += "Content-type: multipart/form-data, boundary=" + boundary + "\r\n";
            content += body;

            return await OwningClient.Request("/webapi/entry.cgi", new StringContent(content));
        }

        public async Task<Response<byte[]>> Download(string InPath)
        {
            // Example: /webapi/entry.cgi?api=SYNO.FileStation.Download&version=2&method=download&path=%5B%22%2Ftest%2FITEMA_20445972-0.mp3%22%5D&mode=%22open%22
            string request = OwningClient.ConstructRequest("entry.cgi", "SYNO.FileStation.Download", 2,
                "&method=download" +
                "&path=" + FormatPath(InPath) +
                "&mode=%22open%22");
            
            byte[]? bytes = null;
            var httpResponse = await OwningClient.Request(request);
            if (httpResponse.success && httpResponse.data != null)
                bytes = httpResponse.data.Content.ReadAsByteArrayAsync().Result;

            return new()
            {
                success = httpResponse.success,
                error = httpResponse.error,
                data = bytes
            };
        }
    }
}
