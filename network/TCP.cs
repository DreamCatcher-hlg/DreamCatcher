using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MyCommon
{
    public struct Rate
    {
        public long Num { get; set; }
        public double Ticks { get; set; }
    }
    public struct CurrentNum
    {
        public long currentNum { get; set; }
        public long totalNum { get; set; }
    }
    public class TCP
    {
        public event EventHandler<string> FinishedEvent;
        public event EventHandler CancelRecEvent;
        public event EventHandler<long> ProgressEvent;
        public event EventHandler<Rate> RateEvent;
        public event EventHandler<CurrentNum> currentEvent;
        static string path = System.AppDomain.CurrentDomain.BaseDirectory;
        int port = 12000;
        readonly object obj = new object();
        public bool IsCancel { get; set; }
        private TcpClient Client { get; set; }
        public bool IsSending = false;
        public bool IsRecing = false;
        public TCP()
        {
            port = Sysconstant.port;
            //FileHelper.FileRecInstance.RecfileEvent += FileRecInstance_RecfileEvent;
        }
        void FileRecInstance_RecfileEvent(object sender, Entity.RecFileEventEntity e)
        {
            //throw new NotImplementedException();
        }
        public void RecStartMsg(TcpClient client)
        {
            try
            {
                var stream = client.GetStream();
                {
                    byte[] buff = new byte[1024];
                    var cou = stream.Read(buff, 0, buff.Length);
                    var res = System.Text.Encoding.UTF8.GetString(buff, 0, cou);
                    if (res.StartsWith("cancel"))
                    {
                        if (CancelRecEvent != null)
                        {
                            CancelRecEvent(this, null);
                        }
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Log.WriteLog(ex.ToString());
            }
        }
        public bool SendOK(TcpClient client)
        {
            bool res = false;
            try
            {
                var stream = client.GetStream();
                byte[] buff = new byte[1024];
                var bys = System.Text.Encoding.UTF8.GetBytes("OK");
                stream.Write(bys, 0, bys.Length);
                res = true;
                IsRecing = true;
            }
            catch (Exception ex)
            {
                log.Log.WriteLog(ex.ToString());
            }
            return res;
        }
        public void Rec(TcpClient client,string filename,long filelen)
        {
            try
            {
                using(var stream = client.GetStream())
                {
                    byte[] buff = new byte[1024];
                    long reccount = 0;
                    var filep = System.IO.Path.Combine(path, "FileRec");
                    var filepath = System.IO.Path.Combine(filep, filename);
                    if (!System.IO.Directory.Exists(filep))
                    {
                        System.IO.Directory.CreateDirectory(filep);
                    }
                    using (System.IO.FileStream writer = new System.IO.FileStream(filepath, System.IO.FileMode.Create,FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                        while (reccount < filelen && !IsCancel)
                        {
                            watch.Restart();
                            int num = stream.Read(buff, 0, buff.Length);
                            if (num <= 0)
                            {
                                break;
                            }
                            else
                            {
                                IsRecing = true;
                            }
                            if (System.Text.Encoding.UTF8.GetString(buff).StartsWith("cancel"))
                            {
                                if (CancelRecEvent != null)
                                {
                                    CancelRecEvent(this, null);
                                }
                                return;
                                //break;
                            }
                            reccount += num;
                            writer.Write(buff, 0, num);
                            watch.Stop();
                            if (RateEvent != null)
                            {
                                //double rate = num / watch.Elapsed.TotalSeconds / 1000;
                                RateEvent(this, new Rate() { Num = num, Ticks = watch.Elapsed.TotalSeconds });
                            }
                            if (currentEvent != null)
                            {
                                currentEvent(this, new CurrentNum() { currentNum = reccount, totalNum = filelen });
                            }
                            if (ProgressEvent != null)
                            {
                                long pro = reccount;
                                ProgressEvent(this, (pro * 100) / filelen);
                                //Console.WriteLine(pro + " " + reccount + " " + filelen);
                            }
                        }
                    }
                    if (IsCancel)
                    {
                        var cancelstr = System.Text.Encoding.UTF8.GetBytes("cancel");
                        stream.Write(cancelstr, 0, cancelstr.Length);
                        return;
                    }
                    else
                    if (FinishedEvent != null)
                    {
                        FinishedEvent(this, filepath);
                    }

                }
            }
            catch (Exception ex)
            {
                log.Log.WriteLog(ex.ToString());
            }
        }
        public void SendCancelMsg(TcpClient client)
        {
            try
            {
                if (client.Connected)
                {
                    using(var stream = client.GetStream())
                    {
                        //byte[] bytes = new byte[1024];
                        var bys = System.Text.Encoding.UTF8.GetBytes("cancel");
                        stream.Write(bys, 0, bys.Length);
                    }
                }

            }
            catch (Exception ex)
            {
                log.Log.WriteLog(ex.ToString());
            }
        }
        public void SendCancel()
        {
            if (this.Client != null)
            {
                try
                {
                    if (Client.Connected)
                    {
                        var stream = Client.GetStream();
                        {
                            var bytes = System.Text.Encoding.UTF8.GetBytes("cancel");
                            stream.Write(bytes, 0, bytes.Length);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Log.WriteLog(ex.ToString());
                }
            }
        }
        public bool Send(string filepath, string safefilename, IPEndPoint remoteendpoint)
        {
            bool IsOk = false;
            TcpClient client = new TcpClient();
            if (!client.Connected)
            {
                Connect(client, remoteendpoint);
            }
            if (!client.Connected)
            {
                return IsOk;
            }
            try
            {

                var streamr = client.GetStream();
                {
                    Client = client;
                    byte[] buff = new byte[1024];
                    var bytes = Getbytes(filepath, safefilename);
                    int num = bytes.Length / buff.Length;
                    while (num >= 0)
                    {
                        var buff1 = bytes.Take(buff.Length).ToArray();
                        bytes.CopyTo(buff1, 0);
                        streamr.Write(buff1, 0, buff1.Length);
                        num--;
                    }
                    int readcou = streamr.Read(buff, 0, buff.Length);
                    if (readcou <= 0)
                    {
                        streamr.Close();
                        client.Close();
                        return IsOk;
                    }
                    
                    var ack=System.Text.Encoding.UTF8.GetString(buff);
                    if (ack.Contains("OK"))
                    {
                        IsSending = true;
                    }
                    else if (ack.Contains("cancel"))
                    {
                        if (CancelRecEvent != null)
                        {
                            CancelRecEvent(this, null);
                        }
                        return false;
                    }
                }
                Task.Run(() =>
                {
                    try
                    {
                        var stream1 = client.GetStream();
                        {
                            var recbys = new byte[1024];
                            var readc=stream1.Read(recbys, 0, recbys.Length);
                            if (readc <= 0)
                            {
                                return;
                            }
                            var rec = System.Text.Encoding.UTF8.GetString(recbys);
                            if (rec.StartsWith("cancel"))
                            {
                                if (CancelRecEvent != null)
                                {
                                    CancelRecEvent(this, null);
                                }
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Log.WriteLog(ex.ToString());
                    }
                });
                using(var stream = client.GetStream())
                {
                    byte[] buff = new byte[1024];
                    long sendCount = 0;
                    using (FileStream reader = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    {
                        var filele = reader.Length;
                        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                        while (sendCount < filele && !IsCancel )
                        {
                            watch.Restart();
                            int readcount = reader.Read(buff, 0, buff.Length);
                            if (readcount <= 0)
                            {
                                break;
                            }
                            if (!client.Connected)
                            {
                                break;
                            }
                            stream.Write(buff, 0, readcount);
                            sendCount += readcount;
                            watch.Stop();
                            if (RateEvent != null)
                            {
                                //double rate = readcount / watch.Elapsed.TotalSeconds / 1000;
                                RateEvent(this, new Rate() { Num = readcount, Ticks = watch.Elapsed.TotalSeconds });
                            }
                            if (currentEvent != null)
                            {
                                currentEvent(this, new CurrentNum() { currentNum = sendCount, totalNum = filele });
                            }
                            if (ProgressEvent != null)
                            {
                                long pro = (sendCount * 100) / filele;
                                ProgressEvent(this, pro);
                               // Console.WriteLine("send:" + pro + " " + sendCount + " " + filele);
                            }
                        }
                    }
                    if (IsCancel)
                    {
                        var cancelstr = System.Text.Encoding.UTF8.GetBytes("cancel");
                        stream.Write(cancelstr, 0, cancelstr.Length);
                    }
                    else
                    if (FinishedEvent != null)
                    {
                        FinishedEvent(this, filepath);
                    }
                    IsOk = true;
                }
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
                log.Log.WriteLog(ex.ToString());
            }


            return IsOk;
        }
        private byte[] Getbytes(string filepath, string safefilename)
        {
            try
            {
                using (FileStream reader = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    var name = System.Text.Encoding.UTF8.GetBytes(safefilename);
                    var namelength = BitConverter.GetBytes(name.Length);
                    var filelength = BitConverter.GetBytes(reader.Length);
                    var filenamebuff = new byte[8 + name.Length];
                    namelength.CopyTo(filenamebuff, 0);
                    filelength.CopyTo(filenamebuff, 4);
                    name.CopyTo(filenamebuff, 8);
                    return filenamebuff;
                }
            }catch(Exception ex)
            {
                log.Log.WriteLog(ex.ToString());
                throw ex;
            }
            return new byte[0];
        }
        private byte[] GetFileBytes(string filepath)
        {
            try
            {
                using (FileStream reader = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                {
                    var filebuff = new byte[reader.Length];
                    reader.Read(filebuff, 0, filebuff.Length);
                    return filebuff;
                }
            }
            catch (Exception ex)
            {
                log.Log.WriteLog(ex.ToString());
                throw ex;
            }
            return new byte[0];
        }
        public long GetFileLen(string filepath)
        {
            try
            {
                using (FileStream reader = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    //var filebuff = new byte[reader.Length];
                    //reader.Read(filebuff, 0, filebuff.Length);
                    return reader.Length;
                }
            }
            catch (Exception ex)
            {
                log.Log.WriteLog(ex.ToString());
                throw ex;
            }
            return 0;
        }
        private void Connect(TcpClient client, IPEndPoint remotepoint)
        {
            try
            {
                client.Connect(remotepoint);
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString());
                log.Log.WriteLog(ex.ToString());
            }
        }
    }
}
