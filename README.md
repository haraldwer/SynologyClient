
This is a .Net implementation of the https API for Synology File Station based on "Synology File Station Official API" available at https://www.synology.com.

Example: 
```cs
SynologyAPI.Client client = new("https://nas.quickconnect.to", 5001, TimeSpan.FromSeconds(10));
if (!client.Connect())
    return;
if (!client.Login("user", "password"))
    return;
var files = client.List("/Pictures", []);
foreach (var filePath in files)
    Console.WriteLine(filePath);
client.Logout();
```

Features:
	- Connect, query the API
	- Login, Logout
	- List files in directory

Planned: 
	- List shared drives
	- Download file
	- Upload file
	- Get image thumbnail

"Synology File Station Official API" direct link:
https://global.synologydownload.com/download/Document/Software/DeveloperGuide/Package/FileStation/All/enu/Synology_File_Station_API_Guide.pdf
