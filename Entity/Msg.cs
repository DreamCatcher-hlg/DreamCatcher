using System.Net;

namespace MyCommon.Entity
{
    public class Msg
    {
        public IPAddress IpAddress { get; set; }
        public Definition.MsgType MsgType{get;set;}
        public string msg { get; set; }
        public int State { get; set; }
    }
}
