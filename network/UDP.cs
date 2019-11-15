using MyCommon.helper;
using System;
using System.Net;
using WindowsFormsApplication17.Entity;

namespace MyCommon
{
    public class UDP
    {
        public event EventHandler<string> ErrorEvent;
        private System.Net.Sockets.UdpClient client = new System.Net.Sockets.UdpClient();
        private int port = 12000;
        public UDP()
        {
            port = Sysconstant.port;
            //Init();
        }
        public bool Send(string str, IPAddress Remoteendpont)
        {
            bool boo = false;
            try
            {
                UDPMsgEntity entity = Sysconstant.LocalBordcastEntity;
                entity.MsgType = 1;
                entity.Msg = str;
                var bytes = SerializeHelper.Serialize(entity);
                //var bytes = System.Text.Encoding.UTF8.GetBytes("msg:" + str);
                if (Remoteendpont != null)
                {
                    //System.Net.IPEndPoint endpoint = Remoteendpont;
                    client.Send(bytes, bytes.Length, new IPEndPoint(Remoteendpont, Sysconstant.port));// endpoint);
                    boo = true;
                }
                else
                {
                    //System.Windows.Forms.MessageBox.Show("没有可发送对象");
                }
            }
            catch(Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
                if (ErrorEvent != null)
                {
                    ErrorEvent(null, ex.ToString());
                }
            }
            return boo;
        }

    }
}
