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
    public partial class UserManageForm : Form
    {
        public UserManageForm()
        {
            InitializeComponent();
        }

        private void UserManageForm_Shown(object sender, EventArgs e)
        {
            var svc = JsonConvert.DeserializeObject<IEnumerable<MesService>>(Resources.services);
            dataGridView1.AutoGenerateColumns = false;
            foreach (var s in svc)
            {
                var node = new TreeNode(s.Name);
                node.Tag = "1";
                foreach (var sv in s.Service)
                {
                    node.Nodes.Add(sv);
                }
                node.ExpandAll();
                treeView1.Nodes.Add(node);
            }

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            dataGridView1.DataSource = null;
            if (treeView1.SelectedNode!=null && treeView1.SelectedNode.Tag?.ToString() != "1")
            {
                this.Text ="用户管理：" +treeView1.SelectedNode.Text;
                dataGridView1.DataSource = UserCommons.GetUsers(treeView1.SelectedNode.Text);
            }
        }
        //添加
        private void button1_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null&&treeView1.SelectedNode.Tag?.ToString()!="1")
            {
                new UserForm(m =>
                {
                    var re = UserCommons.UpdateGroupUsers(m, "Users", treeView1.SelectedNode.Text);
                    MessageBox.Show(re.Message); 
                    if (re.Status)
                    {
                        dataGridView1.DataSource = UserCommons.GetUsers(treeView1.SelectedNode.Text);
                    }
                   
                }).ShowDialog();
            }

        }
        //删除
        private void button2_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Tag?.ToString() != "1")
            {
                if (dataGridView1.CurrentRow != null)
                {
                    if (UserCommons.DeleteUser(dataGridView1.CurrentRow.Cells["Column1"]?.Value.ToString(), treeView1.SelectedNode.Text))
                    {
                        MessageBox.Show("删除成功");
                        dataGridView1.DataSource = UserCommons.GetUsers(treeView1.SelectedNode.Text);
                    }
                }
            }
        }
        //更改
        private void button3_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Tag?.ToString() != "1")
                if (dataGridView1.CurrentRow != null)
                {
                    new UserChangeForm(new UserModel()
                    {
                        Name = dataGridView1.CurrentRow.Cells["Column1"]?.Value.ToString(),
                        DisplayName = dataGridView1.CurrentRow.Cells["Column2"]?.Value.ToString(),
                    }, treeView1.SelectedNode.Text).ShowDialog();
                    dataGridView1.DataSource = UserCommons.GetUsers(treeView1.SelectedNode.Text);
                }

        }
        //批量添加
        private void button4_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Tag?.ToString() != "1")
            {
                new UserAddForm(treeView1.SelectedNode.Text).ShowDialog();
            }
        }
        // 刷新
        private void button5_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Tag?.ToString() != "1")
            { 
                dataGridView1.DataSource = UserCommons.GetUsers(treeView1.SelectedNode.Text);
            }
        }
    }
}
