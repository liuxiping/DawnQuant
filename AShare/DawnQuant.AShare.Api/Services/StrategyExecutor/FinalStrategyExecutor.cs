using DawnQuant.AShare.Api.StrategyExecutor;
using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using DawnQuant.AShare.Repository.Abstract.StrategyMetadata;
using DawnQuant.AShare.Repository.Abstract.UserProfile;
using DawnQuant.AShare.Strategy.Descriptor;
using DawnQuant.AShare.Strategy.Executor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SE = DawnQuant.AShare.Strategy.Executor;


namespace DawnQuant.AShare.Api.StrategyExecutor
{

    /// <summary>
    /// 策略执行者
    /// </summary>
    public class FinalStrategyExecutor
    {
        IFactorMetadataRepository _factorRepository;
        ISelectScopeMetadataRepository _selectScopeMetadataRepository;
        IStockStrategyRepository _stockStrategyRepository;

        IServiceProvider _serviceProvider;


        public FinalStrategyExecutor(IFactorMetadataRepository FactorRepository,
        ISelectScopeMetadataRepository stockSelectScopeRepository, 
        IStockStrategyRepository stockStrategyRepository, IServiceProvider serviceProvider)
        {
            _factorRepository = FactorRepository;
            _selectScopeMetadataRepository = stockSelectScopeRepository;
            _serviceProvider = serviceProvider;
            _stockStrategyRepository = stockStrategyRepository;

        }


        /// <summary>
        /// 根据内容执行策略
        /// </summary>
        /// <param name="strategyContent"></param>
        /// <returns></returns>
        public List<string> ExecuteStrategyByContent(string strategyContent)
        {
            var descriptor = JsonSerializer.Deserialize<StrategyExecutorInsDescriptor>(strategyContent);

            StrategyExecutorBuilder builder = new StrategyExecutorBuilder(descriptor,
                 _factorRepository, _selectScopeMetadataRepository);
            using (var scope = _serviceProvider.CreateScope())
            {
                SE.StrategyExecutor executor = builder.Build(scope);
                var re = executor.Execute();
                return re;
            }
        }


        /// <summary>
        /// 根据ID执行策略
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public List<string> ExecuteStrategyById(long Id)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var stockStrategyRepository = scope.ServiceProvider.GetService<IStockStrategyRepository>();
                var ss = stockStrategyRepository.Entities.Where(p => p.Id == Id).FirstOrDefault();
                if (ss != null)
                {
                    return ExecuteStrategyByContent(ss.StockStragyContent);
                }
                else
                {
                    return null;
                }
            }

        }
    }
}
