using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesPublish
{
    

    public class MesService
    {
        /// <summary>
        /// 环境名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 服务器IP集合
        /// </summary>
        public string[] Service { get; set; }
    }
    
}
