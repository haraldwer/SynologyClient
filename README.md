
This is a .Net implementation of the https API for Synology File Station based on "Synology File Station Official API" available at https://www.synology.com.

Example: 
```cs
using Synology.DataTypes;
using Synology;
using System.Drawing;

// Connecting and logging in
Client client = new("https://nas.quickconnect.to", 5001, TimeSpan.FromSeconds(10));
if (!client.API.Connect().Result.success)
    return;
if (!client.API.Login("user", "password").Result.success)
    return;

// Getting drives
Response<SharedDriveList> sharedDriveResponse = client.FileStation.ListSharedDrives([]).Result;
Console.WriteLine("Recieved " + sharedDriveResponse.data.shares.Count + " shared drives");

// Getting files
Response<FileList> listResponse = client.FileStation.List("/Pictures", []).Result;
Console.WriteLine("Recieved " + listResponse.data.files.Count + " files");

// Downloading a file
Response<byte[]> downloadResponse = client.FileStation.Download("/Pictures/picture.png").Result;
Console.WriteLine("Downloaded " + downloadResponse.data.Length + " bytes");

// Getting a thumbnail
Response<byte[]> thumbResponse = client.FileStation.Thumbnail("/Pictures/landscape.jpg").Result;
Image image = Image.FromStream(new MemoryStream(thumbResponse.data));
image.Save("C://landscape_thumbnail.jpg");
Console.WriteLine("Thumbnail created!");

// Uploading a file
byte[] bytes = File.ReadAllBytes("C://video.mp4");
Response<HttpResponseMessage> uploadResponse = client.FileStation.Upload("/Pictures", "video.mp4", bytes).Result;
if (uploadResponse.success)
    Console.WriteLine("File uploaded!");

// Logging out
client.API.Logout();
```

Features:
- Connect, query the API
- Login, Logout
- List files in directory
- List shared drives
- Download file
- Get image thumbnail

Planned: 
- Upload file (WIP)

"Synology File Station Official API" direct link:
https://global.synologydownload.com/download/Document/Software/DeveloperGuide/Package/FileStation/All/enu/Synology_File_Station_API_Guide.pdf
