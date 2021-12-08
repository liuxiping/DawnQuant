using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Models.AShare.Strategy.Executor.Common
{
    public class SupportedCategory
    {
        /// <summary>
        /// 分类
        /// </summary>
        public long CategoryId { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
