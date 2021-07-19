using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DawnQuant.AShare.Api.StrategyExecutor;
using DawnQuant.AShare.Entities;
using DawnQuant.AShare.Entities.UserProfile;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using DawnQuant.AShare.Repository.Abstract.StrategyMetadata;
using DawnQuant.AShare.Repository.Abstract.UserProfile;
using Google.Protobuf;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace DawnQuant.AShare.Api.Quartz.Job
{
    /// <summary>
    /// 策略执行
    /// </summary>
    public class StrategyScheduledTaskJob : IJob
    {

        private readonly ILogger<StrategyScheduledTaskJob> _logger;
        private readonly IStrategyScheduledTaskRepository _strategyScheduledTaskRepository;
        private readonly IStockStrategyRepository _stockStrategyRepository;
        private readonly ISelfSelectStockRepository _selfSelectStockRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;

        private readonly IFactorMetadataRepository _factorRepository;
        private readonly ISelectScopeMetadataRepository _stockSelectScopeRepository;


        private readonly IBasicStockInfoRepository _basicStockInfoRepository;
        private readonly IIndustryRepository _industryRepository;

        private readonly FinalStrategyExecutor _finalStrategyExecutor;

        public StrategyScheduledTaskJob(ILogger<StrategyScheduledTaskJob> logger, IMapper mapper,
        IStrategyScheduledTaskRepository strategyScheduledTaskRepository,
        IStockStrategyRepository stockStrategyRepository,
         ISelfSelectStockRepository selfSelectStockRepository,
          IFactorMetadataRepository factorRepository,
        ISelectScopeMetadataRepository stockSelectScopeRepository,

        IBasicStockInfoRepository basicStockInfoRepository,
        IIndustryRepository industryRepository,

        IServiceProvider serviceProvider)
        {
            _logger = logger;
            _mapper = mapper;

            _strategyScheduledTaskRepository = strategyScheduledTaskRepository;

            _selfSelectStockRepository = selfSelectStockRepository;

            _serviceProvider = serviceProvider;

            _factorRepository = factorRepository;
            _stockSelectScopeRepository = stockSelectScopeRepository;
            _stockStrategyRepository = stockStrategyRepository;

            _basicStockInfoRepository = basicStockInfoRepository;
            _industryRepository = industryRepository;

            _finalStrategyExecutor = new FinalStrategyExecutor(
               _factorRepository, _stockSelectScopeRepository, _stockStrategyRepository,
               _serviceProvider);
        }

        double _threadCount = 5;

        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                ExecuteStrategyScheduledTask();
            });

        }

        public void ExecuteStrategyScheduledTask()
        {
            //需要在服务器执行的策略
            var sst = _strategyScheduledTaskRepository.Entities.Where(p => p.IsJoinServerScheduleTask == true).ToList();

            int allCount = sst.Count;
            //分片分别开启多个线程更新数据
            int length = (int)Math.Ceiling(sst.Count / _threadCount);

            List<List<StrategyScheduledTask>> lists = new List<List<StrategyScheduledTask>>();

            for (int i = 0; i < _threadCount; i++)
            {
                lists.Add(sst.Skip(i * length).Take(length).ToList());
            }
            List<Task> tasks = new List<Task>();

            //开启多个线程计算计划任务
            foreach (var spilts in lists)
            {
                foreach (var s in spilts)
                {
                    var t = Task.Run(() =>
                    {
                        ExecuteSingleStrategyScheduledTask(s);
                    });
                    tasks.Add(t);
                }
            }

        }

        #region 注释代码
        ///// <summary>
        ///// 执行单个任务计划
        ///// </summary>
        ///// <param name="strategyId"></param>
        ///// <param name="sst"></param>
        ///// <returns></returns>
        //private void ExecuteStrategy(long strategyId, StrategyScheduledTask sst)
        //{

        //    using (var scope = _scopeFactory.CreateScope())
        //    {
        //        IStockStrategyRepository ssr = scope.ServiceProvider.GetService<IStockStrategyRepository>();

        //        ISelfSelectStockRepository sssr = scope.ServiceProvider
        //                  .GetService<ISelfSelectStockRepository>();
        //        IStrategyScheduledTaskRepository sstr = scope.ServiceProvider
        //                  .GetService<IStrategyScheduledTaskRepository>();

        //        var st = ssr.Entities.Where(p => p.Id == strategyId).FirstOrDefault();
        //        if (st != null)
        //        {
        //            //  var client = new Stock.Api.Strategy.StockStrategyApi.StockStrategyApiClient(_grpcChannelSet.StockGrpcChannel);
        //            // var response = client.ExecuteStockStrategy(new Stock.Api.Strategy.ExecuteStockStrategyRequest
        //            // { Strategy = ByteString.CopyFrom(st.StockStragyContent) });

        //            // var stocks = _mapper.Map<IEnumerable<SelfSelectStock>>(response.Entities);
        //            var stocks = _mapper.Map<IEnumerable<SelfSelectStock>>(null);

        //            //todo
        //            List<SelfSelectStock> ustocks = new List<SelfSelectStock>();

        //            foreach (var s in stocks)
        //            {



        //                //更新数据到输出类目
        //                if (!sssr.Entities.Where(p => p.UserId == sst.UserId &&
        //                 p.CategoryId == sst.OutputStockCategoryId && p.TSCode == s.TSCode).Any())
        //                {
        //                    if (!ustocks.Where(p => p.TSCode == s.TSCode).Any())
        //                    {
        //                        s.UserId = sst.UserId;
        //                        s.CategoryId = sst.OutputStockCategoryId;
        //                        s.CreateTime = DateTime.Now;
        //                        ustocks.Add(s);
        //                    }
        //                }

        //            }
        //            if (ustocks.Count > 0)
        //            {
        //                _selectStockRepository.Insert(ustocks);
        //            }

        //            //保存执行时间
        //            sst.LatestExecuteTime = DateTime.Now;
        //            sstr.Save(sst);
        //        }
        //        else
        //        {
        //            _logger.LogError("查找不到策略！");
        //        }
        //    }
        //}
        #endregion


        /// <summary>
        /// 执行单个计划任务
        /// </summary>
        private void ExecuteSingleStrategyScheduledTask(StrategyScheduledTask sst)
        {
            List<string> tsCodes = new List<string>();
            //线程同步对象
            object lockobj = new object();
            //获取计划任务对象
            if (sst == null)
            {
                return;
            }
            else
            {
                if (!string.IsNullOrEmpty(sst.StrategyIds))
                {
                    //提取策略列表
                    var ssids = sst.StrategyIds.Split(',');

                    if (ssids.Length > 0)
                    {
                        //分片分别开启多个线程计算任务
                        int length = (int)Math.Ceiling(ssids.Length / _threadCount);

                        List<List<string>> lists = new List<List<string>>();

                        //策略数目小于线程数目，这线程数目等于策略数目
                        if (ssids.Length <= _threadCount)
                        {
                            _threadCount = ssids.Length;
                        }

                        for (int i = 0; i < _threadCount; i++)
                        {
                            lists.Add(ssids.Skip(i * length).Take(length).ToList());
                        }

                        List<Task> tasks = new List<Task>();

                        //开启多个线程执行策略
                        foreach (var spilt in lists)
                        {
                            tasks.Add(Task.Run(() =>
                            {
                                foreach (var id in spilt)
                                {
                                    //执行策略
                                    var codes = _finalStrategyExecutor.ExecuteStrategyById(long.Parse(id));

                                    lock (lockobj)
                                    {
                                        //合并结果
                                        tsCodes = tsCodes.Union(codes).ToList();
                                    }

                                }

                            }));
                        }

                        Task.WaitAll(tasks.ToArray());
                    }

                }
            }


            //去重
            tsCodes = tsCodes.Distinct().ToList();

            List<SelfSelectStock> ustocks = new List<SelfSelectStock>();

            //更新数据到输出类目
            foreach (var s in tsCodes)
            {

                //检测数据是否存在 如果存在则更新时间
                var self = _selfSelectStockRepository.Entities.Where(p => p.UserId == sst.UserId &&
                   p.CategoryId == sst.OutputStockCategoryId && p.TSCode == s).FirstOrDefault();
                if (self != null)
                {
                    //更新创建时间
                   // self.CreateTime = DateTime.Now;
                    ustocks.Add(self);
                }
                else
                {
                    SelfSelectStock selfSelectStock = new SelfSelectStock();

                    selfSelectStock.Name =
                    selfSelectStock.TSCode = s;
                    selfSelectStock.UserId = sst.UserId;
                    selfSelectStock.CategoryId = sst.OutputStockCategoryId;
                    selfSelectStock.CreateTime = DateTime.Now;

                    //获取行业和名称
                    var basicInfo = _basicStockInfoRepository.Entities.Where(p => p.TSCode == s).FirstOrDefault();
                    string indus = _industryRepository.Entities
                     .Where(p => p.Id == basicInfo.IndustryId).Select(p => p.Name).SingleOrDefault();
                    selfSelectStock.Name = basicInfo.StockName;
                    selfSelectStock.Industry = indus;

                    ustocks.Add(selfSelectStock);
                }


            }

            if (ustocks.Count > 0)
            {
                //保存自选股
                _selfSelectStockRepository.Save(ustocks);
            }

            //保存计划任务执行时间
            sst.LatestExecuteTime = DateTime.Now;
            _strategyScheduledTaskRepository.Save(sst);


        }
    }
}
