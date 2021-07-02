
using DawnQuant.AShare.Entities.EssentialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Analysis.Common
{
    /// <summary>
    /// 涨停判断
    /// </summary>
    public class Harden
    {
        /// <summary>
        /// 判断时间段类是有指定涨停次数
        /// </summary>
        /// <param name="tsCode"></param>
        /// <param name="datas"></param>
        public static bool IsHarden(string tsCode, int hardenCount, List<StockTradeData> datas, List<HBasicStockInfo> hbsInfos)
        {
            //实际满足次数
            int acCount = 0;

            //股票基本信息
            var info = hbsInfos.Where(p => p.TSCode == tsCode).FirstOrDefault();

            if (info != null)
            {
                //ST涨幅5%涨停 ,ST股不可能是新股
                if (info.StockName.ToUpper().Contains("ST"))
                {
                    foreach (var d in datas)
                    {
                        double h = Math.Round(d.PreClose * (1.05), 2, MidpointRounding.AwayFromZero);
                        double close = Math.Round(d.Close, 2, MidpointRounding.AwayFromZero);
                        if (close >= h)
                        {
                            acCount++;
                        }
                    }
                    if (acCount >= hardenCount)
                    {
                        return true;
                    }
                }

                //主板 中小板 10%  上市首日涨幅44%
                if (info.MarketType == StockEssentialDataConst.MainBoard ||
                    info.MarketType == StockEssentialDataConst.SAMBoard)
                {
                    foreach (var d in datas)
                    {
                        double h = Math.Round(d.PreClose * (1.1), 2, MidpointRounding.AwayFromZero);

                        //上市日期涨幅44%
                        if (d.TradeDateTime == info.ListingDate)
                        {
                            h = Math.Round(d.PreClose * (1.44), 2, MidpointRounding.AwayFromZero);
                        }

                        double close = Math.Round(d.Close, 2, MidpointRounding.AwayFromZero);
                        if (close >= h)
                        {
                            acCount++;
                        }
                    }
                    if (acCount >= hardenCount)
                    {
                        return true;
                    }
                }

                //创业板 科创板 20%  上市前5日涨幅不设限制 去除上市前5日数据
                if (info.MarketType == StockEssentialDataConst.GEMBoard ||
                    info.MarketType == StockEssentialDataConst.StarBoard)
                {

                    foreach (var d in datas)
                    {
                        double h = Math.Round(d.PreClose * (1.2), 2, MidpointRounding.AwayFromZero);
                        double close = Math.Round(d.Close, 2, MidpointRounding.AwayFromZero);
                        if (close >= h)
                        {
                            acCount++;
                        }
                    }
                    if (acCount >= hardenCount)
                    {
                        return true;
                    }

                }
            }
           
            return false;
        }


        /// <summary>
        /// 股票基本信息
        /// </summary>
        public class HBasicStockInfo
        {
            public string TSCode { get; set; }

            public string StockName { get; set; }

            public string MarketType { get; set; }

            public DateTime ListingDate { get; set; }
        }
    }
}
