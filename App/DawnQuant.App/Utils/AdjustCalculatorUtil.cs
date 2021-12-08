using DawnQuant.App.Models.AShare;
using DawnQuant.App.Models.AShare.EssentialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Utils
{
    /// <summary>
    /// 复权因子
    /// </summary>
    public static class AdjustCalculatorUtil
    {
        /// <summary>
        /// 计算复权因子
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static void CalculateAllAdjustFactor(List<StockTradeData> datas)
        {
            datas = datas.OrderBy(p => p.TradeDateTime).ToList();
            if (datas!=null)
            {
                double factor = 1;//累计涨幅
                
                //后复权
                foreach (StockTradeData d in datas)
                {
                    factor = factor * (1+((d.Close- d.PreClose) / d.PreClose));
                    d.AdjustFactor = factor;
                }

            }
        }


        /// <summary>
        /// 计算前复权价格
        /// </summary>
        /// <param name="datas"></param>
        public static void CalculatePrePrice(List<StockTradeData> datas)
        {
            if(datas.Count<=0)
            {
                return ;
            }
            datas= datas.OrderByDescending(p => p.TradeDateTime).ToList();

            //计算复权收盘价格
            double curClose = datas[0].Close;//基准价格

            for (int i = 0; i < datas.Count - 1; i++)
            {
                //(datas[i].AdjustFactor-datas[i + 1].AdjustFactor)/

                double newClose = curClose * datas[i + 1].AdjustFactor / datas[i].AdjustFactor;
                double oldClose = datas[i + 1].Close;
                double gain = (newClose - oldClose) / oldClose;

                //调整价格
                datas[i + 1].Open = datas[i + 1].Open * (1 + gain);
                datas[i + 1].Close = newClose; ;
                datas[i + 1].High = datas[i + 1].High * (1 + gain);
                datas[i + 1].Low = datas[i + 1].Low * (1 + gain);

                curClose = newClose;
            }
            datas.Reverse();
        }

        /// <summary>
        /// 计算后复权价格
        /// </summary>
        /// <param name="datas"></param>
        /// <param name="basePrice"></param>
        public static void CalculateAfterPrice(List<StockTradeData> datas,double basePrice)
        {

            datas = datas.OrderBy(p => p.TradeDateTime).ToList();

            foreach (var d in datas)
            {
                double newClose = basePrice * d.AdjustFactor;
                double oldClose = d.Close;
                double gain = (newClose - oldClose) / oldClose;


                d.Open = d.Open * (1 + gain);
                d.Close = newClose; ;
                d.High = d.High * (1 + gain);
                d.Low = d.Low * (1 + gain);
            }

        }
    }
}
