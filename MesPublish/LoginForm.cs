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
    public partial class LoginForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        private Action<string, string, string> _action;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public LoginForm(Action<string, string, string> action)
        {
            InitializeComponent();
            this._action = action;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _action(txtUserName.Text, txtPassWord.Text, cbDomain.Text);
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
    }
}
