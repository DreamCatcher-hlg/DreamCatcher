using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyCommon.log
{
    public class Log
    {
        readonly static object obj = new object();
        readonly static string path = System.Windows.Forms.Application.StartupPath;

        static Log()
        {
            System.Threading.Timer timer = new System.Threading.Timer(new System.Threading.TimerCallback(Deleteby3daysago), null, 200, 8 * 60 * 60 * 1000);

            Task.Run(() =>
            {
                while (true)
                {
                    string msg = "";
                    msgqueue.TryDequeue(out msg);

                    if (!string.IsNullOrEmpty(msg))
                    {
                        Log.Write(msg);
                    }
                    System.Threading.Thread.Sleep(50);
                }
            });
        }

        /// <summary>
        /// 日誌隊列
        /// </summary>
        static System.Collections.Concurrent.ConcurrentQueue<string> msgqueue = new System.Collections.Concurrent.ConcurrentQueue<string>();

        /// <summary>
        /// 寫日誌
        /// </summary>
        /// <param name="msg"></param>
        public static void WriteLog(string msg)
        {
            try
            {
                msgqueue.Enqueue(msg);
                return;
            }
            catch (Exception ex)
            {
                //log.Log.WriteLog(ex.ToString());
                WriteSystemError(ex.ToString());
            }
        }
        public static void WriteSystemError(string msg)
        {
            try
            {
                //lock (obj)
                {
                    //string path = System.Windows.Forms.Application.StartupPath;// System.IO.Directory.GetCurrentDirectory();
                    string logpath = path + "\\Log\\SystemError\\";
                    if (!System.IO.Directory.Exists(logpath))
                    {
                        System.IO.Directory.CreateDirectory(logpath);
                    }
                    DateTime dt = DateTime.Now;
                    //Task.Run(() => { Deleteby3daysago(dt, logpath); });
                    string name = dt.ToString("yyyy-MM-dd_HH") + ".Log";
                    string filename = logpath + name;// "\\Scpe.txt";
                    using (System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite))
                    {
                        fs.Seek(0, System.IO.SeekOrigin.End);
                        byte[] bytes = System.Text.Encoding.Default.GetBytes(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        fs.Write(bytes, 0, bytes.Length);
                        bytes = System.Text.Encoding.Default.GetBytes("\r\n");
                        fs.Write(bytes, 0, bytes.Length);

                        bytes = System.Text.Encoding.Default.GetBytes(msg);
                        fs.Write(bytes, 0, bytes.Length);
                        bytes = System.Text.Encoding.Default.GetBytes("\r\n");
                        fs.Write(bytes, 0, bytes.Length);

                        fs.Flush();
                        fs.Close();
                        fs.Dispose();
                    }
                }
            }
            catch
            {

            }
        }
        private static void Write(string msg)
        {
            try
            {
                //lock (obj)
                {
                    //string path = System.Windows.Forms.Application.StartupPath;// System.IO.Directory.GetCurrentDirectory();
                    string logpath = path + "\\Log\\Log\\";
                    if (!System.IO.Directory.Exists(logpath))
                    {
                        System.IO.Directory.CreateDirectory(logpath);
                    }
                    DateTime dt = DateTime.Now;
                    //Task.Run(() => { Deleteby3daysago(dt, logpath); });
                    string name = dt.ToString("yyyy-MM-dd_HH") + ".Log";
                    string filename = logpath + name;// "\\Scpe.txt";
                    using (System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite))
                    {
                        fs.Seek(0, System.IO.SeekOrigin.End);
                        byte[] bytes = System.Text.Encoding.Default.GetBytes(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        fs.Write(bytes, 0, bytes.Length);
                        bytes = System.Text.Encoding.Default.GetBytes("\r\n");
                        fs.Write(bytes, 0, bytes.Length);

                        bytes = System.Text.Encoding.Default.GetBytes(msg);
                        fs.Write(bytes, 0, bytes.Length);
                        bytes = System.Text.Encoding.Default.GetBytes("\r\n");
                        fs.Write(bytes, 0, bytes.Length);

                        fs.Flush();
                        fs.Close();
                        fs.Dispose();
                    }
                }
            }
            catch
            {

            }
        }
        private static void Deleteby3daysago(object o)
        {
            try
            {
                //string path = System.IO.Directory.GetCurrentDirectory();
                string logpath = path + "\\Log\\Log\\";
                if (!System.IO.Directory.Exists(logpath))
                {
                    System.IO.Directory.CreateDirectory(logpath);
                }
                DateTime dt = DateTime.Now;
                System.IO.DirectoryInfo directoryinfo = new System.IO.DirectoryInfo(logpath);
                System.IO.FileInfo[] files = directoryinfo.GetFiles();
                foreach (var v in files.ToArray())
                {
                    string[] oldstr = v.Name.Split('_');
                    DateTime olddt = dt.AddDays(-3);//.ToString("yyyy-MM-dd");
                    if (oldstr.Length == 2)
                    {
                        DateTime old;
                        try
                        {
                            old = Convert.ToDateTime(oldstr[0]);
                        }
                        catch (Exception ex)
                        {
                            log.Log.WriteLog(ex.ToString());
                            old = DateTime.Now;
                        }

                        //if (v.Name.Contains(olddt))
                        if (olddt > old)
                        {
                            v.Delete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Log.WriteLog(ex.ToString());
            }
        }
    }
}
