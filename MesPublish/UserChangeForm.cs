using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MesPublish
{
    public partial class UserChangeForm : Form
    {
        public UserChangeForm(UserModel model,string ip)
        {
            InitializeComponent();

            txtUser.Text = model.Name;
            txtPassWord.Text = model.PassWord;
            txtDescription.Text = model.DisplayName;
            _ip = ip;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string _ip;
        private void button1_Click(object sender, EventArgs e)
        {
            if (UserCommons.ChangePassword(txtUser.Text,txtDescription.Text,txtPassWord.Text,_ip))
            {
                MessageBox.Show("更改成功");
                this.Close();
            }
        }
    }
}
