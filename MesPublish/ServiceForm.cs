using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MesPublish.Properties;
using Newtonsoft.Json;

namespace MesPublish
{
    public partial class ServiceForm : Form
    {
        public ServiceForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 重启服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                var node = treeView1.SelectedNode;
                if (node.Tag?.ToString() == "IP")
                {
                    Service.RestartOpSystem(node.Text);
                }
            }
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                var node = treeView1.SelectedNode;
                if (node.Tag?.ToString() == "SVC")
                {
                    Service.StartService(node.Text, node.Parent.Text);
                    ServiceForm_Shown(null, null);
                }
            }
        }
        private Dictionary<string,User> Users=new Dictionary<string, User>();
        private void ServiceForm_Shown(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            var services = new[] { "InSite Server", "Camstar Security Server", "CamstarSecurityLMServer" };
            var svc = JsonConvert.DeserializeObject<IEnumerable<MesService>>(Resources.services);
            foreach (var s in svc)
            {
                var node = new TreeNode(s.Name);
                foreach (var sv in s.Service)
                {
                    var cn = new TreeNode(sv);
                    cn.Tag = "IP";//服务器标识
                    var isAlive = Service.PingIp(sv);
                    cn.ContextMenuStrip = contextMenuStrip2;
                   
                    cn.ForeColor = isAlive ? Color.ForestGreen : Color.Red;//检查服务器是否存活
                    if (isAlive)
                    {
                        //Users.Add(sv, new User(sv));
                        var svcs= Service.GetSvc(sv);
                        foreach (var item in services)
                        {
                            var sn = new TreeNode(item);
                            sn.Tag = "SVC";//服务器标识
                            sn.ContextMenuStrip = contextMenuStrip1;
                            sn.ForeColor = svcs.Any(m=>m==item) ? Color.ForestGreen : Color.Red;//检查服务器是否存活
                            cn.Nodes.Add(sn);
                        }
                        node.Nodes.Add(cn);
                    }
                    
                }
                node.ExpandAll();
                treeView1.Nodes.Add(node);
            }
            

        }

        private void button5_Click(object sender, EventArgs e)
        {
            ServiceForm_Shown(null, null);
        }
        /// <summary>
        /// 重启服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode!=null)
            {
                var node = treeView1.SelectedNode;
                if (node.Tag?.ToString()=="SVC")
                {
                    Service.RestartService(node.Text, node.Parent.Text);
                    ServiceForm_Shown(null,null);
                }
            }
        }
        /// <summary>
        /// 关闭服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                var node = treeView1.SelectedNode;
                if (node.Tag?.ToString() == "SVC")
                {
                    Service.StopService(node.Text, node.Parent.Text);
                    ServiceForm_Shown(null, null);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            new Form1().ShowDialog();
        }

        private void 停止服务ToolStripMenuItem_Click(object sender, EventArgs e)
        {

           
        }

        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
           
        }

       
    }
}
