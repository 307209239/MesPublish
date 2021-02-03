using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
    public static class UserCommons
    {
        /// <summary>
        /// 添加账户
        /// </summary>
        /// <param name="userName">用户</param>
        /// <param name="passWord">密码</param>
        /// <param name="displayName">名称</param>
        /// <param name="description">描述</param>
        /// <param name="groupName">组</param>
        /// <param name="canChangePwd">可以更改密码</param>
        /// <param name="pwdExpires">密码永不过期</param>
        /// <param name="ip"></param>
        /// <returns></returns>
        static Result CreateLocalWindowsAccount(string userName, string passWord, string displayName, string description, string groupName="Users", bool canChangePwd=false, bool pwdExpires=true,string ip=null)
        {
            bool retIsSuccess = false;
            try
            {
                PrincipalContext context = new PrincipalContext(ContextType.Machine);
                UserPrincipal user = new UserPrincipal(context);
                user.SetPassword(passWord);
                user.DisplayName = displayName;
                user.Name = userName;
                user.Description = description;
                user.UserCannotChangePassword = canChangePwd;
                user.PasswordNeverExpires = pwdExpires;
                user.Save();
                GroupPrincipal group = GroupPrincipal.FindByIdentity(context, groupName);
                group.Members.Add(user);
                group.Save();
                return  new Result()
                {
                    Status = true,
                    Message = "添加成功"
                };
            }
            catch (Exception ex)
            {
                return  new Result()
                {
                    Status = false,
                    Message = ex.Message
                };
               
            }
           
        }

        static GroupPrincipal CreateGroup(string groupName, Boolean isSecurityGroup,string ip)
        {
            GroupPrincipal retGroup = null;
            try
            {
                retGroup = IsGroupExist(groupName,ip);
                if (retGroup == null)
                {
                    PrincipalContext ctx = new PrincipalContext(ContextType.Domain,ip);
                    GroupPrincipal insGroupPrincipal = new GroupPrincipal(ctx);
                    insGroupPrincipal.Name = groupName;
                    insGroupPrincipal.IsSecurityGroup = isSecurityGroup;
                    insGroupPrincipal.GroupScope = GroupScope.Local;
                    insGroupPrincipal.Save();
                    retGroup = insGroupPrincipal;
                }
            }
            catch (Exception ex)
            {

            }
            return retGroup;
        }

        static GroupPrincipal IsGroupExist(string groupName,string ip)
        {
            GroupPrincipal retGroup = null;
            try
            {
                PrincipalContext ctx = new PrincipalContext(ContextType.Machine,ip);
                GroupPrincipal qbeGroup = new GroupPrincipal(ctx);
                PrincipalSearcher srch = new PrincipalSearcher(qbeGroup);
                foreach (GroupPrincipal ingrp in srch.FindAll())
                {
                    if (ingrp != null && ingrp.Name.Equals(groupName))
                    {
                        retGroup = ingrp;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return retGroup;
        }

        /// <summary>
        /// 批量添加用户
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="usersName"></param>
        /// <returns></returns>
        public static Result UpdateGroupUsers(UserModel usersName, string groupName = "Users", string ip = null)
        {
            GroupPrincipal qbeGroup = CreateGroup(groupName, false, ip);
            foreach (Principal user in qbeGroup.Members)
            {
                if (user.Name == usersName.Name)
                {
                    return new Result()
                    {
                        Status = false,
                        Message = "账户已存在"
                    };
                }

            }
            return CreateLocalWindowsAccount(usersName.Name, "abc123..", usersName.DisplayName, usersName.Description,
                "Users", false, true, ip);


        }

        /// <summary>
        /// 更改
        /// </summary>
        /// <param name="userName">账户</param>
        /// <param name="displayName"></param>
        /// <param name="passWord">密码</param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool ChangePassword(string userName,string displayName, string passWord, string ip=null)
        {
            try
            {
                GroupPrincipal qbeGroup = CreateGroup("Users", false, ip);
                foreach (Principal user in qbeGroup.Members)
                {

                    if (user?.Name == userName)
                    {
                        PrincipalContext context = new PrincipalContext(ContextType.Machine);
                        UserPrincipal userContext = new UserPrincipal(context);
                        user.DisplayName = displayName;
                        if(!string.IsNullOrEmpty(passWord))
                         userContext.SetPassword(passWord);
                        user.Save();
                        return true;
                    }

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

            }

            return false;
        }
        /// <summary>
        /// 获取服务器Users组的所有用户
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        internal static object GetUsers(string ip)
        {
            try
            {
                var list = new List<UserModel>();
                GroupPrincipal qbeGroup = CreateGroup("Users", false, ip);
                foreach (Principal user in qbeGroup.Members)
                {
                    if (user is UserPrincipal)
                    {
                        list.Add(new UserModel()
                        {
                            Name = user?.Name,
                            DisplayName = user?.DisplayName,
                            Description = user?.Description,

                        });
                    }
                    
                }

                return list;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

            }

            return null;
        }

        internal static bool DeleteUser(string name, string ip)
        {
            try
            {
                GroupPrincipal qbeGroup = CreateGroup("Users", false, ip);
                foreach (Principal user in qbeGroup.Members)
                {
                    if (user?.Name == name)
                    {
                        user.Delete();
                        return true;
                    }

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);

            }

            return false;
        }
    }
    public class UserModel
    {
        public  string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }

        public string PassWord { get; set; }

        public bool IsChangePwd { get; set; }

        public bool IsNever{ get; set; }
    }

    public class Result
    {
        public bool Status { get; set; }
        public string Message { get; set; }
    }
}
