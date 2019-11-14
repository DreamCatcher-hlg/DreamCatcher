using MyCommon;
using MyCommon.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace MyChat
{
    public partial class FormMain : Form
    {
        const int CLOSE_SIZE = 15;
        readonly object obj = new object();
        readonly object formobj = new object();
        IList<IPAddress> IPlist = new List<IPAddress>();
        IList<FromChat> ChatFormli = new List<FromChat>();
        string oldtext = "";
        string NoInternettxt = "没有可用网络";
        FormConfig config;
        public FormMain()
        {
            InitializeComponent();
            oldtext = this.Text;
            this.Load += FormMain_Load;
            this.FormClosing += FormMain_FormClosing;
            this.tvUser.NodeMouseDoubleClick += tvUser_NodeMouseDoubleClick;
            MyMessage.MsgEvent += MyMessage_MsgEvent;
            Rectangle rect = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(rect.Width - this.Width - 100, (rect.Height - this.Height) / 2);
            this.notifyIcon1.DoubleClick += notifyIcon1_DoubleClick;
            this.notifyIcon1.MouseClick += notifyIcon1_MouseClick;
            this.linkLabel1.MouseClick += linkLabel1_MouseClick;
            this.tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.tabControl1.DrawItem += tabControl1_DrawItem;
        }

        void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                Graphics g = e.Graphics;
                Font font = new Font("微软雅黑", 14f, System.Drawing.GraphicsUnit.Pixel);
                SolidBrush brush = new SolidBrush(Color.Black);
                if (e.Index == this.tabControl1.SelectedIndex)
                {
                    brush = new SolidBrush(Color.FromArgb(102,102,102));
                }

                RectangleF rectangle = (RectangleF)(this.tabControl1.GetTabRect(e.Index));
                RectangleF rectangle2 = new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
                //g.FillRectangle(new SolidBrush(SystemColors.ButtonHighlight), rectangle2);
                g.FillRectangle(new SolidBrush(Color.FromArgb(197, 229, 255)), rectangle2);
                StringFormat sformat = new StringFormat();
                sformat.LineAlignment = StringAlignment.Center;
                sformat.Alignment = StringAlignment.Center;
                g.DrawString(((TabControl)sender).TabPages[e.Index].Text, font, brush, rectangle2, sformat);
                //Rectangle myTabRect = this.tabControl1.GetTabRect(e.Index);
                ////先添加 TabPage属性
                //e.Graphics.DrawString(this.tabControl1.TabPages[e.Index].Text
                //, this.Font, SystemBrushes.ControlText, myTabRect.X + 2, myTabRect.Y + 2);
                ////再画一个矩形框
                //using (Pen p = new Pen(Color.White))
                //{
                //    myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
                //    myTabRect.Width = CLOSE_SIZE;
                //    myTabRect.Height = CLOSE_SIZE;
                //    e.Graphics.DrawRectangle(p, myTabRect);
                //}
                ////填充矩形框
                //Color recColor = e.State == DrawItemState.Selected ? Color.White : Color.White;
                //using (Brush b = new SolidBrush(recColor))
                //{
                //    e.Graphics.FillRectangle(b, myTabRect);
                //}
                ////画关闭符号
                //using (Pen objpen = new Pen(Color.Black))
                //{
                //    //"\"线
                //    Point p1 = new Point(myTabRect.X + 3, myTabRect.Y + 3);
                //    Point p2 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + myTabRect.Height - 3);
                //    e.Graphics.DrawLine(objpen, p1, p2);
                //    //"/"线
                //    Point p3 = new Point(myTabRect.X + 3, myTabRect.Y + myTabRect.Height - 3);
                //    Point p4 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + 3);
                //    e.Graphics.DrawLine(objpen, p3, p4);
                //    ////=============================================
                //    Bitmap bt = new Bitmap(image);
                //    Point p5 = new Point(myTabRect.X - 50, 4);
                //    e.Graphics.DrawImage(bt, p5);
                //    //e.Graphics.DrawString(this.MainTabControl.TabPages[e.Index].Text, this.Font, objpen.Brush, p5);
                //}

                //绘制小图标
                //==============================================================================
                //Bitmap bt = new Bitmap("E:\\1\\2.jpg");
                //Point p5 = new Point(4, 4);
                ////e.Graphics.DrawImage(bt, e.Bounds);
                //e.Graphics.DrawImage(bt, p5);
                //Pen pt = new Pen(Color.Red);
                ////e.Graphics.DrawString(this.MainTabControl.TabPages[e.Index].Text, this.Font, pt.Brush, e.Bounds);
                //e.Graphics.DrawString(this.MainTabControl.TabPages[e.Index].Text, this.Font, pt.Brush, p5);
                e.Graphics.Dispose();
            }
            catch (Exception)
            {
            }
        }

        void linkLabel1_MouseClick(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.Button == MouseButtons.Left)
            {
                if (config == null || config.IsDisposed )
                {
                    config = new FormConfig();
                    config.Location = new Point(this.Location.X + this.Width / 2 - config.Width / 2, this.Location.Y + this.Height / 2 - config.Height / 2);
                    config.Show();
                }
            }
        }

        void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.Button == MouseButtons.Left)
            {
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.WindowState = FormWindowState.Normal;
                    this.ShowInTaskbar = true;
                }
                if (!this.Visible)
                {
                    this.Visible = true;
                }
                this.Activate();
            }
        }

        void notifyIcon1_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }

        void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
            }
            if (!this.Visible)
            {
                this.Visible = true;
            }
            this.Activate();
        }

        void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                //if (!e.Cancel)
                {
                    e.Cancel = true;
                    //this.WindowState = FormWindowState.Minimized;
                    this.Visible = false;
                    //this.ShowInTaskbar = false;
                }
                
            }
            catch (Exception ex)
            {
                MyCommon.log.Log.WriteLog(ex.ToString());
            }
        }

        void MyMessage_MsgEvent(object sender, Msg e)
        {
            //throw new NotImplementedException();
            if(!this.IsDisposed&&this.IsHandleCreated)
            this.BeginInvoke(new Action(() =>
            {
                IList<FromChat> formli = new List<FromChat>();
                //lock (formobj)
                {
                    formli = ChatFormli.ToList();
                }
                foreach (FromChat v in formli)
                {
                    if (IPAddress.Equals(v.Remoteip, e.IpAddress))
                    {
                        //v.RecEvent(e);
                        return;
                    }
                }
                FromChat form = new FromChat(e.IpAddress);
                form.FormClosing+=form_FormClosing;
                form.Show();
                //lock (formobj)
                {
                    ChatFormli.Add(form);
                }
                form.RecMsg(e);
            }));
        }

        void tvUser_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var ip = e.Node.Tag as IPAddress;
            IList<FromChat> formli = new List<FromChat>();
            //lock (formobj)
            {
                formli = ChatFormli.ToList();
            }
            foreach (FromChat v in formli)
            {
                if (IPAddress.Equals(v.Remoteip, ip))
                {
                    //v.RecEvent(e);
                    v.WindowState = FormWindowState.Normal;
                    v.BringToFront();
                    return;
                }
            }
            FromChat form = new FromChat(ip);
            form.FormClosing += form_FormClosing;
            form.Show();
            //lock (formobj)
            {
                ChatFormli.Add(form);
            }
        }

        void form_FormClosing(object sender, FormClosingEventArgs e)
        {
            //throw new NotImplementedException();
            ChatFormli.Remove(sender as FromChat);
        }

        void FormMain_Load(object sender, EventArgs e)
        {
            if (!Bordcast.BordCastInstance.CheckNet())
            {
                this.Text = NoInternettxt;// "没有可用局域网";
                return;
            }
            TcpConnect.FileRecInstance.Init();
            //FileHelper.FileRecInstance.ErrorEvent += FileRecInstance_ErrorEvent;
            Sysconstant.NewIpEndPoint += Sysconstant_NewIpEndPoint;
            Bordcast.BordCastInstance.Init();
            //Bordcast.BordCastInstance.ErrorEvent += BordCastInstance_ErrorEvent;
          
            
        }

        void FileRecInstance_ErrorEvent(object sender, string e)
        {
            //throw new NotImplementedException();
            MessageBox.Show(e);
            MyCommon.log.Log.WriteLog(e);
        }

        void BordCastInstance_ErrorEvent(object sender, string e)
        {
            //throw new NotImplementedException();
            MessageBox.Show(e);
            MyCommon.log.Log.WriteLog(e);
        }

        void Sysconstant_NewIpEndPoint(object sender, System.Net.IPAddress e)
        {
            //throw new NotImplementedException();
            bool Isexist = false;
            lock (obj)
            {
                var ip = IPlist.Where(x => IPAddress.Equals(x, e)).SingleOrDefault();
                if (ip != null)
                {
                    Isexist = true;
                }
                else
                {
                    IPlist.Add(e);
                }
            }
            if (!Isexist)
                this.BeginInvoke(new Action(() =>
                {

                    string text = "";
                    var ips = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                    if (ips.Contains(e))
                    {
                        text = "自己(" + e + ")";
                    }
                    else
                    {
                        text = e.ToString();
                    }
                    var node = this.tvUser.Nodes.Add(text);// (e.Address.ToString());
                    node.Tag = e;

                }));
        }

        private void 刷新ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Text != NoInternettxt)//"没有可用局域网")
            {
                return;
            }
            if (!Bordcast.BordCastInstance.CheckNet())
            {
                this.Text = NoInternettxt;// "没有可用局域网";
                return;
            }
            else
            {
                this.Text = oldtext;
            }
            TcpConnect.FileRecInstance.Init();
            //FileHelper.FileRecInstance.ErrorEvent += FileRecInstance_ErrorEvent;
            Sysconstant.NewIpEndPoint += Sysconstant_NewIpEndPoint;
            Bordcast.BordCastInstance.Init();
            //Bordcast.BordCastInstance.ErrorEvent += BordCastInstance_ErrorEvent;
            Rectangle rect = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(rect.Width - this.Width - 100, (rect.Height - this.Height) / 2);
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //this.Close();
            try
            {
                MyMessage.MsgEvent -= MyMessage_MsgEvent;
                this.notifyIcon1.Visible = false;
                foreach (FromChat form in ChatFormli.ToArray())
                {
                    form.Close();
                }
                //Application.Exit(new CancelEventArgs(true));
                System.Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MyCommon.log.Log.WriteSystemError(ex.ToString());
            }
        }

        private void 手动添加联系人ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAddUser form = new FormAddUser(this);
            form.ShowDialog();
        }
    }
}
