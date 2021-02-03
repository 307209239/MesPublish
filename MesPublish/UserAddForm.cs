using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MesPublish
{
    public partial class UserAddForm : Form
    {
        private string _ip;
        public UserAddForm(string ip)
        {
            InitializeComponent();
            this._ip = ip;
        }
        //关闭
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //导入
        private void button1_Click(object sender, EventArgs e)
        {
            var op = new OpenFileDialog()
            {
                Filter = "txt|*.txt"
            };
            if (DialogResult.OK == op.ShowDialog())
            {
               var s= File.ReadLines( op.FileName,Encoding.UTF8).ToList();
               for (int i = 1; i < s.Count(); i++)
               {
                   var data = s[i].Split(',');
                   if (data.Length>1)
                   {
                       dataGridView1.Rows.Add(data[0], data[1]);
                   }
                   
               }
              
            }
        }
        //下载模板
        private void button2_Click(object sender, EventArgs e)
        {
            var op = new SaveFileDialog()
            {
                Filter = "txt|*.txt"
            };
            if (DialogResult.OK ==op.ShowDialog())
            {
                File.Copy("模板.txt", op.FileName);
                Process.Start(op.FileName);
            }
        }
        //添加账户
        private void button3_Click(object sender, EventArgs e)
        {
           
           
            new Action(() =>
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                   var model=new UserModel()
                    {
                        Name = row.Cells["Column1"].Value?.ToString(),
                        DisplayName = row.Cells["Column1"].Value?.ToString(),
                    };
                   var re = UserCommons.UpdateGroupUsers(model, "Users", _ip);
                
                       SetRow(row,re.Message, re.Status);
                  
                }

            }).BeginInvoke(null, null);
            
        }

        private void SetRow(DataGridViewRow row,string message,bool isError)
        {
            dataGridView1.Invoke(new Action(() =>
            {
                row.Cells["Column3"].Value = message;
                row.DefaultCellStyle.BackColor =isError?Color.Green: Color.Red;
            }));
        }
    }
}
