﻿
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
                        Id=1,
                        Name = "市场类型",
                        SortNum = 1
                    });
                    scoper.Insert(new SelectScopeMetadata
                    {
                        Id=1001,
                        CategoryId = aShare.Id,
                        Name = "沪深股市所有股票",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.SelectScope.AllMarketStocksExecutor",
                        SortNum=1
                    });

                    scoper.Insert(new SelectScopeMetadata
                    {
                        Id = 1002,
                        CategoryId = aShare.Id,
                        Name = "主板",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.SelectScope.MainBoardMarketStocksExecutor",
                        SortNum = 2
                    });

                    scoper.Insert(new SelectScopeMetadata
                    {
                        Id = 1003,
                        CategoryId = aShare.Id,
                        Name = "中小板",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.SelectScope.SAMBoardMarketStocksExecutor",
                        SortNum = 3
                    });

                    scoper.Insert(new SelectScopeMetadata
                    {
                        Id = 1004,
                        CategoryId = aShare.Id,
                        Name = "创业板",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.SelectScope.GEMBoardMarketStocksExecutor",
                        SortNum = 4
                    });

                    scoper.Insert(new SelectScopeMetadata
                    {
                        Id = 1005,
                        CategoryId = aShare.Id,
                        Name = "科创板",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.SelectScope.StarBoardMarketStocksExecutor",
                        SortNum = 5
                    });

                    #endregion

                    #region
                    var self = categoryRepository.Insert(new SelectScopeMetadataCategory
                    {
                        Id=2,
                        Name = "用户数据",
                        SortNum = 2
                    });
                    scoper.Insert(new SelectScopeMetadata
                    {
                        Id = 2001,
                        CategoryId = self.Id,
                        Name = "自选股分类",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.SelectScope.SelfSelectStockCategoryExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.SelectScope.SelfSelectStockCategoryExecutorParameter",
                        SortNum = 1

                    });

                    scoper.Insert(new SelectScopeMetadata
                    {
                        Id = 2002,
                        CategoryId = self.Id,
                        Name = "龙头股",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.SelectScope.BellwetherExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.SelectScope.BellwetherExecutorParameter",

                        SortNum = 2

                    });

                    scoper.Insert(new SelectScopeMetadata
                    {
                        Id = 2003,
                        CategoryId = self.Id,
                        Name = "题材热点",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.SelectScope.SubjectAndHotExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.SelectScope.SubjectAndHotExecutorParameter",

                        SortNum = 3

                    });
                    #endregion

                    var index = categoryRepository.Insert(new SelectScopeMetadataCategory
                    {
                        Id=3,
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
                    //财务指标
                    var f = factorMetadataCategoryRepository.Insert(new FactorMetadataCategory
                        {  Id=1, Name = "财务指标", SortNum = 1 });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        Id=1001,
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
                        Id = 1002,
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
                        Id = 1003,
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
                        Id = 1004,
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
                        Id = 1005,
                        CategoryId = f.Id,
                        Name = "股息率因子",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.DVFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.DVFactorExecutorParameter",
                        SortNum = 5,
                    });


                    //技术指标
                    var ti = factorMetadataCategoryRepository.Insert(new FactorMetadataCategory 
                    { Id=2, Name = "技术指标", SortNum = 2 });
                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        Id=2001,
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
                        Id = 2002,
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
                        Id = 2003,
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
                        Id = 2004,
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
                        Id = 2005,
                        CategoryId = ti.Id,
                        Name = "涨幅因子",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.GainFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.GainFactorExecutorParameter",
                        SortNum = 5
                    });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        Id = 2006,
                        CategoryId = ti.Id,
                        Name = "涨停因子",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.HardenFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.HardenFactorExecutorParameter",
                        SortNum = 6
                    });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        Id = 2007,
                        CategoryId = ti.Id,
                        Name = "振幅因子",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.AMFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.AMFactorExecutorParameter",
                        SortNum = 7
                    });

                   

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        Id = 2008,
                        CategoryId = ti.Id,
                        Name = "缩量因子(对比前一个交易周期)",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.VolReductionFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.VolReductionFactorExecutorParameter",
                        SortNum = 8
                    });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        Id = 2009,
                        CategoryId = ti.Id,
                        Name = "形态因子",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.CorrelationFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.CorrelationFactorExecutorParameter",
                        SortNum = 9
                    });


                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        Id = 2010,
                        CategoryId = ti.Id,
                        Name = "锤子线",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.HammerFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.HammerFactorExecutorParameter",
                        SortNum = 10
                    });

                    //主力指标
                    var mainForce = factorMetadataCategoryRepository.Insert(new FactorMetadataCategory
                    { Id = 3, Name = "主力指标", SortNum = 3 });

                    //排除指标
                    var ex = factorMetadataCategoryRepository.Insert(new FactorMetadataCategory 
                    {  Id=4,Name = "排除指标", SortNum = 4 });
                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        Id = 4001,
                        CategoryId = ex.Id,
                        Name = "排除ST股",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.ExcludeSTFactorExecutor",
                        SortNum = 1
                    });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        Id = 4002,
                        CategoryId = ex.Id,
                        Name = "排除自选分类",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.ExcludeSelfSelectFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.ExcludeSelfSelectFactorExecutorParameter",
                        SortNum = 2
                    });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        Id = 4003,
                        CategoryId = ex.Id,
                        Name = "排除市场龙头",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.BellwetherFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.BellwetherFactorExecutorParameter",
                        SortNum = 3
                    });

                    factorMetadataRepository.Insert(new FactorMetadata
                    {
                        Id = 4004,
                        CategoryId = ex.Id,
                        Name = "排除题材热点",
                        ImplAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ImplClassName = "DawnQuant.AShare.Strategy.Executor.Factor.SubjectAndHotFactorExecutor",
                        ParameterAssemblyName = "DawnQuant.AShare.Strategy.dll",
                        ParameterClassName = "DawnQuant.AShare.Strategy.Executor.Factor.SubjectAndHotFactorExecutorParameter",
                        SortNum = 4
                    });
                }
            }
        }
    }
    
}
