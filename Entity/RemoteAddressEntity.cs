using System.Net;

namespace MyCommon.Entity
{
    public class RemoteAddressEntity
    {
        public IPAddress[] IPs { get; set; }
        //public string MAC { get; set; }
        public string HostName { get; set; }
    }
}
