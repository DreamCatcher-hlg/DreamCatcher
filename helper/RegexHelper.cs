
namespace MyCommon.helper
{
    public class RegexHelper
    {
        public static bool IsCorrenctIP(string ip)
        {
            //如果为空，认为验证不合格
            if (string.IsNullOrEmpty(ip))
            {
                return false;
            }

            //清除要验证字符传中的空格
            ip = ip.Trim();

            //模式字符串，正则表达式
            string patten = @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$";
            //验证
            return System.Text.RegularExpressions.Regex.IsMatch(ip, patten);
        }
    }
}
