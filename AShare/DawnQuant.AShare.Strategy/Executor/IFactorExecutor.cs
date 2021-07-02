using System;
using System.Collections.Generic;
using System.Text;

namespace DawnQuant.AShare.Strategy.Executor
{
    /// <summary>
    /// 选股因子执行器
    /// </summary>
    public interface IFactorExecutor
    {

        /// <summary>
        /// 执行选择
        /// </summary>
        /// <param name="tsCodes"></param>
        /// <returns></returns>
        List<string> Execute(List<string> tsCodes);

        public object Parameter { get; set; }

    }

  
}
