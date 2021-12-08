using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Utils
{
    public class LatestTradingDateUtil
    {
        /// <summary>
        /// 获取最新交易日期，并且数据已经更新
        /// </summary>
        /// <returns></returns>
        public static DateTime GetLatestTradingDate(ITradingCalendarRepository _tcRepository,
        Func<string, KCycle, IStockTradeDataRepository> _stdrFunc, IBasicStockInfoRepository bsInfoRepository)
        {
            DateTime adt = DateTime.Now.Date;

            //获取最新交易日期
            var tc = _tcRepository.Entities.Where(p => p.Date == DateTime.Now.Date
             && p.Exchange == StockEssentialDataConst.SSE).FirstOrDefault();
            if (tc != null)
            {
                adt = tc.IsOpen ? tc.Date : tc.PreDate.Value;
            }

            return adt;

            //检测数据是否已经更新
            //同时检测三个股票是否已经更新数据
            // var main = bsInfoRepository.Entities.Where(p => p.ListedStatus == StockEssentialDataConst.Listing &&
            //  p.MarketType == StockEssentialDataConst.MainBoard).Select(p => p.TSCode).Take(1).First();


            // var star = bsInfoRepository.Entities.Where(p => p.ListedStatus == StockEssentialDataConst.Listing &&
            // p.MarketType == StockEssentialDataConst.StarBoard).Select(p => p.TSCode).Take(1).First();


            // var gemb = bsInfoRepository.Entities.Where(p => p.ListedStatus == StockEssentialDataConst.Listing &&
            //p.MarketType == StockEssentialDataConst.GEMBoard).Select(p => p.TSCode).Take(1).First();


            // //三个同时有数据
            // if (!(_stdrFunc(main, KCycle.Day).Entities.Where(p => p.TradeDateTime == adt).Any() &&
            //     _stdrFunc(star, KCycle.Day).Entities.Where(p => p.TradeDateTime == adt).Any() &&
            //     _stdrFunc(gemb, KCycle.Day).Entities.Where(p => p.TradeDateTime == adt).Any()))
            // {
            //     adt = _tcRepository.Entities.Where(p => p.Date == adt && p.Exchange == StockEssentialDataConst.SSE)
            //         .Select(p => p.PreDate).FirstOrDefault().Value;
            // }
        }
    }

}
