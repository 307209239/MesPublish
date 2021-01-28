using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace MesPublish
{
    public class Service
    {
        /// <summary>
        /// 用户
        /// </summary>
        public static string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public static string PassWord { get; set; }

        /// <summary>
        /// 域名
        /// </summary>
        public static string Domain { get; set; }

        /// <summary>
        /// ping ip,测试能否ping通
        /// </summary>
        /// <param name="strIP">IP地址</param>
        /// <returns></returns>
        public static bool PingIp(string strIP)
        {
            bool bRet = false;
            try
            {
                Ping pingSend = new Ping();
                PingReply reply = pingSend.Send(strIP, 1000);
                if (reply.Status == IPStatus.Success)
                    bRet = true;
            }
            catch (Exception)
            {
                bRet = false;
            }

            return bRet;
        }

        /// <summary>
        /// 获取远程服务器上运行的camstar 服务
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>

        public static IEnumerable<string> GetSvc(string ip)
        {
            var svc = new List<string>();
            
            var co = new ConnectionOptions();
            if (!string.IsNullOrEmpty(UserName))
            {
                co.Username = Domain + "\\" + UserName; //连接需要的用户名
                co.Password = PassWord; //连接需要的密码
            }

            string connectString =
                "SELECT * FROM Win32_Service WHERE State='Running' and DisplayName like '%Camstar%'"; //查询字符串
            System.Management.ManagementScope ms =
                new System.Management.ManagementScope("\\\\" + ip + "\\root\\cimv2", co);
            var oq = new System.Management.ObjectQuery(connectString);
            var query = new ManagementObjectSearcher(ms, oq);
            var queryCollection = query.Get();
            foreach (ManagementObject mo in queryCollection)
            {
                // string hardwareID    = mo["HardwareID"]); //直接根据属性名得到属性的值

                //遍历所有属性，得到所有属性的值
                PropertyDataCollection searcherProperties = mo.Properties;
                foreach (PropertyData sp in searcherProperties)
                {
                    if (sp.Name == "Name")
                    {
                        svc.Add(sp.Value.ToString());
                    }
                }
            }

            return svc;
        }

        /// <summary>
        /// 重启指定服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="ip"></param>
        public static void RestartService(string serviceName,string ip)
        {
            

            var co = new ConnectionOptions();
            if (!string.IsNullOrEmpty(UserName))
            {
                co.Username =Domain+"\\"+ UserName; //连接需要的用户名
                co.Password = PassWord; //连接需要的密码
            }

            string connectString =
                $@"SELECT * FROM Win32_Service WHERE  Name='{serviceName}'"; //查询字符串
            System.Management.ManagementScope ms =
                new System.Management.ManagementScope("\\\\" + ip + "\\root\\cimv2", co);
            var oq = new System.Management.ObjectQuery(connectString);
            var query = new ManagementObjectSearcher(ms, oq);
            var queryCollection = query.Get();
            foreach (ManagementObject mo in queryCollection)
            {
                mo.InvokeMethod("StopService", null);
                mo.Put();
                mo.InvokeMethod("StartService", null);
                mo.Put();

            }

        }

        /// <summary>
        /// 关闭指定服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="ip"></param>
        public static void StopService(string serviceName, string ip)
        {


            var co = new ConnectionOptions();
            if (!string.IsNullOrEmpty(UserName))
            {
                co.Username = Domain + "\\" + UserName; //连接需要的用户名
                co.Password = PassWord; //连接需要的密码
            }

            string connectString =
                $@"SELECT * FROM Win32_Service WHERE  Name='{serviceName}'"; //查询字符串
            System.Management.ManagementScope ms =
                new System.Management.ManagementScope("\\\\" + ip + "\\root\\cimv2", co);
            var oq = new System.Management.ObjectQuery(connectString);
            var query = new ManagementObjectSearcher(ms, oq);
            var queryCollection = query.Get();
            foreach (ManagementObject mo in queryCollection)
            {
                mo.InvokeMethod("StopService", null);
            }

        }
        /// <summary>
        /// 启动指定服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="ip"></param>
        public static void StartService(string serviceName, string ip)
        {


            var co = new ConnectionOptions();
            if (!string.IsNullOrEmpty(UserName))
            {
                co.Username = Domain + "\\" + UserName; //连接需要的用户名
                co.Password = PassWord; //连接需要的密码
            }

            string connectString =
                $@"SELECT * FROM Win32_Service WHERE  Name='{serviceName}'"; //查询字符串
            System.Management.ManagementScope ms =
                new System.Management.ManagementScope("\\\\" + ip + "\\root\\cimv2", co);
            var oq = new System.Management.ObjectQuery(connectString);
            var query = new ManagementObjectSearcher(ms, oq);
            var queryCollection = query.Get();
            foreach (ManagementObject mo in queryCollection)
            {
                mo.InvokeMethod("StartService", null);
            }

        }
        /// <summary>
        /// 重启操作系统
        /// </summary>
        public static void RestartOpSystem(string ip)
        {
            
            var co = new ConnectionOptions();
            if (!string.IsNullOrEmpty(UserName))
            {
                co.Username = Domain + "\\" + UserName; //连接需要的用户名
                co.Password = PassWord; //连接需要的密码
            }

            string connectString =
                "SELECT * FROM Win32_OperatingSystem"; //查询字符串
            System.Management.ManagementScope ms =
                new System.Management.ManagementScope("\\\\" + ip + "\\root\\cimv2", co);
            var oq = new System.Management.ObjectQuery(connectString);
            var query = new ManagementObjectSearcher(ms, oq);
            var queryCollection = query.Get();
            foreach (ManagementObject mo in queryCollection)
            {
                mo.InvokeMethod("Reboot", null);
            }

            
        }
    }
}
