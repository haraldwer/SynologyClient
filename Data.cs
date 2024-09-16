
namespace SynologyAPI.Data
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

    public class ListEntryAdditional
    {
        public string real_path = "";
        public int size = 0;
        public string type = "";
    }

    public class ListEntry
    {
        public ListEntryAdditional additional = new();
        public bool isdir = false;
        public string name = "";
        public string path = "";
    }

    public class List
    {
        public List<ListEntry> files = new();
    }
}
