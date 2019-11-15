using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using WindowsFormsApplication17.Entity;

namespace MyCommon
{
    public class Sysconstant
    {
        public static bool MutiBordcast = false;
        public static event EventHandler<IPAddress> NewIpEndPoint;
        public static int port = 12000;
        private static readonly object remoteobj = new object();
        private static List<IPAddress> remoteLi = new List<IPAddress>();
        public static List<IPAddress> RemoteLi
        {
            get
            {
                return remoteLi;
            }
        }

        //public static Dictionary<string,remo>
        //private static IPAddress[] localIPAddressli = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(x => x.ToString().Contains("192.168")).ToArray();
        public static IPAddress[] LocalIPAddress
        {
            get
            {
                return Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(x => x.ToString().Contains("192.168")).ToArray();
            }
        }

        public static UDPMsgEntity LocalBordcastEntity
        {
            get
            {
                UDPMsgEntity entity = new UDPMsgEntity();
                entity.HostName = Dns.GetHostName();
                entity.IPs = LocalIPAddress;
                //entity.MAC = GetMacByNetworkInterface();
                return entity;
            }
        }
        public static void AddRemotePoint(IPAddress endpoint)
        {
            lock (remoteobj)
            {
                var point = remoteLi.Where(x => IPAddress.Equals(x, endpoint)).SingleOrDefault();
                if (point == null)
                {
                    remoteLi.Add(endpoint);
                }
                if (NewIpEndPoint != null)
                {
                    NewIpEndPoint(null, endpoint);
                }

            }
        }
        #region 通过NetworkInterface获取MAC地址
        /// <summary>
        /// 通过NetworkInterface获取MAC地址
        /// </summary>
        /// <returns></returns>
        private static string GetMacByNetworkInterface()
        {
            try
            {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface ni in interfaces)
                {
                    return BitConverter.ToString(ni.GetPhysicalAddress().GetAddressBytes());
                }
            }
            catch (Exception)
            {
            }
            return "00-00-00-00-00-00";
        }
        #endregion

        /// <summary>
        /// 修改程序在注册表中的键值  
        /// </summary>
        /// <param name="isAuto">true:开机启动,false:不开机自启</param>
        public static void AutoStart(bool isAuto, string path, bool IsNeed = false)
        {
            try
            {
                //var appname = System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName;
                var processname = "MyChat";// System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                if (isAuto == true)
                {

                    RegistryKey R_local = Registry.CurrentUser;//.LocalMachine;//RegistryKey R_local = Registry.CurrentUser;
                    RegistryKey R_run = R_local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    var value = R_run.GetValue(processname);
                    if (value == null)
                    {
                        R_run.SetValue(processname, path);
                    }
                    else if (value != null)
                    {
                        if (IsNeed)
                        {
                            R_run.SetValue(processname, path);
                        }
                    }
                    R_run.Close();
                    R_local.Close();
                }
                else
                {
                    RegistryKey R_local = Registry.CurrentUser;//.LocalMachine;//RegistryKey R_local = Registry.CurrentUser;
                    RegistryKey R_run = R_local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                    var value = R_run.GetValue(processname);
                    if (value != null)
                    {
                        R_run.SetValue(processname, "");
                    }
                    //R_run.DeleteValue(processname, false);
                    R_run.Close();
                    R_local.Close();
                }

                //GlobalVariant.Instance.UserConfig.AutoStart = isAuto;
            }
            catch (Exception ex)
            {
                //MessageBoxDlg dlg = new MessageBoxDlg();
                //dlg.InitialData("您需要管理员权限修改", "提示", MessageBoxButtons.OK, MessageBoxDlgIcon.Error);
                //dlg.ShowDialog();
                //MessageBox.Show("您需要管理员权限修改", "提示");
                log.Log.WriteSystemError(ex.ToString());
            }
        }
        public static bool GetAutoStart(string path)
        {
            bool IsAutoStart = false;
            try
            {
                //var appname = System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName;
                var processname = "MyChat";// System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                RegistryKey R_local = Registry.CurrentUser;//.LocalMachine;//RegistryKey R_local = Registry.CurrentUser;
                RegistryKey R_run = R_local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                var value = R_run.GetValue(processname);
                if (value != null && value.ToString().ToLower() == path.ToLower())
                {
                    IsAutoStart = true;
                }

                R_run.Close();
                R_local.Close();

            }
            catch (Exception ex)
            {
                log.Log.WriteSystemError(ex.ToString());
            }
            return IsAutoStart;
        }
    }
}
