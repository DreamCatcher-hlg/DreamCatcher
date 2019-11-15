using MyCommon.Entity;
using MyCommon.helper;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using WindowsFormsApplication17.Entity;

namespace MyCommon
{
    public class Bordcast
    {
        System.Net.Sockets.UdpClient client;
        private int port = 12000;
        System.Threading.Timer time;//= new System.Threading.Timer();
        private Bordcast()
        {
            port = Sysconstant.port;
            //Init();
        }
        private static Bordcast bordcast = new Bordcast();
        public static Bordcast BordCastInstance
        {
            get
            {
                return bordcast;
            }
        }

        public void Init()
        {
            Task.Run(new Action(() =>
            {
                client = new System.Net.Sockets.UdpClient(new IPEndPoint(IPAddress.Any, port));
                time = new System.Threading.Timer(new System.Threading.TimerCallback(SendBordcast), null, 0, 5000);
                Task.Run(() =>
                {
                    Rec();
                });
            }));

        }

        public bool CheckNet()
        {

            if (Sysconstant.LocalIPAddress != null && Sysconstant.LocalIPAddress.Length > 0)
            {
                return true;
            }

            return false;
        }
        private void Bind(UdpClient client, IPEndPoint ip)
        {
            try
            {
                var n = Dns.GetHostName();

                client.Client.Bind(ip);

            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
                //if (ErrorEvent != null)
                //{
                //    ErrorEvent(null, ex.ToString());
                //}
                log.Log.WriteLog(ex.ToString());
            }
        }
        private void SendBordcast(object o)
        {
            try
            {
                UDPMsgEntity entity = Sysconstant.LocalBordcastEntity;
                var bytes = SerializeHelper.Serialize(entity);
                if (!Sysconstant.MutiBordcast)
                {
                    System.Net.IPEndPoint endpoint = new System.Net.IPEndPoint(System.Net.IPAddress.Broadcast, port);
                   
                    var count = client.Send(bytes, bytes.Length, endpoint);
                }
                else
                {
                    foreach (IPAddress ip in Sysconstant.LocalIPAddress)
                    {
                        int index = ip.ToString().LastIndexOf(".");
                        string ip1 = ip.ToString().Substring(0, index);
                        string ip2 = ip1 + ".255";
                        IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ip2), port);
                        var count = client.Send(bytes, bytes.Length, endpoint);
                    }
                }

            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
                //if (ErrorEvent != null)
                //{
                //    ErrorEvent(null, ex.ToString());
                //}
                log.Log.WriteLog(ex.ToString());
            }
        }
        private void Rec()
        {
            try
            {
                System.Net.IPEndPoint remoteendpoint = new System.Net.IPEndPoint(IPAddress.Any, port);
                while (true)
                {
                    var bytes = client.Receive(ref remoteendpoint);
                    Task.Run(() =>
                    {
                        var entity = SerializeHelper.Deserialize(bytes) as UDPMsgEntity;
                        if (entity != null)
                        {
                            var rec = System.Text.Encoding.UTF8.GetString(bytes);
                            Console.WriteLine(string.Join<IPAddress>(",", entity.IPs));
                            var reip = remoteendpoint.Address.ToString();
                            
                            //if (reip != "127.0.0.1")
                            {
                                if (entity.MsgType == 0)
                                {
                                    Sysconstant.AddRemotePoint(remoteendpoint.Address);
                                    log.Log.WriteLog("收到数据包:" + reip);
                                }
                                if (entity.MsgType == 1)
                                {
                                    MyMessage.NewMsg(new Msg() { msg = entity.Msg, MsgType = Definition.MsgType.Txt, IpAddress = remoteendpoint.Address });
                                }
                            }
                        }
                        else
                        {

                        }
                    });
                }
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
                //if (ErrorEvent != null)
                //{
                //    ErrorEvent(null, ex.ToString());
                //}
                log.Log.WriteLog(ex.ToString());
            }
        }
    }
}
