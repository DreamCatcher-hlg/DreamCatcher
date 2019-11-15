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
using System.Net.Sockets;
using MyCommon;
using MyCommon.Entity;

namespace MyChat.usercontrol
{
    public partial class UCRecFile : UserControl
    {
        System.Threading.Timer time;
        public event EventHandler<string> FinishedEvent;
        public event EventHandler RecCancelEvent;
        public event EventHandler<bool> RecEvent;
        TCP tcp = new TCP();
        TcpClient Client { get; set; }
        string FileName { get; set; }
        long filelen = 0;
        private CurrentNum Num = new CurrentNum();
        private Rate Rates = new Rate();
        public int Progress
        {
            get;
            set;
            //{
                //if (!this.IsDisposed && this.IsHandleCreated)
                //if (!progressBar1.IsDisposed && progressBar1.InvokeRequired)
                //{
                //    this.BeginInvoke(new Action(() =>
                //    {
                //        this.progressBar1.Value = value;
                //    }));
                //}
                //else
                //    this.progressBar1.Value = value;

            //}
        }
        private UCRecFile()
        {
            InitializeComponent();
            this.Load += UCRecFile_Load;
            tcp.CancelRecEvent += tcp_CancelRecEvent;
            tcp.ProgressEvent += tcp_ProgressEvent;
            tcp.FinishedEvent += tcp_FinishedEvent;
            tcp.RateEvent += tcp_RateEvent;
            tcp.currentEvent += tcp_currentEvent;
            this.label2.Text = "";
            this.label3.Text = "";
            //FileHelper.FileRecInstance.RecFileProEvent += FileRecInstance_RecFileProEvent;
            this.Disposed += UCRecFile_Disposed;
        }

        void UCRecFile_Disposed(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Gc();
        }

        void UCRecFile_Load(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
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
                    //this.label3.Text = (((double)Num.currentNum) / 1024 / 1024).ToString("G5") + "MB" + "/" + (((double)Num.totalNum) / 1024 / 1024).ToString("G5") + "MB";
                    //string rate = "0";
                    //if (!double.Equals(0.0, Rates.Ticks))
                    //{
                    //    rate = (Rates.Num / Rates.Ticks / 1024 / 1024).ToString("G5");
                    //}
                    //this.label2.Text = rate + "MB/S";// (Rates.Num / Rates.Ticks / 1024 / 1024).ToString("G5") + "MB/S";
                    this.label3.Text = total;
                    this.label2.Text = rate;
                    this.progressBar1.Value = Progress;
                }));
            }
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
            Progress = (int)e;
        }

        void tcp_CancelRecEvent(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (RecCancelEvent != null)
            {
                RecCancelEvent(this, null);
            }
            Gc();
        }
        public UCRecFile(TcpClient client, long filelength, string filename)
            : this()
        {
            Client = client;
            filelen = filelength;
            FileName = filename;
            this.lblFilename.Text = filename;
            Task.Run(() =>
            {
                tcp.Rec(this.Client, this.FileName, this.filelen);
            });
        }
        
        void FileRecInstance_RecFileProEvent(object sender,ProgressEntity e)
        {
            //throw new NotImplementedException();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Task.Run(() =>
            {
                tcp.SendOK(this.Client);
            });
            this.linkLabel1.Visible = false;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (RecEvent != null)
            {
                RecEvent(this, false);
            }
            Gc();
            tcp.IsCancel = true;
            if (!tcp.IsRecing)
            {
                Task.Run(() =>
                {
                    tcp.SendCancelMsg(this.Client);
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
            //        tcp.SendCancelMsg(this.Client);
            //    });
            //}
        }
        public void Exit()
        {
            tcp.SendCancelMsg(this.Client);
        }
        ~UCRecFile()
        {
            Gc();
        }
    }
}
