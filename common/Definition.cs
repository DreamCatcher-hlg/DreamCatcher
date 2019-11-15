
namespace MyCommon
{
    public class Definition
    {
        public enum MsgType
        {
            /// <summary>
            /// 文本消息
            /// </summary>
            Txt,
            /// <summary>
            /// 文件消息
            /// </summary>
            File,
            /// <summary>
            /// 异常消息
            /// </summary>
            Error
        }
        public enum ShowMsgType
        {
            Receive,
            Send
        }
    }
}
