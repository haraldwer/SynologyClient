
using Synology.DataTypes;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

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

            string boundary = "AaB03x";
            using var formData = new MultipartFormDataContent(boundary);
            formData.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
            formData.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("boundary", boundary));

            Func<string, string, StringContent> getStringContent = (string name, string value) => 
            {
                var sc = new StringContent(value);
                sc.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = $"\"{name}\""
                };
                sc.Headers.ContentType = null;
                return sc;
            };

            var overwriteValue = InOverwrite ? "true" : "false";
            if (true) //(apiInfo.Version >= 3) 
                overwriteValue = InOverwrite ? "overwrite" : "skip"; // 3rd option: Ask

            formData.Add(getStringContent("api", "SYNO.FileStation.Upload"));
            formData.Add(getStringContent("version", "3"));
            formData.Add(getStringContent("method", "upload"));
            formData.Add(getStringContent("path", InPath));
            formData.Add(getStringContent("overwrite", overwriteValue));
            formData.Add(getStringContent("create_parents", InCreateParents ? "true" : "false"));

            using var fileContent = new ByteArrayContent(InFileContent);
            var urlEncodedFilename = Uri.EscapeDataString(InFileName);
            var headerValue = $@"form-data; name=""file""; filename=""{InFileName}""; filename*=UTF-8''{urlEncodedFilename}";
            var bytes = Encoding.UTF8.GetBytes(headerValue);
            headerValue = bytes.Aggregate("", (current, b) => current + (char)b);
            fileContent.Headers.Add("Content-Disposition", headerValue);
            formData.Add(fileContent);

            string request = OwningClient.ConstructRequest("entry.cgi", "SYNO.FileStation.Upload", 2, "&method=upload");
            return await OwningClient.RequestHttp("/webapi/entry.cgi", formData);
        }

        public async Task<Response<byte[]>> Download(string InPath)
        {
            // Example: /webapi/entry.cgi?api=SYNO.FileStation.Download&version=2&method=download&path=%5B%22%2Ftest%2FITEMA_20445972-0.mp3%22%5D&mode=%22open%22
            string request = OwningClient.ConstructRequest("entry.cgi", "SYNO.FileStation.Download", 2,
                "&method=download" +
                "&path=" + FormatPath(InPath) +
                "&mode=%22open%22");

            return await OwningClient.RequestBytes(request);
        }

        public async Task<Response<byte[]>> Thumbnail(string InPath, ThumbnailSize InSize = ThumbnailSize.small, ThumbnailRotation InRotation = ThumbnailRotation.None)
        {
            // Example: /webapi/entry.cgi?api=SYNO.FileStation.Thumb&version=2&method=get&path=%22%2Fphoto%2Ftest.jpg%22

            string request = OwningClient.ConstructRequest("entry.cgi", "SYNO.FileStation.Thumb", 2,
                "&method=get" +
                "&path=" + FormatPath(InPath) +
                "&size=" + InSize.ToString() + 
                "&rotate=" + (int)InRotation);

            return await OwningClient.RequestBytes(request);
        }
    }
}
