
using DawnQuant.AShare.Entities.StrategyMetadata;
using DawnQuant.AShare.Repository.Abstract.StrategyMetadata;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Api.Data
{
    public static class SeedData
    {

        /// <summary>
        /// 选股范围基础数据
        /// </summary>
        public static void InitStockStrategyMetadata(IApplicationBuilder app)
        {
            InitSelectScopeMetadata(app);
            InitFactorMetadata(app);
        }

        public static void InitSelectScopeMetadata(IApplicationBuilder app)
        {
            //分类基础数据
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var categoryRepository = scope.ServiceProvider.GetService<ISelectScopeMetadataCategoryRepository>();
                var scoper = scope.ServiceProvider.GetService<ISelectScopeMetadataRepository>();

                //没有数据加入基础数据
                if (!categoryRepository.Entities.Any())
                {
                    #region 市场类型
                    var aShare = categoryRepository.Insert(new SelectScopeMetadataCategory
                    {
                        Name = "市场类型",
                        SortNum = 1
                    });
                    scoper.Insert(new SelectScopeMetadata
                    {
                        CategoryId = aShare.Id,
                        Name = "沪深股市所有股票",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.SelectScope.AllMarketStocksExecutor",
                        SortNum=1
                    });

                    scoper.Insert(new SelectScopeMetadata
                    {
                        CategoryId = aShare.Id,
                        Name = "主板",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.SelectScope.MainBoardMarketStocksExecutor",
                        SortNum = 2
                    });

                    scoper.Insert(new SelectScopeMetadata
                    {
                        CategoryId = aShare.Id,
                        Name = "中小板",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.SelectScope.SAMBoardMarketStocksExecutor",
                        SortNum = 3
                    });

                    scoper.Insert(new SelectScopeMetadata
                    {
                        CategoryId = aShare.Id,
                        Name = "创业板",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.SelectScope.GEMBoardMarketStocksExecutor",
                        SortNum = 4
                    });

                    scoper.Insert(new SelectScopeMetadata
                    {
                        CategoryId = aShare.Id,
                        Name = "科创板",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.SelectScope.StarBoardMarketStocksExecutor",
                        SortNum = 5
                    });

                    #endregion
                    var self = categoryRepository.Insert(new SelectScopeMetadataCategory
                    {
                        Name = "自选股分类",
                        SortNum = 2
                    });

                    scoper.Insert(new SelectScopeMetadata
                    {
                        CategoryId = self.Id,
                        Name = "自选股分类",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.SelectScope.SelfSelectStockCategoryExecutor",
                        ParameterAssemblyName= "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName= "DawnQuant.AShare.Strategy.Executor.SelectScope.SelfSelectStockCategoryExecutorParameter",
                        SortNum = 1

                    });

                    var index = categoryRepository.Insert(new SelectScopeMetadataCategory
                    {
                        Name = "指数",
                        SortNum = 3
                    });
                  
                }
            }
        }


        /// <summary>
        /// 选股因子
        /// </summary>
        /// <param name="app"></param>
        public static void InitFactorMetadata(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var factorMetadataCategoryRepository = scope.ServiceProvider.GetService<IFactorMetadataCategoryRepository>();
                var factorMetadataRepository = scope.ServiceProvider.GetService<IFactorMetadataRepository>();


                if (!factorMetadataCategoryRepository.Entities.Any())
                {
                    var f = factorMetadataCategoryRepository.Insert(new FactorMetadataCategory { Name = "财务指标", SortNum = 1 });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        CategoryId = f.Id,
                        Name = "市值因子",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.MarketValueFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.MarketValueFactorExecutorParameter",
                        SortNum = 1
                    });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        CategoryId = f.Id,
                        Name = "市盈率因子",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.PEFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.PEFactorExecutorParameter",
                        SortNum = 2,
                    });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        CategoryId = f.Id,
                        Name = "市净率因子",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.PBFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.PBFactorExecutorParameter",
                        SortNum = 3,
                    });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        CategoryId = f.Id,
                        Name = "市销率因子",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.PSFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.PSFactorExecutorParameter",
                        SortNum = 4,
                    });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        CategoryId = f.Id,
                        Name = "股息率因子",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.DVFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.DVFactorExecutorParameter",
                        SortNum = 5,
                    });

                    var ti = factorMetadataCategoryRepository.Insert(new FactorMetadataCategory { Name = "技术指标", SortNum = 2 });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        CategoryId = ti.Id,
                        Name = "均线多头排列因子",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.SMABullFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.SMABullFactorExecutorParameter",
                        SortNum = 1

                    });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        CategoryId = ti.Id,
                        Name = "均线附近因子",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.NearTheSMAFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.NearTheSMAFactorExecutorParameter",
                        SortNum = 2
                    });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        CategoryId = ti.Id,
                        Name = "均线之上因子",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.AboveSMAFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.AboveSMAFactorExecutorParameter",
                        SortNum = 3
                    });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        CategoryId = ti.Id,
                        Name = "均线之下因子",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.BelowSMAFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.BelowSMAFactorExecutorParameter",
                        SortNum = 4
                    });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        CategoryId = ti.Id,
                        Name = "振幅因子",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.AMFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.AMFactorExecutorParameter",
                        SortNum = 5
                    });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        CategoryId = ti.Id,
                        Name = "涨幅因子",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.GainFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.GainFactorExecutorParameter",
                        SortNum = 6
                    });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        CategoryId = ti.Id,
                        Name = "缩量因子(对比前一个交易周期)",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.VolReductionFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.VolReductionFactorExecutorParameter",
                        SortNum = 7
                    });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        CategoryId = ti.Id,
                        Name = "形态因子",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.CorrelationFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.CorrelationFactorExecutorParameter",
                        SortNum = 8
                    });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        CategoryId = ti.Id,
                        Name = "涨停因子",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.HardenFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.HardenFactorExecutorParameter",
                        SortNum = 9
                    });


                    var ex = factorMetadataCategoryRepository.Insert(new FactorMetadataCategory { Name = "排除指标", SortNum = 3 });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        CategoryId = ex.Id,
                        Name = "排除ST股",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.ExcludeSTFactorExecutor",
                        SortNum = 1
                    });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        CategoryId = ex.Id,
                        Name = "排除自选分类",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.ExcludeSelfSelectFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.ExcludeSelfSelectFactorExecutorParameter",
                        SortNum = 2
                    });
                }
            }
        }
    }
    
}
