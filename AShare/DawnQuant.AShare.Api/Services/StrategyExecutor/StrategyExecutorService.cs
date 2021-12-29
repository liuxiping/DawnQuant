using AutoMapper;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using DawnQuant.AShare.Repository.Abstract.StrategyMetadata;
using DawnQuant.AShare.Strategy.Executor;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using DawnQuant.AShare.Strategy.Descriptor;
using DawnQuant.AShare.Repository.Abstract.UserProfile;
using SE = DawnQuant.AShare.Strategy.Executor;

namespace DawnQuant.AShare.Api.StrategyExecutor
{
    public class StrategyExecutorService : StrategyExecutorApi.StrategyExecutorApiBase
    {

        IFactorMetadataRepository _factorRepository;
        ISelectScopeMetadataRepository _stockSelectScopeRepository;
        IStockStrategyRepository _stockStrategyRepository;

        IIndustryRepository _industryRepository;
        IBasicStockInfoRepository _basicStockInfoRepository;


        IServiceProvider _serviceProvider;
        FinalStrategyExecutor _finalStrategyExecutor;

        public StrategyExecutorService(
        IFactorMetadataRepository factorRepository,IStockStrategyRepository stockStrategyRepository,
        ISelectScopeMetadataRepository stockSelectScopeRepository, IServiceProvider serviceProvider, 
        IIndustryRepository industryRepository, IBasicStockInfoRepository basicStockInfoRepository)
        {
            _factorRepository = factorRepository;
            _stockSelectScopeRepository = stockSelectScopeRepository;
            _serviceProvider = serviceProvider;
          
            _industryRepository = industryRepository;
            _basicStockInfoRepository = basicStockInfoRepository;

            _stockStrategyRepository = stockStrategyRepository;

            _finalStrategyExecutor = new FinalStrategyExecutor(
                _factorRepository, _stockSelectScopeRepository,_stockStrategyRepository,
                _serviceProvider);

    }

        /// <summary>
        /// 执行单个策略
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<ExecuteStrategyResponse> ExecuteStrategyByContent(ExecuteStrategyByContentRequest request, ServerCallContext context)
        {
            return Task.Run((() =>
            {
                var strategyContent = request.StrategyContent;

                var re=_finalStrategyExecutor.ExecuteStrategyByContent(strategyContent);

                 ExecuteStrategyResponse response = new ExecuteStrategyResponse();
                if (re != null && re.Count > 0)
                {
                    foreach (var tscode in re)
                    {
                        var e = _basicStockInfoRepository.Entities.Where(p => p.TSCode == tscode && !p.StockName.Contains("退")).FirstOrDefault();

                        if (e != null)
                        {
                            string indus = _industryRepository.Entities.Where(p => p.Id == e.IndustryId).Select(p => p.Name).SingleOrDefault();

                            response.Entities.Add(new ExecuteStrategyResult { TSCode = e.TSCode, Name = e.StockName, Industry = indus ?? "" });
                        }
                    }
                }
                return response;
            }));
        }

        public override Task<ExecuteStrategyResponse> ExecuteStrategyById(ExecuteStrategyByIdRequest request, ServerCallContext context)
        {
            return Task.Run((() =>
            {

                var re = _finalStrategyExecutor.ExecuteStrategyById(request.StrategyId);

                ExecuteStrategyResponse response = new ExecuteStrategyResponse();
                if (re != null && re.Count > 0)
                {
                    foreach (var tscode in re)
                    {
                        var e = _basicStockInfoRepository.Entities.Where(p => p.TSCode == tscode && !p.StockName.Contains("退")).FirstOrDefault();

                        if (e != null)
                        {
                            string indus = _industryRepository.Entities.Where(p => p.Id == e.IndustryId).Select(p => p.Name).SingleOrDefault();
                            response.Entities.Add(new ExecuteStrategyResult { TSCode = e.TSCode, Name = e.StockName, Industry = indus ?? "" });
                        }
                    }
                }
                return response;
            }));
        }
    }
}
