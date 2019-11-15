using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MyCommon
{
    public class TcpConnect
    {
        string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "FileRec");
        int port = 12000;
        
        readonly object obj = new object();
        private TcpConnect()
        {
            port = Sysconstant.port;
            //Init();
        }
        private static TcpConnect fileRec = new TcpConnect();
        public static TcpConnect FileRecInstance
        {
            get
            {
                return fileRec;
            }
        }
        public void Init()
        {
            foreach (IPAddress ip in Sysconstant.LocalIPAddress)
            {
                Task.Run(() =>
                {

                    TcpListener listener = new TcpListener(ip, port);
                    listener.Start(1000);
                    while (true)
                    {
                        try
                        {
                            var socket = listener.AcceptTcpClient();

                            Rec(socket);

                        }
                        catch (Exception ex)
                        {
                            //if (ErrorEvent != null)
                            //    ErrorEvent(null, ex.ToString());
                            log.Log.WriteLog(ex.ToString());
                        }
                    }
                });
            }
        } 
      
        private void Rec(TcpClient client)
        {
            var endpoint = (IPEndPoint)client.Client.RemoteEndPoint;
            //MyMessage.NewMsg(new Entity.TcpConnectMsg() { Client = client, msg = "", IpPoint = endpoint, MsgType = Definition.MsgType.File, State = 0 });
            //return;
            Task.Run(() =>
            {
                try
                {
                    var stream = client.GetStream();
                    {


                        byte[] buff = new byte[1024];
                        {
                            int cou = 0;

                            cou = stream.Read(buff, 0, buff.Length);

                            int filenamele = BitConverter.ToInt32(buff, 0);
                            int filelen = BitConverter.ToInt32(buff, 4);
                            var filename = System.Text.Encoding.UTF8.GetString(buff, 8, filenamele);
                            MyMessage.NewMsg(new Entity.TcpConnectMsg() { Client = client, FileName = filename, FileLen = filelen, msg = "", IpAddress = endpoint.Address, MsgType = Definition.MsgType.File, State = 0 });
                            return;
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
                    MyMessage.NewMsg(new Entity.Msg() { msg = ex.ToString(), IpAddress = endpoint.Address, MsgType = Definition.MsgType.Error });
                }
            });
        }
    }
}
