using System;
using System.Net;

namespace WindowsFormsApplication17.Entity
{
    [Serializable]
    public class UDPMsgEntity
    {
        public IPAddress[] IPs { get; set; }
        //public string MAC { get; set; }
        public string HostName { get; set; }
        /// <summary>
        /// 0：广播 1：普通消息
        /// </summary>
        public int MsgType { get; set; }
        public string Msg { get; set; }
    }
}
