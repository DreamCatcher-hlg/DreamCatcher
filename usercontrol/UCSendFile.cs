using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using MyCommon;

namespace MyChat.usercontrol
{
    public partial class UCSendFile : UserControl
    {
        System.Threading.Timer time;
        public event EventHandler RecCancelEvent;
        public event EventHandler<string> FinishedEvent;
        string FileName { get; set; }
        public string FilePath { get; set; }
        TCP tcp = new TCP();
        IPAddress RemoteIp { get; set; }
        public event EventHandler SendEvent;
        private CurrentNum Num = new CurrentNum();
        private Rate Rates = new Rate();
        private int Progress
        {
            get;
            set;
        }
        private UCSendFile()
        {
            InitializeComponent();
            this.label2.Text = "";
            this.label3.Text = "";
        }

        public UCSendFile(string filepath, string filename, IPAddress ip)
            : this()
        {
            FilePath = filepath;
            FileName = filename;
            RemoteIp = ip;
            this.lblFilename.Text = filename;
            this.Load += UCSendFile_Load;
            tcp.ProgressEvent += tcp_ProgressEvent;
            tcp.FinishedEvent += tcp_FinishedEvent;
            tcp.CancelRecEvent += tcp_CancelRecEvent;
            tcp.RateEvent += tcp_RateEvent;
            tcp.currentEvent += tcp_currentEvent;
            this.Disposed += UCSendFile_Disposed;
            //this.HandleDestroyed += UCSendFile_HandleDestroyed;
        }

        void UCSendFile_HandleDestroyed(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        void UCSendFile_Disposed(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Gc();
        }

        void tcp_currentEvent(object sender, CurrentNum e)
        {
            //throw new NotImplementedException();
            Num = e;
        }

        void tcp_RateEvent(object sender, Rate e)
        {
            //throw new NotImplementedException();
            Rates = e;
        }

        void tcp_CancelRecEvent(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (RecCancelEvent != null)
            {
                RecCancelEvent(this, e);
            }
            Gc();
        }

        void tcp_FinishedEvent(object sender, string e)
        {
            //throw new NotImplementedException();
            if (FinishedEvent != null)
            {
                FinishedEvent(this, e);
            }
            Gc();
        }

        void tcp_ProgressEvent(object sender, long e)
        {
            //throw new NotImplementedException();
            //if(!this.IsDisposed&&this.IsHandleCreated)
            //if (!progressBar1.IsDisposed&& this.progressBar1.InvokeRequired)
            //{
            //    this.BeginInvoke(new Action(() =>
            //    {
            //        this.progressBar1.Value =(int) e;
            //    }));
            //} else
            //this.progressBar1.Value = (int)e;
            Progress = (int)e;
        }

        void UCSendFile_Load(object sender, EventArgs e)
        {
            //throw new NotImplementedException();

            Task.Run(() =>
            {
                //tcp.Send(FilePath, FileName, RemoteIp);
                tcp.Send(FilePath, FileName, new IPEndPoint(RemoteIp, Sysconstant.port));
            });
            time = new System.Threading.Timer(new System.Threading.TimerCallback(ShowRate), null, 0, 100);
        }
        private void ShowRate(object o)
        {
            if (!this.IsDisposed && this.IsHandleCreated)
            {
                string total = (((double)Num.currentNum) / 1024 / 1024).ToString("G5") + "MB" + "/" + (((double)Num.totalNum) / 1024 / 1024).ToString("G5") + "MB"; ;
                string rate = "0";
                if (!double.Equals(0.0, Rates.Ticks))
                {
                    rate = (Rates.Num / Rates.Ticks / 1024 / 1024).ToString("G5");
                }
                rate += "MB/S";
                this.BeginInvoke(new Action(() =>
                {
                    this.label3.Text = total;// (((double)Num.currentNum) / 1024 / 1024).ToString("G5") + "MB" + "/" + (((double)Num.totalNum) / 1024 / 1024).ToString("G5") + "MB";
                    //this.label2.Text = (Rates.Num / Rates.Ticks / 1024 / 1024).ToString("G5") + "MB/S";
                    //string rate = "0";
                    //if (!double.Equals(0.0, Rates.Ticks))
                    //{
                    //    rate = (Rates.Num / Rates.Ticks / 1024 / 1024).ToString("G5");
                    //}
                    this.label2.Text = rate;// rate + "MB/S";

                    this.progressBar1.Value = Progress;
                }));
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(FilePath);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (SendEvent != null)
            {
                SendEvent(this, null);
            }
            Gc();
            tcp.IsCancel = true;
            if (!tcp.IsSending)
            {
                Task.Run(() =>
                {
                    tcp.SendCancel();
                });
            }
        }
        private void Gc()
        {
            if (time != null)
            {
                time.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                time.Dispose();
                time = null;
            }
            //if (tcp != null)
            //{
            //    tcp.IsCancel = true;
            //    Task.Run(() =>
            //    {
            //        tcp.SendCancel();
            //    });
            //}
        }
        public void Exit()
        {
            tcp.SendCancel();
        }
        ~UCSendFile()
         {
             Gc();
        }
    }
}
