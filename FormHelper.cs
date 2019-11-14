using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MyChat
{
    class FormHelper
    {
        /// <summary>
        /// 隐藏控件的光标
        /// </summary>
        /// <param name="hWnd">控件句柄</param>
        /// <returns></returns>
        [DllImport("user32", EntryPoint = "HideCaret")]
        public static extern bool HideCaret(IntPtr hWnd);
        [DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "FlashWindowEx")]
        private static extern void FlashWindowEx(ref FLASHWINFO pwfi);
        public struct FLASHWINFO
        {
            public UInt32 cbSize;//该结构的字节大小
            public IntPtr hwnd;//要闪烁的窗口的句柄，该窗口可以是打开的或最小化的
            public UInt32 dwFlags;//闪烁的状态
            public UInt32 uCount;//闪烁窗口的次数
            public UInt32 dwTimeout;//窗口闪烁的频度，毫秒为单位；若该值为0，则为默认图标的闪烁频度
        }
        public const UInt32 FLASHW_TRAY = 2;
        public const UInt32 FLASHW_TIMERNOFG = 12;
        public static void FlashWin(IntPtr handle)
        {
            FLASHWINFO fInfo = new FLASHWINFO();
            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = handle;
            fInfo.dwFlags = FLASHW_TRAY | FLASHW_TIMERNOFG;
            fInfo.uCount = 3;// UInt32.MaxValue;
            fInfo.dwTimeout = 500;
            FlashWindowEx(ref fInfo);
        }

        public const int CFE_LINK = 0x20;
        public const int CFM_LINK = 0x20;
        public const int EM_SETCHARFORMAT = 0x444;
        public const int SCF_SELECTION = 1;

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public class CHARFORMAT2A
        {
            public int cbSize;
            public int dwMask;
            public int dwEffects;
            public int yHeight;
            public int yOffset;
            public int crTextColor;
            public byte bCharSet;
            public byte bPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
            public byte[] szFaceName = new byte[0x20];
            public short wWeight;
            public short sSpacing;
            public int crBackColor;
            public int lcid;
            public int dwReserved;
            public short sStyle;
            public short wKerning;
            public byte bUnderlineType;
            public byte bAnimation;
            public byte bRevAuthor;
        }
        [DllImport("user32.dll ", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam,
                [In, Out, MarshalAs(UnmanagedType.LPStruct)]   CHARFORMAT2A lParam);

        public const int WM_USER = 0x0400;
        public const int EM_GETPARAFORMAT = WM_USER + 61;
        public const int EM_SETPARAFORMAT = WM_USER + 71;
        public const long MAX_TAB_STOPS = 32;
        public const uint PFM_LINESPACING = 0x00000100;
        [StructLayout(LayoutKind.Sequential)]
        private struct PARAFORMAT2
        {
            public int cbSize;
            public uint dwMask;
            public short wNumbering;
            public short wReserved;
            public int dxStartIndent;
            public int dxRightIndent;
            public int dxOffset;
            public short wAlignment;
            public short cTabCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public int[] rgxTabs;
            public int dySpaceBefore;
            public int dySpaceAfter;
            public int dyLineSpacing;
            public short sStyle;
            public byte bLineSpacingRule;
            public byte bOutlineLevel;
            public short wShadingWeight;
            public short wShadingStyle;
            public short wNumberingStart;
            public short wNumberingStyle;
            public short wNumberingTab;
            public short wBorderSpace;
            public short wBorderWidth;
            public short wBorders;
        }

        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, ref PARAFORMAT2 lParam);

        /// <summary>
        /// 设置行距
        /// </summary>
        /// <param name="ctl">控件</param>
        /// <param name="dyLineSpacing">间距</param>
        public static void SetLineSpace(Control ctl, int dyLineSpacing)
        {
            PARAFORMAT2 fmt = new PARAFORMAT2();
            fmt.cbSize = Marshal.SizeOf(fmt);
            fmt.bLineSpacingRule = 4;// bLineSpacingRule;
            fmt.dyLineSpacing = dyLineSpacing;
            fmt.dwMask = PFM_LINESPACING;
            try
            {
                SendMessage(new HandleRef(ctl, ctl.Handle), EM_SETPARAFORMAT, 0, ref fmt);
            }
            catch
            {

            }
        }

        public static void FileMsgAdd(string fileMsg, string path, RichTextBox box)
        {
            var filemsg = TextToRtf(fileMsg);
            var path1 = TextToRtf(path);
            string insertRtf = @"{\rtf1" + TextToRtf(fileMsg) + @"\v #" + TextToRtf(path) + @"\v0}";
            int iniPos = box.SelectionStart;
            box.SelectedRtf = insertRtf;
            box.SelectionStart = iniPos;
            box.SelectionLength = insertRtf.Length;// filemsg.Length + path1.Length;//fileMsg.Length + path.Length;//filemsg.Length + path1.Length;//

            CHARFORMAT2A vCharFormat2a = new CHARFORMAT2A();
            vCharFormat2a.cbSize = Marshal.SizeOf(typeof(CHARFORMAT2A));
            vCharFormat2a.dwMask = CFM_LINK;
            vCharFormat2a.dwEffects = CFE_LINK;
            SendMessage(box.Handle, EM_SETCHARFORMAT, SCF_SELECTION,
                    vCharFormat2a);
        }

        private static string TextToRtf(string AText)
        {
            string vReturn = "";
            foreach (char vChar in AText)
            {
                switch (vChar)
                {
                    case '\\':
                        vReturn += @"\\";
                        break;
                    case '{':
                        vReturn += @"\{";
                        break;
                    case '}':
                        vReturn += @"\}";
                        break;
                    default:
                        if (vChar > (char)127)
                            vReturn += @"\u" + ((int)vChar).ToString() + "?";
                        else
                            vReturn += vChar;
                        break;
                }
            }
            return vReturn;
        }

    }
}
