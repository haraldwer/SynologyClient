
using Synology.DataTypes;
using Synology.Parameters;
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

        string FormatParameters<T>(T[] InParameters) where T : struct, Enum
        {
            string formatted = "";
            formatted += "%5B"; // {
            bool first = true;
            foreach (var param in InParameters)
            {
                var name = Enum.GetName(typeof(T), param);
                if (name == "")
                    continue;
                if (!first)
                    formatted += "%2C"; // ,
                first = false;
                formatted += "%22" + name + "%22"; // "param"
            }
            formatted += "%5D"; // }
            return formatted;
        }

        /// <summary>
        /// Enumerate files in a given folder.
        /// </summary>
        /// <param name="InFolderPath">A listed folder path starting with a shared folder.</param>
        /// <param name="InAdditionalParameters">Optional. Additional requested file information.</param>
        /// <param name="InOffset">Optional. Specify how many files are skipped before beginning to return listed files.</param>
        /// <param name="InLimit">Optional. Number of files requested. 0 indicates to list all files with a given folder.</param>
        /// <param name="InSortMode">Optional. Specify which file information to sort on.</param>
        /// <param name="InSortDirection">Optional. Specify to sort ascending or to sort descending</param>
        /// <param name="InGlobPattern">Optional. Given glob pattern(s) to find files whose names and extensions match a case-insensitive glob pattern.ou can use "," to separate multiple glob patterns.</param>
        /// <param name="InFileType">Optional. "file": only enumerate regular files; "dir": only enumerate folders; "all" enumerate regular files and folders.</param>
        public async Task<Response<FileList>> List(string InFolderPath, ListAdditionalParameters[] InAdditionalParameters, int InOffset = 0, int InLimit = 0, ListSortMode InSortMode = ListSortMode.name, ListSortDirection InSortDirection = ListSortDirection.asc, string InGlobPattern = "*", ListFileType InFileType = ListFileType.all)
        {
            // Example: /webapi/entry.cgi?api=SYNO.FileStation.List&version=2&method=list&additional=%5B%22real_path%22%2C%22size%22%2C%22owner%22%2C%22time%2Cperm%22%2C%22type%22%5D&folder_path=%22%2Fvideo%22
            string request = OwningClient.ConstructRequest("entry.cgi", "SYNO.FileStation.List", 2,
                "&method=list" +
                "&folder_path=" + FormatPath(InFolderPath) +
                "&offset=" + InOffset.ToString() +
                "&limit=" + InLimit.ToString() +
                "&sort_by=" + nameof(InSortMode) +
                "&sort_direction=" + nameof(InSortDirection) +
                "&pattern=" + InGlobPattern +
                "&filetype=" + nameof(InFileType) +
                "&additional=" + FormatParameters(InAdditionalParameters));

            return await OwningClient.RequestObject<FileList>(request);
        }

        /// <summary>
        /// Get information of file(s).
        /// </summary>
        /// <param name="InFiles">One or more folder/file path(s) starting with a shared folder. </param>
        /// <param name="InAdditionalParameters">Optional. Additional requested file information. </param>
        public async Task<Response<FileList>> Info(List<string> InFiles, ListAdditionalParameters[] InAdditionalParameters)
        {
            // Example: /webapi/entry.cgi?api=SYNO.FileStation.List&version=2&method=getinfo&additional=%5B%22real_path%22%2C%22size%2Cowner%22%2C%22time%2Cperm%2C%22type%22%5D&path=%5B%22%2Fvideo%2F1%22%2C%22%2Fvideo%2F2.txt%22%5D
            string path = "";
            foreach (var file in InFiles)
                path += (path == "" ? "" : ";") + FormatPath(file);
            string request = OwningClient.ConstructRequest("entry.cgi", "SYNO.FileStation.List", 2,
                "&method=getinfo" +
                "&path=" + path +
                "&additional=" + FormatParameters(InAdditionalParameters));

            return await OwningClient.RequestObject<FileList>(request);
        }

        /// <summary>
        /// List all shared folders.
        /// </summary>
        /// <param name="InAdditionalParameters">Optional. Additional requested file information. </param>
        /// <param name="InOffset">Optional. Specify how many files are skipped before beginning to return listed shared folders.</param>
        /// <param name="InLimit">Optional. Number of shared folders requested. 0 lists all shared folders.</param>
        /// <param name="InSortMode">Optional. Specify which file information to sort on.</param>
        /// <param name="InSortDirection">Optional. Specify to sort ascending or to sort descending</param>
        /// <param name="InOnlyWriteable">Optional. true : List writable shared folders. false : List writable and read-only shared folders.</param>
        public async Task<Response<SharedDriveList>> ListSharedDrives(ListShareAdditionalParameters[] InAdditionalParameters, int InOffset = 0, int InLimit = 0, ListSortMode InSortMode = ListSortMode.name, ListSortDirection InSortDirection = ListSortDirection.asc, bool InOnlyWriteable = false)
        {
            // Example: /webapi/entry.cgi?api=SYNO.FileStation.List&version=2&method=list_share&additional=%5B%22real_path%22%2C%22owner%2Ctime%22%5D
            string request = OwningClient.ConstructRequest("entry.cgi", "SYNO.FileStation.List", 2,
                "&method=list_share" +
                "&offset=" + InOffset.ToString() +
                "&limit=" + InLimit.ToString() +
                "&sort_by=" + nameof(InSortMode) +
                "&sort_direction=" + nameof(InSortDirection) +
                "&onlywriteable=" + (InOnlyWriteable ? "true" : "false") + 
                "&additional=" + FormatParameters(InAdditionalParameters));

            return await OwningClient.RequestObject<SharedDriveList>(request);
        }

        /// <summary>
        /// Upload a file by RFC 1867, http://tools.ietf.org/html/rfc1867.
        /// </summary>
        /// <param name="InPath">A destination folder path starting with a shared folder to which files can be uploaded.</param>
        /// <param name="InFileName">A file name. </param>
        /// <param name="InFileContent">The contents of the file. </param>
        /// <param name="InUploadFileExistsBehavior">Optional. overwrite: overwrite the destination file if one exists. skip: skip the upload if the destination file exists. throw_error: respond with an error when the destination file already exists</param>
        /// <param name="InCreateParents">Optional. Create parent folder(s) if none exist.</param>
        public async Task<Response<HttpResponseMessage>> Upload(string InPath, string InFileName, byte[] InFileContent, UploadFileExistBehaviorParameter InUploadFileExistsBehavior = UploadFileExistBehaviorParameter.overwrite, bool InCreateParents = true)
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

            formData.Add(getStringContent("api", "SYNO.FileStation.Upload"));
            formData.Add(getStringContent("version", "3"));
            formData.Add(getStringContent("method", "upload"));
            formData.Add(getStringContent("path", InPath));
            formData.Add(getStringContent("create_parents", InCreateParents ? "true" : "false"));
            if (InUploadFileExistsBehavior != UploadFileExistBehaviorParameter.throw_error)
                formData.Add(getStringContent("overwrite", nameof(InUploadFileExistsBehavior)));

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

        /// <summary>
        /// Download files/folders. <br />
        /// <br />
        /// Notes:<br />
        /// - If only one file is specified, the file content is responded.<br />
        /// - If more than one file/folder is given, binary content in ZIP format which they are compressed to is responded.<br />
        /// </summary>
        /// <param name="InPath">A file or folder path starting with a shared folder to be downloaded. When more than one file is to be downloaded, files/folders will be compressed as a zip file.</param>
        public async Task<Response<byte[]>> Download(string InPath)
        {
            // Example: /webapi/entry.cgi?api=SYNO.FileStation.Download&version=2&method=download&path=%5B%22%2Ftest%2FITEMA_20445972-0.mp3%22%5D&mode=%22open%22
            string request = OwningClient.ConstructRequest("entry.cgi", "SYNO.FileStation.Download", 2,
                "&method=download" +
                "&path=" + FormatPath(InPath) +
                "&mode=%22open%22");

            return await OwningClient.RequestBytes(request);
        }

        /// <summary>
        /// Get a thumbnail of a file.<br />
        /// Note:<br />
        /// 1. Supported image formats: <br />
        /// jpg, jpeg, jpe, bmp, png, tif, tiff, gif, arw, srf, sr2, dcr, k25, <br />
        /// kdc, cr2, crw, nef, mrw, ptx, pef, raf, 3fr, erf, mef, mos, orf, <br />
        /// rw2, dng, x3f, heic, raw.<br />
        /// 2. Supported video formats in an indexed folder: <br />
        /// 3gp, 3g2, asf, dat, divx, dvr-ms, m2t, m2ts, m4v, mkv, mp4, mts, <br />
        /// mov, qt, tp, trp, ts, vob, wmv, xvid, ac3, amr, rm, rmvb, ifo, <br />
        /// mpeg, mpg, mpe, m1v, m2v, mpeg1, mpeg2, mpeg4, ogv, webm, flv, <br />
        /// f4v, avi, swf, vdr, iso, hevc.<br />
        /// 3. Video thumbnails exist only if video files are placed in the "photo" shared folder or users' home folders
        /// </summary>
        /// <param name="InPath">A file path starting with a shared folder.</param>
        /// <param name="InSize">Optional. Return different size thumbnail.</param>
        /// <param name="InRotation">Optional. Return rotated thumbnail.</param>
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
