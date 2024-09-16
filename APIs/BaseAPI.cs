
namespace Synology
{
    public class BaseAPI
    {
        protected Client OwningClient;

        internal BaseAPI(Client InClient)
        {
            OwningClient = InClient;
        }
    }
}
