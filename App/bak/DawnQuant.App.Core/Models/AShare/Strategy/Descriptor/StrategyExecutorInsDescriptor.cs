﻿

namespace DawnQuant.App.Core.Models.AShare.Strategy.Descriptor
{
    /// <summary>
    /// 股票策略的描述
    /// </summary>
   
    public class StrategyExecutorInsDescriptor
    {

        public StrategyExecutorInsDescriptor()
        {
            FactorInsDescriptors = new List<FactorExecutorInsDescriptor>();
            SelectScopeInsDescriptors = new List<SelectScopeExecutorInsDescriptor>();
        }

        public List<FactorExecutorInsDescriptor> FactorInsDescriptors { get; set; }

        
        public List<SelectScopeExecutorInsDescriptor> SelectScopeInsDescriptors { get; set; }


       
    }
}
