using DawnQuant.AShare.Analysis.Resample;
using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Factor
{
    /// <summary>
    /// 缩量相对前一个周期缩量
    /// </summary>
    public class VolReductionFactorExecutor : IFactorExecutor
    {
        public object Parameter { get ; set; }

        private readonly Func<string, KCycle, IStockTradeDataRepository> _stdrFunc;


        public VolReductionFactorExecutor(Func<string, KCycle, IStockTradeDataRepository> stdrFunc)
        {
            _stdrFunc = stdrFunc;
        }

        public List<string> Execute(List<string> tsCodes)
        {

            VolReductionFactorExecutorParameter p = (VolReductionFactorExecutorParameter)Parameter;
            List<string> rtsCodes = new List<string>();
            List<StockTradeData> datas = new List<StockTradeData>();
            if (tsCodes != null && tsCodes.Count > 0)
            {
                //日线
                if (p.KCycle == KCycle.Day)
                {
                    foreach (var tsCode in tsCodes)
                    {
                        var tdr = _stdrFunc(tsCode, KCycle.Day);
                        datas = tdr.Entities.OrderByDescending(pt => pt.TradeDateTime).Take(2).ToList();
                        //正序排列
                        datas.Reverse();
                        if (datas.Count == 2)
                        {
                            if (IsMeet(p, datas))
                            {
                                rtsCodes.Add(tsCode);
                            }
                        }
                    }
                }
                //周线
                else if (p.KCycle == KCycle.Week)
                {

                    foreach (var tsCode in tsCodes)
                    {
                        int size = (2 + 2) * 5;
                        var tdr = _stdrFunc(tsCode, KCycle.Day);
                        var tempdatas = tdr.Entities.OrderByDescending(pt => pt.TradeDateTime).Take(size).ToList();
                        var temp = ResampleBasedOnDailyData.ToWeekCycle(tempdatas);
                        if (temp.Count >= 2)
                        {
                            datas = temp.Skip(temp.Count - 2)
                                .Take(2).ToList();

                            if (IsMeet(p, datas))
                            {
                                rtsCodes.Add(tsCode);
                            }
                        }

                    }
                }
                else
                {
                    throw new NotSupportedException("只支持日线 周线 ");
                }
            }
            return rtsCodes;
        }

        private bool IsMeet(VolReductionFactorExecutorParameter p, List<StockTradeData> datas)
        {
            double preVol = datas[0].Volume;
            double curVol = datas[1].Volume;

            double ratio = (preVol - curVol)*100 / preVol;

            if (ratio>=p.ReductionRatio)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
