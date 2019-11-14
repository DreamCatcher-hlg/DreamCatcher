using MyCommon;
using System;
using System.Windows.Forms;
namespace MyChat
{
    public partial class FormConfig : Form
    {
        public FormConfig()
        {
            InitializeComponent();
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            this.Load += FormConfig_Load;
            this.checkBox1.CheckedChanged+=checkBox1_CheckedChanged;
            this.checkBox2.CheckedChanged += checkBox2_CheckedChanged;
        }

        void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Sysconstant.MutiBordcast = this.checkBox2.Checked;
        }

        void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Sysconstant.AutoStart(this.checkBox1.Checked, Application.ExecutablePath, true);
        }

        void FormConfig_Load(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            this.checkBox1.Checked = Sysconstant.GetAutoStart(Application.ExecutablePath);
        }
    }
}
