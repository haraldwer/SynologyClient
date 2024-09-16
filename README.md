
This is a .Net implementation of the https API for Synology File Station based on "Synology File Station Official API" available at https://www.synology.com.

Example: 
```cs
using Synology.DataTypes;
using Synology;

// Connecting and logging in
Client client = new("https://nas.quickconnect.to", 5001, TimeSpan.FromSeconds(10));
if (!client.API.Connect())
    return;
if (!client.API.Login("user", "password"))
    return;

// Getting drives
Response<SharedDriveList> sharedDriveResponse = client.FileStation.ListSharedDrives([]);
foreach (ListEntry item in sharedDriveResponse.data.shares)
    Console.WriteLine(item.name);

// Getting files
Response<FileList> listResponse = client.FileStation.List("/Pictures", []);
foreach (ListEntry item in listResponse.data.files)
    Console.WriteLine(item.name);

// Logging out
client.API.Logout();
```

Features:
- Connect, query the API
- Login, Logout
- List files in directory
- List shared drives

Planned: 
- Download file
- Upload file
- Get image thumbnail

"Synology File Station Official API" direct link:
https://global.synologydownload.com/download/Document/Software/DeveloperGuide/Package/FileStation/All/enu/Synology_File_Station_API_Guide.pdf
