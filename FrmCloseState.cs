using System;
using System.Drawing;
using System.Windows.Forms;

namespace MyChat
{
    public partial class FrmCloseState : Form
    {
        System.Windows.Forms.Timer timer = new Timer();
        int Cou = 0;
        string text = "请等待操作完成";
        public FrmCloseState(Form f)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(f.Location.X + f.Width / 2 - this.Width / 2, f.Location.Y + f.Height / 2 - this.Height / 2);
            this.Load += FrmCloseState_Load;
            this.FormClosed += FrmCloseState_FormClosed;
        }

        void FrmCloseState_FormClosed(object sender, FormClosedEventArgs e)
        {
            //throw new NotImplementedException();
            timer.Stop();
            timer.Dispose();
            timer = null;
        }

        void FrmCloseState_Load(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            timer.Interval = 100;
            timer.Tick += timer_Tick;
            timer.Start();

        }

        void timer_Tick(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if(Cou>3){
                Cou=0;
            }

            int co = Cou;
            string stext = text;
            while (--co >= 0)
            {
                stext += "。";
            }
            this.label1.Text = stext;
            Cou++;
        }
    }
}
