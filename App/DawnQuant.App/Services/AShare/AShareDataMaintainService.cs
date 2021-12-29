using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BillowFinancial.App.Core.Utilities;
using DawnQuant.App.Models;
using DawnQuant.Passport;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.App.Models.AShare.EssentialData;
using DawnQuant.App.Utils;
using Autofac;


namespace DawnQuant.App.Services.AShare
{
    public class AShareDataMaintainService 
    {
        private readonly GrpcChannelSet _grpcChannelSet;
        private readonly IPassportProvider _passportProvider;
        private readonly IMapper _mapper;
        private readonly SelfSelService _selfSelService;

        private readonly ILogger _logger;

        private readonly BellwetherService _bellwetherService;
        private readonly SubjectAndHotService _subjectAndHotService;


        public AShareDataMaintainService()
        {
            _grpcChannelSet = IOCUtil.Container.Resolve< GrpcChannelSet>();
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();
            _mapper = IOCUtil.Container.Resolve<IMapper>(); ;
            _selfSelService = IOCUtil.Container.Resolve<SelfSelService>();

            _bellwetherService = IOCUtil.Container.Resolve<BellwetherService>();
            _subjectAndHotService = IOCUtil.Container.Resolve<SubjectAndHotService>();

            _logger = IOCUtil.Container.Resolve<ILogger>();
        }

        /// <summary>
        /// 下载进度事件
        /// </summary>
        public    event Action<string> DownLoadStockDataProgress;
        protected void OnDownLoadStockDataProgress(string msg)
        {
            DownLoadStockDataProgress?.Invoke(msg);
        }

        /// <summary>
        /// 下载股票相关数据
        /// </summary>
        public void DownLoadStockData(int threadCount = 5)
        {
            //自选股
            var items = _selfSelService.GetSelfSelectStocksByUser(_passportProvider.UserId);
            var tsCodes = items.Select(p => p.TSCode).ToList<string>();

            //龙头股和题材热点
            var bellwethers = _bellwetherService.GetBellwetherStocksByUser(_passportProvider.UserId);

            var subjectAndHots= _subjectAndHotService.GetSubjectAndHotStocksByUser(_passportProvider.UserId);

            tsCodes.AddRange(bellwethers.Select(p => p.TSCode));
            tsCodes.AddRange(subjectAndHots.Select(p => p.TSCode));

            tsCodes=tsCodes.Distinct().ToList();

            DownLoadStockData(tsCodes, threadCount);

        }
       
        public void DownLoadStockData(List<string> tsCodes, int threadCount = 5)
        {
            tsCodes = tsCodes.Distinct().ToList();

            int allCount=tsCodes.Count;
            int completeCount = 0;
            object lockObj=new object();


            //下载交易数据
            List<Task> tasks = new List<Task>();

            if (tsCodes.Count <= threadCount)
            {
                foreach (var s in tsCodes)
                {

                    tasks.Add(Task.Run(() =>
                    {
                        DownLoadStockData(s);
                        lock(lockObj)
                        {
                            ++completeCount;
                            
                        }
                        string msg = $"正在下载交易数据，已经完成{completeCount}个，总共{allCount}个";
                        OnDownLoadStockDataProgress(msg);
                    }));
                }
            }
            else //分片
            {
                List<List<string>> lists = new List<List<string>>();
                int size = (int)Math.Ceiling(tsCodes.Count / (double)threadCount);
                for (int i = 0; i < threadCount; i++)
                {
                    lists.Add(tsCodes.Skip(i * size).Take(size).ToList());
                }

                foreach (var l in lists)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        foreach (var s in l)
                        {
                            DownLoadStockData(s);
                            lock (lockObj)
                            {
                                ++completeCount;
                            }
                            
                            string msg = $"正在下载交易数据，已经完成{completeCount}个，总共{allCount}个";
                            OnDownLoadStockDataProgress(msg);
                        }
                    }));
                }

            }
            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// 下载单个股票相关数据
        /// </summary>
        /// <param name="tsCode"></param>
        public void DownLoadStockData(string tsCode)
        {
            //下载交易数据
            DownLoadStockTradeData(tsCode);

        }

        /// <summary>
        /// 下载每日指标数据
        /// </summary>
        /// <param name="tsCode"></param>
        private void DownLoadStockDailyIndicator(string tsCode)
        {
          //  using (IStockDailyIndicatorRepository repository = _funcStockDailyIndicator(tsCode))
            {
                var client = new StockDailyIndicatorApi
                 .StockDailyIndicatorApiClient(_grpcChannelSet.AShareGrpcChannel);

               // DateTime? latest = null;
                DateTime? start = null;
                DateTime? end = null;
                //if (repository.Entities.Any())//增量更新
                //{
                //    latest = repository.Entities.AsNoTracking()
                //      .OrderByDescending(p => p.TradeDate).FirstOrDefault().TradeDate;
                //    start = DateTime.SpecifyKind(latest.Value.AddSeconds(1), DateTimeKind.Utc);
                //    end = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                //}
                Metadata meta = new Metadata();
                meta.AddAuthorization(_passportProvider?.AccessToken);
                GetStockDailyIndicatorRequest request = new GetStockDailyIndicatorRequest
                {
                    TSCode = tsCode,
                };

                if (start != null && end != null)
                {
                    request.StartDateTime = Timestamp.FromDateTime(start.Value);
                    request.EndDateTime = Timestamp.FromDateTime(end.Value);
                }

                var response = client.GetStockDailyIndicator(request, meta);

                var task = response.ResponseStream.MoveNext();

                while (true)
                {
                    task.Wait();
                    if (task.Result)
                    {
                        var message = response.ResponseStream.Current;
                        //保存数据
                      //  repository.Save(_mapper.Map<IEnumerable<StockDailyIndicator>>(message.Entities));
                        task = response.ResponseStream.MoveNext();
                    }
                    else
                    {
                        break;
                    }
                }

            }

        }

        private void DownLoadStockDailyIndicator()
        {
            var items = _selfSelService.GetSelfSelectStocksByUser(_passportProvider.UserId);

            if (items != null && items.Count > 0)
            {
                foreach (var item in items)
                {
                    DownLoadStockDailyIndicator(item.TSCode);
                }
            }
        }


        /// <summary>
        /// 下数据历史交易数据
        /// </summary>
        /// <param name="tsCode"></param>
        /// <param name="stockKCycle"></param>
        private async void DownLoadStockTradeData(string tsCode)
        {
            //判断本地是否有数据
            string filename = Helper.GetStockTradeDataFileName(tsCode, KCycle.Day); 

            //增量更新
            if (File.Exists(filename))
            {
                //读取全部数据
                var datas=ReadAllStockTradeData(tsCode);

                var start = DateTime.SpecifyKind(datas.Last().TradeDateTime, DateTimeKind.Utc);
                var end = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);

                List<StockTradeData> indatas = GetStockTradeDataFromServer(tsCode, KCycle.Day, start, end);

                //合并数据

                if(indatas != null && indatas.Count>0)
                {
                    indatas= indatas.OrderBy(p=>p.TradeDateTime).ToList();

                    foreach (var indata in indatas)
                    {
                       var d= datas.Find(p=>p.TradeDateTime==indata.TradeDateTime);
                        if (d != null)
                        {
                            datas.Remove(d);
                           
                        }
                        datas.Add(indata);
                    }

                    int tryCount=0; 
                    //重试10次 
                    while (tryCount<10)
                    {
                        try
                        {
                            //保存数据 二进制定长方式模式
                            using (FileStream fs = new FileStream(filename, FileMode.Create))
                            {
                                //排序
                                datas = datas.OrderBy(p => p.TradeDateTime).ToList();
                                BinaryWriter bw = new BinaryWriter(fs);
                                WriteStockTradeData(bw, datas);
                                bw.Close();
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"写入{tsCode}的股票交易数据发生错误：\r\n{ex.Message}\r\n{ex.StackTrace}");
                            Task.Delay(500).Wait();
                            tryCount++;
                        }
                    }
                }
                #region
                //using (FileStream fs = new FileStream(filename, FileMode.Open))
                //{
                //    BinaryReader br = new BinaryReader(fs);

                //    //定位到末尾
                //    br.BaseStream.Seek(0, SeekOrigin.End);
                //    //最后一行
                //    br.BaseStream.Seek(-8, SeekOrigin.End);

                //    //最后的数据日期
                //    last = DateTime.FromBinary(br.ReadInt64());

                //    //读取全部数据

                //}
                //  var end = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                //  List<StockTradeData> datas = GetStockTradeDataFromServer(tsCode, KCycle.Day, start, end);

                #endregion

                

            }
            else//下载全部数据
            {
                List<StockTradeData> datas =  GetStockTradeDataFromServer(tsCode, KCycle.Day, null, null);

                if (datas.Count > 0)
                {
                    //保存数据 二进制定长方式
                    using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
                    {
                        BinaryWriter bw = new BinaryWriter(fs);
                        WriteStockTradeData(bw, datas);
                        bw.Close();
                    }
                }
            }

        }
        /// <summary>
        /// 从服务器获取交易数据
        /// </summary>
        /// <param name="tscode"></param>
        /// <param name="kCycle"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private  List<StockTradeData> GetStockTradeDataFromServer(string tscode,KCycle kCycle,
            DateTime? start,DateTime? end)
        {
            List<StockTradeData> datas = new List<StockTradeData>();


            var client = new StockTradeDataApi
                 .StockTradeDataApiClient(_grpcChannelSet.AShareGrpcChannel);
            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

            GetTradeDataRequest request = new GetTradeDataRequest
            {
                KCycle = _mapper.Map<KCycleDto>(KCycle.Day),
                TSCode = tscode,
            };
            if (start != null && end != null)
            {
                request.StartDateTime = Timestamp.FromDateTime(start.Value);
                request.EndDateTime = Timestamp.FromDateTime(end.Value);
            }

            var response = client.GetTradeData(request, meta);
            var task = response.ResponseStream.MoveNext();
            while (true)
            {
                task.Wait();
                if (task.Result)
                {
                    var message = response.ResponseStream.Current;
                    //读取数据
                    datas.AddRange(_mapper.Map<IList<StockTradeData>>(message.Entities));
                    task = response.ResponseStream.MoveNext();
                }
                else
                {
                    break;
                }
            }
            return datas;
           
        }


        /// <summary>
        /// 二进制写数据
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="datas"></param>
        private void WriteStockTradeData(BinaryWriter bw, List<StockTradeData> datas)
        {
            foreach (var d in datas)
            {
                bw.Write(d.Open);
                bw.Write(d.Close);
                bw.Write(d.High);
                bw.Write(d.Low);
                bw.Write(d.Volume);
                bw.Write(d.Amount);
                bw.Write(d.PreClose);
                bw.Write(d.Turnover);
                bw.Write(d.TurnoverFree);
                bw.Write(d.AdjustFactor);
                bw.Write(d.TradeDateTime.ToBinary());
              
            }
        }


        /// <summary>
        /// 清楚冗余数据
        /// </summary>
        public void ClearRedundancyData()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 清除所有数据
        /// </summary>
        public void ClearAllData()
        {
            string dailyTDPath = Path.Combine(Environment.CurrentDirectory, "Data\\DailyTD\\");

            if(Directory.Exists(dailyTDPath))
            {
                Directory.Delete(dailyTDPath);
            }

        }

        /// <summary>
        /// 判断当前时间是否开市
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            var date = DateTime.Now.Date;
            bool isOpen = false;
            var utcdate = DateTime.SpecifyKind(new DateTime(date.Year, date.Month, date.Day), DateTimeKind.Utc);
            //交易所代码
            var tcClient = new TradingCalendarApi.TradingCalendarApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider.AccessToken);

            var tr = tcClient.MarketIsOpen(new MarketIsOpenRequest
            {
                Exchange = "SSE",
                Date = Timestamp.FromDateTime(utcdate)
            }, meta);
            isOpen = tr.IsOpen;
            return isOpen;
        }


        /// <summary>
        /// 读取本地交易数据
        /// </summary>
        /// <param name="tsCode"></param>
        /// <param name="kCycle"></param>
        /// <returns></returns>
        private List<StockTradeData> ReadAllStockTradeData(string tsCode,KCycle kCycle= KCycle.Day)
        {
            List<StockTradeData> datas = new List<StockTradeData>();
           
            string filename = Helper.GetStockTradeDataFileName(tsCode, kCycle);
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryReader br = new BinaryReader(fs);

                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    StockTradeData d = new StockTradeData();
                    d.Open = br.ReadDouble();
                    d.Close = br.ReadDouble();
                    d.High = br.ReadDouble();
                    d.Low = br.ReadDouble();
                    d.Volume = br.ReadDouble();
                    d.Amount = br.ReadDouble();
                    d.PreClose = br.ReadDouble();
                    d.Turnover = br.ReadDouble();
                    d.TurnoverFree = br.ReadDouble();
                    d.AdjustFactor = br.ReadDouble();
                    d.TradeDateTime = DateTime.FromBinary(br.ReadInt64());
                    datas.Add(d);
                }
            }
            return datas;
        }

    }
}
