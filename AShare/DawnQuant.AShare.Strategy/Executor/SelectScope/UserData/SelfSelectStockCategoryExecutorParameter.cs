
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.SelectScope
{
    /// <summary>
    /// 自选分类
    /// </summary>
   
    public class SelfSelectStockCategoryExecutorParameter 
    {

        /// <summary>
        /// 支持的分类
        /// </summary>
        public List<long> SupportedCategories { get; set; }

    }



   
}
