
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

    public class Owner
    {
        public string group = "";
        public string user = "";
        public int uid = 0;
        public int gid = 0;
    }

    public class Time
    {
        public long atime = 0;
        public long crtime = 0;
        public long ctime = 0;
        public long mtime = 0;
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
