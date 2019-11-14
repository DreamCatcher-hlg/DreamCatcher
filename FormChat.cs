using MyCommon;
using MyCommon.Entity;
using System;
using System.Drawing;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyChat.usercontrol;

namespace MyChat
{
    public partial class FromChat : Form
    {
        private readonly object startPositionobj = new object();
        private int LineSpace = 300;
        UDP udp = new UDP();
        //TCP tcp = new TCP();
        public IPAddress Remoteip { get; set; }
        private FromChat()
        {
            InitializeComponent();
            udp.ErrorEvent += udp_ErrorEvent;
            //tcp.ErrorEvent += tcp_ErrorEvent;
            this.rtbSend.DragDrop += rtbSend_DragDrop;
            this.rtbSend.DragEnter += rtbSend_DragEnter;
            this.rtbView.TextChanged += rtbView_TextChanged;
            this.Load+=Form1_Load;
            this.FormClosing += Form1_FormClosing;
            this.FormClosed += Form1_FormClosed;
            this.DragEnter += Form1_DragEnter;
            this.DragDrop += Form1_DragDrop;
            this.rtbSend.PreviewKeyDown += rtbSend_PreviewKeyDown;
            this.rtbView.LinkClicked += rtbView_LinkClicked;
            this.rtbView.SelectionChanged += rtbView_SelectionChanged;
            this.rtbView.MouseDown += rtbView_MouseDown;
            //FormHelper.HideCaret(this.rtbView.Handle);
        }

        void rtbView_MouseDown(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
            FormHelper.HideCaret(this.rtbView.Handle);
        }

        void rtbView_SelectionChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        void rtbView_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            //throw new NotImplementedException();
            System.Diagnostics.Process.Start(e.LinkText.Substring(e.LinkText.IndexOf('#') + 1));
        }

        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void rtbSend_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.Control && e.KeyCode == Keys.V)
            {
                var data = Clipboard.GetDataObject();
                if (Clipboard.ContainsData(DataFormats.FileDrop))
                {
                    var datas = Clipboard.GetData(DataFormats.FileDrop);
                    var paths = ((System.Array)data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
                    Clipboard.Clear();
                }
                else if (Clipboard.ContainsImage())
                {
                    var datas = Clipboard.GetData(DataFormats.Bitmap);
                    Bitmap map = datas as Bitmap;
                    Clipboard.Clear();
                }
            }
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException();
            var path=((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            var paths = path.Split('\\');
            string filename = paths[paths.Length - 1];
            SendFile(path, filename);
        }

        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException();
            e.Effect = DragDropEffects.Copy;
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.Cancel)
            {
                e.Cancel = false;
            }
            FrmCloseState frm = new FrmCloseState(this);
            frm.Show();
            if (this.panel1.Controls.Count > 0)
            {
                
                int times = 100;
                var act = new Action(() =>
                {
                    try
                    {
                        foreach (Control v in this.panel1.Controls)
                        {
                            if (v is UCSendFile)
                            {
                                (v as UCSendFile).Exit();
                            }
                            else if (v is UCRecFile)
                            {
                                (v as UCRecFile).Exit();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MyCommon.log.Log.WriteLog(ex.ToString());
                    }
                    while (--times >= 0)
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                }).BeginInvoke(null, null);
                while (!act.IsCompleted)
                {
                    Application.DoEvents();
                }
            }
            frm.Close();
            this.Gc();
        }
        public FromChat(IPAddress point):this()
        {
            Remoteip = point;
        }
        void rtbView_TextChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            this.rtbView.ScrollToCaret();
        }
        void Form1_Load(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            this.Text = "正在与" + Remoteip + "通话";
            MyMessage.MsgEvent += Message_MsgEvent;
            this.rtbSend.Select();
            //FormHelper.SetLineSpace(this.rtbView, LineSpace);
        }

        void Message_MsgEvent(object sender, MyCommon.Entity.Msg e)
        {
            //throw new NotImplementedException();
            if (e.IpAddress.ToString() == this.Remoteip.ToString())
            {
                if (e.MsgType == Definition.MsgType.File)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        AddRecControl(e);
                    }));
                }
                else if(e.MsgType==Definition.MsgType.Txt)
                ShowRec(e);
                else if (e.MsgType == Definition.MsgType.Error)
                {
                    ShowError(e.msg);
                }
            }
        }

        void ucrec_FinishedEvent(object sender, string e)
        {
            //throw new NotImplementedException();
            this.BeginInvoke(new Action(() =>
            {
                this.panel1.Controls.Remove(sender as UCRecFile);

                ShowFinished(e, Color.Blue, Definition.ShowMsgType.Receive);
            }));
        }

        void ucrec_RecCancelEvent(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (!this.IsDisposed && this.IsHandleCreated)
            {
                this.BeginInvoke(new Action(() =>
                {
                    this.panel1.Controls.Remove(sender as UCRecFile);
                }));
                ShowError("对方取消操作");
            }
        }

        void tcp_ErrorEvent(object sender, string e)
        {
            //throw new NotImplementedException();
            ShowError(e);
        }

        void udp_ErrorEvent(object sender, string e)
        {
            //throw new NotImplementedException();
            ShowError(e);
        }

        void rtbSend_DragEnter(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException();
            e.Effect = DragDropEffects.Link;
        }

        void rtbSend_DragDrop(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException();
            //var path=((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            //this.rtbView.LoadFile(path, RichTextBoxStreamType.RichText);
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.rtbSend.Text))
                {
                    return;
                }
                var sendstr = this.rtbSend.Text;
                var res = await Task.Run(() => { return udp.Send(sendstr, Remoteip); });
                ShowSendmsg(sendstr, res);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this.rtbSend.Clear();
                this.rtbSend.Select();
            }
        }

        private void SendFile(string filepath,string safefilename)
        {
            if (System.IO.File.Exists(filepath))
            {

            }
            else
            {
                //GZipCompress.Compress(new DirectoryInfo(filepath));
                //GZipCompress.Compress(filepath, filepath);
                //GZipCompress.CompressEx(new DirectoryInfo(filepath));
                //GZipCompress.DeCompress(null);
                return;
            }
            foreach (Control v in this.panel1.Controls)
            {
                if (v is UCSendFile)
                {
                    var send = v as UCSendFile;
                    if (filepath == send.FilePath)
                    {
                        return;
                    }
                }
            }
            var ucsend = new UCSendFile(filepath, safefilename, Remoteip);
            this.panel1.Controls.Add(ucsend);
            ucsend.Dock = DockStyle.Top;
            ucsend.SendEvent += ucsend_SendEvent;
            ucsend.FinishedEvent += ucsend_FinishedEvent;
            ucsend.RecCancelEvent += ucsend_RecCancelEvent;
        }

        void ucsend_RecCancelEvent(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (!this.IsDisposed && this.IsHandleCreated)
            {
                this.BeginInvoke(new Action(() =>
                {
                    this.panel1.Controls.Remove(sender as UCSendFile);
                }));
                ShowError("对方取消操作");
            }
        }

        void ucsend_FinishedEvent(object sender, string e)
        {
            //throw new NotImplementedException();
            this.BeginInvoke(new Action(() =>
            {
                this.panel1.Controls.Remove(sender as UCSendFile);

                ShowFinished(e, Color.Blue,Definition.ShowMsgType.Send);
            }));
        }

        void ucsend_SendEvent(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            this.panel1.Controls.Remove(sender as UCSendFile);
        }
        private void AddRecControl(Msg e)
        {
            TcpConnectMsg ms = e as TcpConnectMsg;
            UCRecFile ucrec = new UCRecFile(ms.Client, ms.FileLen, ms.FileName);
            ucrec.RecCancelEvent += ucrec_RecCancelEvent;
            ucrec.FinishedEvent += ucrec_FinishedEvent;
            this.panel1.Controls.Add(ucrec);
            ucrec.Dock = DockStyle.Top;
            ucrec.RecEvent += ucrec_RecEvent;
        }
        private void ShowSendmsg(string msg, bool boo)
        {
            string dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (boo)
            {
                //this.rtbView.RightMargin = 100;
                //int x = this.rtbView.RightMargin;
                var lines = this.rtbView.Lines.Length;
                this.rtbView.SelectionAlignment = HorizontalAlignment.Right;
                this.rtbView.SelectionColor = Color.Green;
                this.rtbView.AppendText(dt + Environment.NewLine);
                this.rtbView.SelectionAlignment = HorizontalAlignment.Right;
                this.rtbView.SelectionColor = Color.Green;
                this.rtbView.AppendText("" + msg + Environment.NewLine);
            }
            else
            {
                this.rtbView.SelectionAlignment = HorizontalAlignment.Right;
                this.rtbView.SelectionColor = Color.Green;
                this.rtbView.AppendText(dt + Environment.NewLine);
                this.rtbView.SelectionAlignment = HorizontalAlignment.Right;
                this.rtbView.SelectionColor = Color.Green;
                this.rtbView.AppendText("失败: " + msg + Environment.NewLine);
            }
        }
        private void ShowFinished(string msg,Color color,Definition.ShowMsgType type)
        {
            string text = "";
            if (type == Definition.ShowMsgType.Receive)
            {
                text = "接收完毕：";
                this.rtbView.SelectionAlignment = HorizontalAlignment.Left;

            }
            else if (type == Definition.ShowMsgType.Send)
            {
                text = "发送完毕：";
                this.rtbView.SelectionAlignment = HorizontalAlignment.Right;
            }

            string dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.rtbView.SelectionColor = Color.Blue;
            this.rtbView.AppendText(dt +Environment.NewLine);// "\r\n");
            this.rtbView.SelectionColor = Color.Blue;
            this.rtbView.AppendText(text + msg + Environment.NewLine);// + "\r\n");
            //FormHelper.FileMsgAdd(@"打开文件",@"#"+ msg, this.rtbView);
            
            //FormHelper.FileMsgAdd("打开", msg, this.rtbView);
            //this.rtbView.InsertLink("打开", msg);
            //var msgs=msg.Split('\\');
            //this.rtbView.AppendText(" ");
            //string directory=string.Join("\\", msgs, 0, msgs.Length - 1);
            //FormHelper.FileMsgAdd("打开目录", directory, this.rtbView);
            //this.rtbView.InsertLink("打开目录", directory);
            //this.rtbView.AppendText(@"明日科技：http://dd");
           // this.rtbView.AppendText(Environment.NewLine);
        }
        private void ShowRec(Msg e)
        {
            if (!this.IsDisposed && this.IsHandleCreated)
            {
                string dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                this.Invoke(new Action(() =>
                {
                    this.rtbView.SelectionAlignment = HorizontalAlignment.Left;
                    this.rtbView.AppendText(dt + Environment.NewLine);
                    if (e.MsgType == Definition.MsgType.File)
                    {
                        this.rtbView.AppendText("收到文件:" + e.msg + Environment.NewLine);
                    }
                    else if (e.MsgType == Definition.MsgType.Txt)
                    {
                        this.rtbView.AppendText("" + e.msg + Environment.NewLine);
                    }
                    FormHelper.FlashWin(this.Handle);
                    this.WindowState = FormWindowState.Normal;
                }));
            }
        }

        void ucrec_RecEvent(object sender, bool e)
        {
            //throw new NotImplementedException();
            if (e)
            {
                //tcp.Send()
            }
            else
            {
                this.panel1.Controls.Remove((Control)sender);
            }
        }
        private void ShowError(string e)
        {
            if (!this.IsDisposed && this.IsHandleCreated)
            {
                string dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                this.Invoke(new Action(() =>
                {
                    this.rtbView.SelectionAlignment = HorizontalAlignment.Center;
                    this.rtbView.SelectionColor = Color.Red;
                    this.rtbView.AppendText(dt + Environment.NewLine);
                    this.rtbView.SelectionAlignment = HorizontalAlignment.Center;
                    this.rtbView.SelectionColor = Color.Red;
                    this.rtbView.AppendText(e + Environment.NewLine);
                }));
            }
        }

        private void Gc()
        {
            MyMessage.MsgEvent -= Message_MsgEvent;
        }
        public void ProgressCall(int progress)
        {

        }
        public void RecMsg(Msg e)
        {
            this.BeginInvoke(new Action(() =>
            {
                if (e.IpAddress.ToString() == this.Remoteip.ToString())
                {
                    if (e.MsgType == Definition.MsgType.File)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            AddRecControl(e);
                        }));
                    }
                    else if (e.MsgType == Definition.MsgType.Txt)
                        ShowRec(e);
                    else if (e.MsgType == Definition.MsgType.Error)
                    {
                        ShowError(e.msg);
                    }
                }
            }));
        }
        ~FromChat()
        {
            Gc();
        }
    }
}
