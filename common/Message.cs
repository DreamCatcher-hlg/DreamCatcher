using MyCommon.Entity;
using System;
using System.Threading.Tasks;

namespace MyCommon
{
    public class MyMessage
    {
        public static event EventHandler<Msg> MsgEvent;
        public static void NewMsg(Msg msg)
        {
            if (MsgEvent != null)
            {
                Task.Run(() =>
                {
                    MsgEvent(null, msg);
                });
            }
        }
    }
}
