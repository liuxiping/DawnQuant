using DawnQuant.AShare.Analysis.Common;
using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using DawnQuant.AShare.Strategy.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DawnQuant.AShare.Analysis.Common.Harden;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{
    /// <summary>
    /// 涨停板
    /// </summary>
    public class HardenFactorExecutor : IFactorExecutor
    {
        private readonly ITradingCalendarRepository _tcRepository;
        private readonly IBasicStockInfoRepository _bsinfoRepository;
        private readonly Func<string, KCycle, IStockTradeDataRepository> _stdrFunc;


        public HardenFactorExecutor(ITradingCalendarRepository tcRepository,
            IBasicStockInfoRepository bsinfoRepository,
            Func<string, KCycle, IStockTradeDataRepository> stdrFunc)
        {
            _tcRepository = tcRepository;
            _stdrFunc = stdrFunc;
            _bsinfoRepository = bsinfoRepository;
        }

        public object Parameter { get; set; }

        public List<string> Execute(List<string> tsCodes)
        {
            HardenFactorExecutorParameter p = (HardenFactorExecutorParameter)Parameter;
            List<string> meet = new List<string>();

            //参数检查
            if (p.HardenCount > p.LatestTradeDateCount)
            {
                return meet;
            }

            //读取股票基本信息
            var basicInfos = _bsinfoRepository.Entities.Select(p => new HBasicStockInfo
            {
                TSCode = p.TSCode, StockName = p.StockName,
                MarketType = p.MarketType, ListingDate = p.ListingDate
            }).ToList();

            DateTime latest= DateTime.Now.Date;
            //获取最新交易日期
            latest = LatestTradingDateUtil.GetLatestTradingDate(_tcRepository, _stdrFunc, _bsinfoRepository);

            //统计最近交易日
            if (p.LatestTradeDateCount.HasValue)
            {

                foreach (var tsCode in tsCodes)
                {
                    var stdr = _stdrFunc(tsCode, KCycle.Day);
                    var datas = stdr.Entities.OrderByDescending(p => p.TradeDateTime).Take(p.LatestTradeDateCount.Value).ToList();

                    var binfo= basicInfos.Where(p => p.TSCode == tsCode).FirstOrDefault();

                    if (binfo != null)
                    {
                        //创业板 科创板 上市前5日不设涨幅，去除数据
                        if (binfo.MarketType == StockEssentialDataConst.GEMBoard ||
                            binfo.MarketType == StockEssentialDataConst.SAMBoard)
                        {
                            var tcs = _tcRepository.Entities.OrderBy(p => p.Date)
                                .Where(p => p.Date>= binfo.ListingDate && p.IsOpen && p.Exchange == StockEssentialDataConst.SSE).Take(5);

                            datas = datas.Where(p => p.TradeDateTime > tcs.Last().Date).ToList();
                        }


                        if (Harden.IsHarden(tsCode, p.HardenCount, datas, basicInfos))
                        {
                            meet.Add(tsCode);
                        }
                    }
                    
                }
            }
            else
            {
                //开始日期
                DateTime start;
                if (p.StartDate.HasValue)
                {
                    start = p.StartDate.Value.Date;
                }
                else
                {
                    start = latest;
                    
                }
                //结束日期
                DateTime end;
                if (p.EndDate.HasValue)
                {
                    end = p.EndDate.Value.Date;
                }
                else
                {
                    end = latest;
                }
                foreach (var tsCode in tsCodes)
                {
                    var stdr = _stdrFunc(tsCode, KCycle.Day);
                    var datas = stdr.Entities.Where(p => p.TradeDateTime>=start && p.TradeDateTime<=end).ToList();

                    var binfo = basicInfos.Where(p => p.TSCode == tsCode).FirstOrDefault();

                    //创业板 科创板 上市前5日不设涨幅，去除数据
                    if (binfo.MarketType == StockEssentialDataConst.GEMBoard ||
                        binfo.MarketType == StockEssentialDataConst.SAMBoard)
                    {
                        var tcs = _tcRepository.Entities.OrderBy(p => p.Date)
                            .Where(p => p.Date >= binfo.ListingDate && p.IsOpen && p.Exchange == StockEssentialDataConst.SSE).Take(5);

                        datas = datas.Where(p => p.TradeDateTime > tcs.Last().Date).ToList();
                    }

                    if (Harden.IsHarden(tsCode, p.HardenCount, datas, basicInfos))
                    {
                        meet.Add(tsCode);
                    }
                }
            }
            return meet;
        }



    }
}
