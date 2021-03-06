﻿using System;
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
    public partial class UserForm : Form
    {
        private Action<UserModel> _action;
        public UserForm(Action<UserModel> action)
        {
            InitializeComponent();
            this._action = action;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this._action(new UserModel()
            {
                Name = txtUser.Text,
                DisplayName = txtDescription.Text
                
            });
            this.Close();
        }
    }
}
