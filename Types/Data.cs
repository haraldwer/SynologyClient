
namespace Synology.DataTypes
{
    public class APIInfo
    {
        public string path = "";
        public int minVersion = 0;
        public int maxVersion = 0;
    }

    public class Login
    {
        public string sid = ""; 
    }

    public class ListEntryOwner
    {
        public string group;
        public string user;
    }

    public class ListEntryTime
    {
        public long atime = 0;
        public long crtime = 0;
        public long ctime = 0;
        public long mtime = 0;
    }

    public class ListEntryAdditional
    {
        public string real_path = "";
        public int size = 0;
        public string type = "";
        public ListEntryOwner owner;
        public ListEntryTime time;
    }

    public class ListEntry
    {
        public ListEntryAdditional additional = new();
        public bool isdir = false;
        public string name = "";
        public string path = "";
    }

    public class FileList
    {
        public List<ListEntry> files = new();
        int total = 0;
        int offset = 0;
    }

    public class SharedDriveList
    {
        public List<ListEntry> shares = new();
        int total = 0;
        int offset = 0;
    }

    public enum ThumbnailSize
    {
        small,
        medium,
        large,
        original
    }

    public enum ThumbnailRotation
    {
        None = 0,
        Rotate_90,
        Rotate_180,
        Rotate_270,
        Rotate_360
    }
}
