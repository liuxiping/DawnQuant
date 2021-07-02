using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor
{
    /// <summary>
    /// 选股策略执行器
    /// </summary>
    public class StrategyExecutor
    {
        public StrategyExecutor()
        {
            ScopeExecutor = new List<ISelectScopeExecutor>();
            FactorExecutor = new List<IFactorExecutor>();
        }

        /// <summary>
        /// 股票范围
        /// </summary>
        public List<ISelectScopeExecutor> ScopeExecutor { get; set; }

        /// <summary>
        /// 股票策略因子
        /// </summary>
        public List<IFactorExecutor> FactorExecutor { get; set; }

        /// <summary>
        /// 异步执行策略选股
        /// </summary>
        /// <returns></returns>
        public Task<List<string>> ExecuteAsync()
        {
            return Task.Run(() =>
             {
                return Execute();

             });

        }

        /// <summary>
        /// 执行策略选股
        /// </summary>
        /// <returns></returns>
        public List<string> Execute()
        {
            List<string> tscodes = new List<string>();


            if (ScopeExecutor != null && ScopeExecutor.Count > 0)
            {
                //股票选择范围取并集
                foreach (var scope in ScopeExecutor)
                {
                    tscodes = tscodes.Union(scope.Execute()).ToList();
                }

                if (tscodes.Count > 0 && FactorExecutor != null && FactorExecutor.Count > 0)
                {
                    //执行策略选股
                    foreach (var ss in FactorExecutor)
                    {
                        if (tscodes.Count <= 0)
                        {
                            break;
                        }

                        tscodes = ss.Execute(tscodes);
                    }

                }
            }

            return tscodes;
        }
    }
}
