using System;
using System.Collections.Generic;
using System.Text;

namespace DawnQuant.AShare.Strategy.Executor
{

    /// <summary>
    /// 股票选择范围执行器
    /// </summary>
    public interface ISelectScopeExecutor
    {
        /// <summary>
        /// 返回股票TSCode 
        /// </summary>
        /// <returns></returns>
        List<string> Execute();


        public object Parameter { get; set; }

    }
}
