
namespace Synology
{
    public class ErrorInfo
    {
        public int code = 0;
        public string path = "";
    }

    public class Error
    {
        public int code = 0;
        public List<ErrorInfo> errors = new();
    }

    public class Response<T>
    {
        public bool success = false;
        public Error? error = null;
        public T? data = default;
    }
}
