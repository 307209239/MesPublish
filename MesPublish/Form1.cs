using MesPublish.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MesPublish
{
    public partial class Form1 : Form
    {
       
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
           
            var svc = JsonConvert.DeserializeObject<IEnumerable<MesService>>(Resources.services);

            foreach (var s in svc)
            {
                var node = new TreeNode(s.Name);
                foreach (var sv in s.Service)
                {
                    node.Nodes.Add(sv);
                    Users.Add(sv,new User(sv));
                }
                node.ExpandAll();
                treeView1.Nodes.Add(node);
            }

            //new Action(() =>
            //{
            //    for (int i = 0; i < 10000; i++)
            //    {
            //        ShowMessage(Guid.NewGuid().ToString("N"));
            //    }
            //}).BeginInvoke(null,null);

        }
        /// <summary>
        /// 添加MDB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "MDB|*.mdb";
            op.Multiselect = false;
            if (DialogResult.OK == op.ShowDialog())
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[0].Value.ToString() == "MDB")
                    {
                        ShowMessage("只能添加一个MDB文件", true);
                        return;
                    }
                }
                dataGridView1.Rows.Add("MDB", op.FileName, "删除");
            }
        }
        /// <summary>
        /// 添加文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Filter = "*|*.*";
            op.Multiselect = true;
            if (DialogResult.OK == op.ShowDialog())
            {

                foreach (var f in op.FileNames)
                {
                    if (!f.Contains("Camstar Portal")||!f.StartsWith(@"\\"))
                    {
                        ShowMessage($"{f}错误，只支持远程服务器Camstar Portal文件夹下的文件", true);
                        continue;
                    }
                    var isAdd = false;
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (Path.GetFileName(row.Cells[1].Value.ToString()) == Path.GetFileName(f))
                        {
                            ShowMessage($"{Path.GetFileName(f)}文件已存在", true);
                            isAdd = true;
                            break;
                        }
                    }

                    if (!isAdd)
                    {
                        dataGridView1.Rows.Add("FILE", f, "删除");
                    }

                }
            }

        }
        /// <summary>
        /// 删除列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                dataGridView1.Rows.RemoveAt(e.RowIndex);
            }
        }

        /// <summary>
        /// 显示信息
        /// </summary>
        /// <param name="s"></param>
        /// <param name="isError"></param>
        private void ShowMessage(string s, bool isError = false)
        {
            richTextBox1.Invoke(new Action<string>(a =>
            {
                richTextBox1.SelectionColor = isError ? Color.Red : Color.Black;
                richTextBox1.AppendText(s + "\n");
            }), s);
        }
        /// <summary>
        /// 备份MDB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            var svc = GetSelectNode();
            new Action(() =>
            {
                foreach (var s in svc)
                {
                    if (CheckIpcConnect(s))
                    {
                        BackupMdb(s);

                    }
                }

            }).BeginInvoke(null, null);

        }
        /// <summary>
        /// 备份MDB文件
        /// </summary>
        /// <param name="ip"></param>
        private void BackupMdb(string ip)
        {
            ShowMessage($"备份 {ip} MDB文件");
            try
            {
                if (Directory.Exists($@"\\{ip}\\d$\"))
                {
                    File.Copy($@"\\{ip}\c$\Program Files (x86)\Camstar\InSite Administration\InSiteSemi.mdb", $@"\\{ip}\d$\backMDB\InSiteSemi-{DateTime.Now:yyyyMMddHHmm}.mdb", true);
                }
                else
                {
                    File.Copy($@"\\{ip}\c$\Program Files (x86)\Camstar\InSite Administration\InSiteSemi.mdb", $@"\\{ip}\e$\backMDB\InSiteSemi-{DateTime.Now:yyyyMMddHHmm}.mdb", true);

                }
                ShowMessage($"备份 {ip} MDB文件完成！");

            }
            catch (Exception e)
            {
                ShowMessage($"备份 {ip} MDB文件失败：{e.Message}", true);
            }


        }

        /// <summary>
        /// 获取选中的服务器
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> GetSelectNode()
        {
            return from TreeNode n in treeView1.Nodes from TreeNode n1 in n.Nodes where n1.Checked select n1.Text;
        }
        /// <summary>
        /// 检查IPC连接
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private bool CheckIpcConnect(string ip)
        {
            ShowMessage($"检查{ip}共享目录权限");
            if (!Directory.Exists($@"\\{ip}\\c$\"))
            {
                if (DialogResult.Yes == new LoginForm((U, P, D) =>
                {
                    Service.UserName = U;
                    Service.PassWord = P;
                    Service.Domain = D;
                }).ShowDialog())
                {
                    return AddIpcConnect(ip);
                }

            }
            return true;
        }

        /// <summary>
        /// 添加IPC连接
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private bool AddIpcConnect(string ip)
        {
            Process.Start("net", $@"use \\{ip}\ipc$ {Service.PassWord} /user:{Service.Domain}\{Service.UserName}");
            if (!CheckIpcConnect(ip))
            {
                ShowMessage($"添加{ip}共享目录权限失败", true);
                return false;
            }

            return true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            var svc = GetSelectNode();
            if (svc.Any())
            {
                if (dataGridView1.RowCount > 0)
                {
                    var mdbFile = "";
                    var files = new List<string>();
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells[0].Value.ToString() == "MDB")
                        {
                            mdbFile = row.Cells[1].Value.ToString();
                        }
                        else
                        {
                            files.Add(row.Cells[1].Value.ToString());
                        }
                    }
                    foreach (var s in svc)
                    {
                        if (!string.IsNullOrEmpty(mdbFile))
                        {
                            //备份MDB
                            BackupMdb(s);
                            //更新MDB
                            UpdateMdb(mdbFile, s);
                        }

                        if (files.Any())
                        {
                            foreach (var f in files)
                            {
                                //备份文件
                                BackupFile(f, s);
                                //更新文件
                                UpdateFile(f, s);
                            }

                        }
                    }

                }
                else
                {
                    ShowMessage("没有需要更新数据", true);
                }

            }
            else
            {
                ShowMessage("请选择服务器", true);
            }
        }
        /// <summary>
        /// 更新文件
        /// </summary>
        /// <param name="f"></param>
        /// <param name="ip"></param>
        private void UpdateFile(string f, string ip)
        {
            try
            {
                var oldFile = $@"\\{ip}{f.Substring(f.IndexOf('\\', 2))}";
                File.Copy(f, oldFile, true);
                ShowMessage($"更新 {ip} 文件完成");
            }
            catch (Exception e)
            {
                ShowMessage($"更新 {ip} 文件失败：{e.Message}");
            }
           
        }
        private Dictionary<string, User> Users = new Dictionary<string, User>(); 
        /// <summary>
        /// 备份文件
        /// </summary>
        /// <param name="f"></param>
        /// <param name="ip"></param>
        private void BackupFile(string f, string ip)
        {
            var oldFile = $@"\\{ip}{f.Substring(f.IndexOf('\\', 2))}";
            var oldDir = DateTime.Now.ToString("yyyyMMddHHmm") + "-back";
            var newDir = DateTime.Now.ToString("yyyyMMddHHmm") + "-new";
            ShowMessage($"备份 {ip}文件");
            try
            {
                if (Directory.Exists($@"\\{ip}\\d$\"))
                {
                    //备份旧文件
                    if (File.Exists(oldFile))
                    {
                        if (!Directory.Exists($@"\\{ip}\\d$\updateFiles\{oldDir}"))
                        {
                            Directory.CreateDirectory($@"\\{ip}\\d$\updateFiles\{oldDir}");
                        }
                        File.Copy(oldFile, $@"\\{ip}\d$\updateFiles\{oldDir}\{Path.GetFileName(f)}", true);
                    }
                    //备份新文件
                    if (!Directory.Exists($@"\\{ip}\\d$\updateFiles\{newDir}"))
                    {
                        Directory.CreateDirectory($@"\\{ip}\\d$\updateFiles\{newDir}");
                    }
                    File.Copy(f, $@"\\{ip}\d$\updateFiles\{newDir}\{Path.GetFileName(f)}", true);
                }
                else
                {
                    //备份旧文件
                    if (File.Exists(oldFile))
                    {
                        if (!Directory.Exists($@"\\{ip}\\e$\updateFiles\{oldDir}"))
                        {
                            Directory.CreateDirectory($@"\\{ip}\\e$\updateFiles\{oldDir}");
                        }
                        File.Copy(oldFile, $@"\\{ip}\e$\updateFiles\{oldDir}\{Path.GetFileName(f)}", true);
                    }
                    //备份新文件
                    if (!Directory.Exists($@"\\{ip}\\e$\updateFiles\{newDir}"))
                    {
                        Directory.CreateDirectory($@"\\{ip}\\e$\updateFiles\{newDir}");
                    }
                    File.Copy(f, $@"\\{ip}\e$\updateFiles\{newDir}\{Path.GetFileName(f)}", true);

                }
                ShowMessage($"备份 {ip} 文件完成！");

            }
            catch (Exception e)
            {
                ShowMessage($"备份 {ip} 文件失败：{e.Message}", true);
            }
        }

        /// <summary>
        /// 更新MDB
        /// </summary>
        /// <param name="file"></param>
        /// <param name="ip"></param>
        private void UpdateMdb(string file, string ip)
        {
            ShowMessage($"更新 {ip} MDB文件");
            try
            {

                File.Copy(file, $@"\\{ip}\c$\Program Files (x86)\Camstar\InSite Administration\InSiteSemi.mdb", true);
            }
            catch (Exception e)
            {
                ShowMessage($"更新 {ip} MDB文件失败：{e.Message}", true);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            new ServiceForm().Show();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            DirectoryEntry localMachine = new DirectoryEntry("WinNT://" + Environment.MachineName + ",Computer");
            var group = localMachine.Children.Find("Users", "group");
            var user = localMachine.Children.Find("Administrator", "user");
            localMachine.CommitChanges();
            localMachine.Close();
            new UserForm((u, p, d) =>
            {
                foreach (var ip in GetSelectNode())
                {
                    if (Users[ip]!=null)
                    {
                        var s=Users[ip].ViweUsers();
                        Users[ip].AddUserByCmd(u, p, d);
                    }
                }
            }).ShowDialog();
        }
    }


}
