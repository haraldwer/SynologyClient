
namespace Synology.DataTypes
{
    public class FileAdditional
    {
        public string real_path = "";
        public int size = 0;
        public string type = "";
        public Owner? owner;
        public Time? time;
    }

    public class File
    {
        public FileAdditional additional = new();
        public bool isdir = false;
        public string name = "";
        public string path = "";
    }

    public class FileList
    {
        public List<File> files = new();
        int total = 0;
        int offset = 0;
    }
}
