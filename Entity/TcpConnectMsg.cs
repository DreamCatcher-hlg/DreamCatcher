using System.Net.Sockets;

namespace MyCommon.Entity
{
    public class TcpConnectMsg : Msg
    {
        public TcpClient Client { get; set; }
        public string FileName { get; set; }
        public int FileLen { get; set; }
    }
}
