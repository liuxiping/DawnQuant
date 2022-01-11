
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
    /// 题材热点
    /// </summary>
   
    public class SubjectAndHotExecutorParameter
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        public string UserName { get; set; }

        public bool OnlyFocus { get; set; }

    }



   
}
