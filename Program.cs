using MyCommon;
using System;
using System.Text;
using System.Windows.Forms;

namespace MyChat
{
    static class Program
    {
        static System.Threading.Mutex mutex = null;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool flag = false;
            mutex = new System.Threading.Mutex(true, "MyChat", out flag);
            if (!flag)
            {
                MessageBox.Show("当前程序已启动", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Application.ThreadException += Application_ThreadException;
            System.AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            System.Threading.Tasks.Task.Run(() =>
            {
                Sysconstant.AutoStart(true, Application.ExecutablePath);
            });
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string error = GetExceptionMsg(e.ExceptionObject as Exception, e.ToString());
            MyCommon.log.Log.WriteSystemError(error);
            MessageBox.Show(error);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            string error = GetExceptionMsg(e.Exception, e.ToString());
            MyCommon.log.Log.WriteLog(error);
        }

        /// <summary>
        /// 生成自定义异常消息
        /// </summary>
        /// <param name="ex">异常对象</param>
        /// <param name="backStr">备用异常消息：当ex为null时有效</param>
        /// <returns>异常字符串文本</returns>
        static string GetExceptionMsg(Exception ex, string backStr)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("****************************异常文本****************************");
            sb.AppendLine("【出现时间】：" + DateTime.Now.ToString());
            if (ex != null)
            {
                sb.AppendLine("【异常类型】：" + ex.GetType().Name);
                sb.AppendLine("【异常信息】：" + ex.Message);
                sb.AppendLine("【堆栈调用】：" + ex.StackTrace);
            }
            else
            {
                sb.AppendLine("【未处理异常】：" + backStr);
            }
            sb.AppendLine("***************************************************************");
            return sb.ToString();
        }
       
    }
}
