using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MesPublish
{
    public class User
    {
        public string IP { get; set; } = string.Empty;

        public string UserName => Service.UserName;

        public string PassWord => Service.PassWord;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip">目标IP地址</param>
        public User(string ip)
        {
            IP = ip;
        }

        /// <summary>
        /// 返回所有用户
        /// </summary>
        /// <returns>帐号数组</returns>
        public string[] ViweUsers()
        {
            try
            {
                ConnectionOptions options = new ConnectionOptions();
                options.Username = UserName;
                options.Password = PassWord;

                ManagementScope Conn = new ManagementScope(@"\\" + IP + @"\root\cimv2", options);
                Conn.Connect();
                //确定WMI操作的内容
                ObjectQuery oq = new ObjectQuery("SELECT * FROM Win32_UserAccount");
                ManagementObjectSearcher query1 = new ManagementObjectSearcher(Conn, oq);
                //获取WMI操作内容
                ManagementObjectCollection queryCollection1 = query1.Get();
                //根据使用者选择，执行相应的远程操作
                string[] s = new string[queryCollection1.Count];
                int i = 0;
                foreach (ManagementObject mo in queryCollection1)
                {
                    s[i] += mo.GetPropertyValue("Name");
                    i++;
                }
                return s;
            }
            //报错
            catch (Exception ee)
            {
                throw ee;
            }
        }

        /// <summary>
        /// 修改帐号密码
        /// </summary>
        /// <param name="changeUserName">要修改的帐号</param>
        /// <param name="changePassWord">修改后的密码</param>
        /// <returns>是否成功</returns>
        public bool ChangePassWordByCmd(string changeUserName, string changePassWord)
        {
            try
            {

                ConnectionOptions options = new ConnectionOptions();
                options.Username = UserName;
                options.Password = PassWord;

                ManagementScope Conn = new ManagementScope(@"\\" + IP + @"\root\cimv2", options);
                Conn.Connect();

                ManagementPath path = new ManagementPath("Win32_Process");
                ManagementClass processClass = new ManagementClass(Conn, path, null);

                ManagementBaseObject inParams = processClass.GetMethodParameters("Create");

                //Parameters for creation of process.
                inParams["CommandLine"] = "net user " + changeUserName + " " + changePassWord + "";

                //Invoke Method.
                ManagementBaseObject outParams = processClass.InvokeMethod("Create", inParams, null);

                return true;
            }
            //报错
            catch (Exception ee)
            {
                return false;
            }
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="newUserName">用户名</param>
        /// <param name="newPassWord">密码</param>
        /// <param name="description"></param>
        /// <returns>是否成功</returns>
        public bool AddUserByCmd(string newUserName, string newPassWord,string description)
        {
            try
            {
                ConnectionOptions options = new ConnectionOptions();
                options.Username = UserName;
                options.Password = PassWord;
                ManagementScope Conn = new ManagementScope(@"\" + IP + "rootCIMV2", options);
                Conn.Connect();
                ManagementPath path = new ManagementPath("Win32_Process");
                ManagementClass processClass = new ManagementClass(Conn, path, null);
                ManagementBaseObject inParams1 = processClass.GetMethodParameters("Create");
                //Parameters for creation of process.
                inParams1["CommandLine"] = $"net user {newUserName} {newPassWord} /add /expires:never /fullname:\"{description}\"";
                //Invoke Method.
                ManagementBaseObject outParams1 = processClass.InvokeMethod("Create", inParams1, null);
                ManagementBaseObject inParams2 = processClass.GetMethodParameters("Create");
                inParams2["CommandLine"] = "net localgroup Users " + newUserName + " /add ";
                ManagementBaseObject outParams2 = processClass.InvokeMethod("Create", inParams2, null);
                return true;
            }
            //报错
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="delUserName">要删除的用户名</param>
        /// <returns>是否成功</returns>
        public bool DelUserByCmd(string delUserName)
        {
            try
            {
                ConnectionOptions options = new ConnectionOptions();
                options.Username = UserName;
                options.Password = PassWord;

                ManagementScope Conn = new ManagementScope(@"\\" + IP + @"\root\cimv2", options);
                Conn.Connect();

                ManagementPath path = new ManagementPath("Win32_Process");
                ManagementClass processClass = new ManagementClass(Conn, path, null);

                ManagementBaseObject inParams = processClass.GetMethodParameters("Create");

                inParams["CommandLine"] = "net user " + delUserName + " /delete";

                ManagementBaseObject outParams = processClass.InvokeMethod("Create", inParams, null);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
