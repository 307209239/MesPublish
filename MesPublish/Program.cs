using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MesPublish
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //UserCommons.UpdateGroupUsers(new List<string>() {"LA01", "LA02"},"Users","172.22.71.168");

            //UserCommons.ChangePassword("LA01", "abc123...", "172.22.71.168");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ServiceForm());
        }
    }
}
