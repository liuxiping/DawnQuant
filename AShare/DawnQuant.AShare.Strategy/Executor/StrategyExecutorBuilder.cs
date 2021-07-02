using DawnQuant.AShare.Repository.Abstract.StrategyMetadata;
using DawnQuant.AShare.Strategy.Descriptor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace DawnQuant.AShare.Strategy.Executor
{
    /// <summary>
    /// 股票策略创建者
    /// </summary>
    public class StrategyExecutorBuilder
    {
        StrategyExecutorInsDescriptor _strategyInsDescriptor;
        IFactorMetadataRepository _factorMetadataRepository;
        ISelectScopeMetadataRepository  _selectScopeMetadataRepository;
     

        public StrategyExecutorBuilder(StrategyExecutorInsDescriptor strategyInsDescriptor,
             IFactorMetadataRepository factorMetadataRepository,
        ISelectScopeMetadataRepository selectScopeMetadataRepository)
        {
            _strategyInsDescriptor = strategyInsDescriptor;
            _factorMetadataRepository = factorMetadataRepository;
            _selectScopeMetadataRepository = selectScopeMetadataRepository;
        }


        /// <summary>
        /// 依赖注入
        /// </summary>
        /// <param name="serviceScope"></param>
        /// <returns></returns>
        public StrategyExecutor Build(IServiceScope  serviceScope )
        {
            StrategyExecutor strategyExecutor = new StrategyExecutor();

            //选股范围
            foreach (SelectScopeExecutorInsDescriptor descriptor in _strategyInsDescriptor.SelectScopeInsDescriptors)
            {

                //选股范围
                // var selectScope = _selectScopeMetadataRepository.Entities.Where(p => p.Id == descriptor.MetadataId).SingleOrDefault();
                 var selectScope = serviceScope.ServiceProvider.
                    GetService<ISelectScopeMetadataRepository>().Entities.
                    Where(p => p.Id == descriptor.MetadataId).SingleOrDefault();

                if (selectScope != null)
                {
                    var cdir = Environment.CurrentDirectory;
                    //获取执行类的名称
                    Assembly asmImpl = Assembly.LoadFrom(selectScope.ImplAssemblyName);
                    Type scopeExecutorType = asmImpl.GetType(selectScope.ImplClassName);

                    ISelectScopeExecutor scopeExecutor=(ISelectScopeExecutor)serviceScope.ServiceProvider.GetService(scopeExecutorType);
                    if (!string.IsNullOrEmpty(descriptor.Parameter))
                    {
                        //反序列化获取参数
                        Assembly asmPara = Assembly.LoadFrom(selectScope.ParameterAssemblyName);
                        Type pType = asmPara.GetType(selectScope.ParameterClassName);
                        scopeExecutor.Parameter = JsonSerializer.Deserialize(descriptor.Parameter, pType);
                    }

                    strategyExecutor.ScopeExecutor.Add(scopeExecutor);
                }
                
            }

            //选择器
            foreach(FactorExecutorInsDescriptor descriptor in _strategyInsDescriptor.FactorInsDescriptors)
            {
                //股票选择器
                // var factor = _factorMetadataRepository.Entities.Where(p => p.Id == descriptor.MetadataId).SingleOrDefault();
                var factor = serviceScope.ServiceProvider.GetService<IFactorMetadataRepository>()
                    .Entities.Where(p => p.Id == descriptor.MetadataId).SingleOrDefault();
                if (factor != null)
                {
                    //获取选择器的类型,初始化选择器
                    Assembly asmImpl = Assembly.LoadFrom(factor.ImplAssemblyName);
                    Type selectorExecutorType = asmImpl.GetType(factor.ImplClassName);
                    IFactorExecutor selectorExecutor = (IFactorExecutor)serviceScope.ServiceProvider.GetService(selectorExecutorType);
                    if (!string.IsNullOrEmpty(descriptor.Parameter))
                    {
                        Assembly asmPara = Assembly.LoadFrom(factor.ParameterAssemblyName);
                        Type pType = asmPara.GetType(factor.ParameterClassName);
                        selectorExecutor.Parameter = JsonSerializer.Deserialize(descriptor.Parameter,pType);
                    }
                    strategyExecutor.FactorExecutor.Add(selectorExecutor);
                }
            }
            return strategyExecutor;
        }
    }
}
