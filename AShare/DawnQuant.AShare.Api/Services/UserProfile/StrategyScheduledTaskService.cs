using AutoMapper;
using DawnQuant.AShare.Api.StrategyExecutor;
using DawnQuant.AShare.Entities;
using DawnQuant.AShare.Entities.UserProfile;
using DawnQuant.AShare.Repository.Abstract.StrategyMetadata;
using DawnQuant.AShare.Repository.Abstract.UserProfile;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using DawnQuant.AShare.Repository.Abstract.EssentialData;

namespace DawnQuant.AShare.Api.UserProfile
{
    public class StrategyScheduledTaskService: StrategyScheduledTaskApi.StrategyScheduledTaskApiBase
    {
        double _threadCount = 5;

        private readonly ILogger<StrategyScheduledTaskService> _logger;
        private readonly IStrategyScheduledTaskRepository  _strategyScheduledTaskRepository;


        private readonly IFactorMetadataRepository _factorRepository;
        private readonly ISelectScopeMetadataRepository _stockSelectScopeRepository;
        private readonly IStockStrategyRepository _stockStrategyRepository;

        private readonly IMapper _mapper;

        private readonly FinalStrategyExecutor _finalStrategyExecutor;

        private readonly IServiceProvider _serviceProvider;

        private readonly ISelfSelectStockRepository _selfSelectStockRepository;

        private readonly IBasicStockInfoRepository _basicStockInfoRepository;
        private readonly IIndustryRepository _industryRepository;

        public StrategyScheduledTaskService(IServiceProvider serviceProvider, 
        ILogger<StrategyScheduledTaskService> logger,IMapper mapper,

        ISelfSelectStockRepository selfSelectStockRepository,
        IBasicStockInfoRepository basicStockInfoRepository,
        IIndustryRepository industryRepository,
        IFactorMetadataRepository factorRepository,
        ISelectScopeMetadataRepository stockSelectScopeRepository,
        IStockStrategyRepository stockStrategyRepository,
        IStrategyScheduledTaskRepository strategyScheduledTaskRepository)
        {
            _logger = logger;
            _mapper = mapper;

            _factorRepository = factorRepository;
            _stockSelectScopeRepository = stockSelectScopeRepository;
            _serviceProvider = serviceProvider;

            _strategyScheduledTaskRepository = strategyScheduledTaskRepository;
            _stockStrategyRepository = stockStrategyRepository;

            _serviceProvider = serviceProvider;

            _selfSelectStockRepository = selfSelectStockRepository;

            _finalStrategyExecutor = new FinalStrategyExecutor(
              _factorRepository, _stockSelectScopeRepository, _stockStrategyRepository,
              _serviceProvider);

            _basicStockInfoRepository = basicStockInfoRepository;
            _industryRepository = industryRepository;
        }
        public override Task<SaveStrategyScheduledTasksResponse> SaveStrategyScheduledTasks(SaveStrategyScheduledTasksRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                SaveStrategyScheduledTasksResponse response = new SaveStrategyScheduledTasksResponse();
                var sst = _strategyScheduledTaskRepository.Save(_mapper.Map<IEnumerable<StrategyScheduledTask>>(request.Entities));
                response.Entities.AddRange(_mapper.Map<IEnumerable<StrategyScheduledTaskDto>>(sst));
                return response;
            });

        }

        public override Task<GetStrategyScheduledTasksByUserIdResponse> GetStrategyScheduledTasksByUserId(GetStrategyScheduledTasksByUserIdRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetStrategyScheduledTasksByUserIdResponse response = new GetStrategyScheduledTasksByUserIdResponse();
                var ts = _strategyScheduledTaskRepository.Entities.OrderBy(p=>p.SortNum).Where(p => p.UserId == request.UserId);
                response.Entities.AddRange(_mapper.Map<IEnumerable<StrategyScheduledTaskDto>>(ts));
                return response;

            });
        }

        public override Task<Empty> DelStrategyScheduledTaskById(DelStrategyScheduledTaskByIdRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                _strategyScheduledTaskRepository.Delete(request.Id);
                return new Empty();
            });
        }

        /// <summary>
        /// 执行策略任务计划
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<ExecuteStrategyScheduledTaskResponse> ExecuteStrategyScheduledTask(ExecuteStrategyScheduledTaskRequest request, ServerCallContext context)
        {
           return Task.Run(() =>
            {
                ExecuteStrategyScheduledTaskResponse r = new ExecuteStrategyScheduledTaskResponse();

                List<string> tsCodes = new List<string>();

                //线程同步对象
                object lockobj = new object();

                //获取计划任务对象
                var sst = _strategyScheduledTaskRepository.Entities.Where(p => p.Id == request.Id).FirstOrDefault();
                if(sst==null)
                {
                    string msg = "查找不到计划任务，请检查参数";
                    _logger.LogError(msg);
                    throw new Exception(msg);
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
                            if(ssids.Length<=_threadCount)
                            {
                                _threadCount = ssids.Length;
                            }

                            for (int i = 0; i < _threadCount; i++)
                            {
                                lists.Add(ssids.Skip(i * length).Take(length).ToList());
                            }

                            List<Task> tasks = new List<Task>();

                            //开启多个线程计算复权因子
                            foreach (var spilt in lists)
                            {
                                tasks.Add(Task.Run(() =>
                                {
                                    foreach (var id in spilt)
                                    {
                                        //执行策略
                                        var codes = _finalStrategyExecutor.ExecuteStrategyById(long.Parse(id));

                                        lock (lockobj) {
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
                        self.CreateTime = DateTime.Now;
                        ustocks.Add(self);
                    }
                    else
                    {
                        SelfSelectStock selfSelectStock = new SelfSelectStock();
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

                //保存执行时间
                sst.LatestExecuteTime = DateTime.Now;
                _strategyScheduledTaskRepository.Save(sst);
                r.TSCodes.AddRange(tsCodes);

                return r;
            });
            
        }


 
    }
}
