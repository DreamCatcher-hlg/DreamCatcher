using MyCommon;
using MyCommon.helper;
using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;

namespace MyChat
{
    public partial class FormAddUser : Form
    {
        public FormAddUser(Form f)
        {
            InitializeComponent();
            this.Location = new Point(f.Location.X + f.Width / 2 - this.Width / 2, f.Location.Y + f.Height / 2 - this.Height / 2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var IP=this.textBox1.Text.Trim();
            if (!RegexHelper.IsCorrenctIP(IP))
            {
                MessageBox.Show("输入的IP地址无效,请输入正确IP地址", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Sysconstant.AddRemotePoint(IPAddress.Parse(IP));
            this.Close();
        }

    }
}
