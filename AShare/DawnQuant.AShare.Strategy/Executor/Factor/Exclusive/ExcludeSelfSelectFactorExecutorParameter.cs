using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{

    /// <summary>
    /// 排除自选分类
    /// </summary>
    public class ExcludeSelfSelectFactorExecutorParameter
    {
        public List<long> ExcludeCategories { get; set; }
    }

       


}
