
namespace Synology.DataTypes
{
    public class SharedFolderACL
    {
        public bool append = false;
        public bool del = false;
        public bool exec = false;
        public bool read = false;
        public bool write = false;
    }

    public class SharedFolderPermissions
    {
        public bool disable_download = false;
        public bool disable_list = false;
        public bool disable_modify = false;
    }

    public class DrivePermissions
    {
        public string share_right = "";
        public int posix = 0;
        public SharedFolderPermissions? adv_right;
        public bool acl_enable = false;
        public bool is_acl_mode = false;
        public SharedFolderACL? acl;
    }

    public class VolumeStatus
    {
        public int freespace = 0;
        public int totalspace = 0;
        public bool @readonly = false;
    }

    public class DriveAdditional
    {
        public string real_path = "";
        public Owner? owner;
        public Time? time;
        public DrivePermissions? perm;
        public string mount_point_type = "";
        public VolumeStatus? volume_status;
    }

    public class Drive
    {
        public string path = "";
        public string name = "";
        public FileAdditional additional = new();
    }

    public class SharedDriveList
    {
        int total = 0;
        int offset = 0;
        public List<File> shares = new();
    }
}
