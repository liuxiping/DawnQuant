using DawnQuant.AShare.Repository.Abstract.EssentialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Common
{
    /// <summary>
    /// 从10大流通股追踪人数
    /// </summary>
    public class Top10FloatHolderExecutorBase
    {
        public object Parameter { get; set; }

        private readonly ITop10FloatHolderRepository _top10FloatHolderRepository;

        public Top10FloatHolderExecutorBase(ITop10FloatHolderRepository top10FloatHolderRepository)
        {
            _top10FloatHolderRepository = top10FloatHolderRepository;
        }

        public List<string> Execute()
        {
            List<string> result = new List<string>();

            Top10FloatHolderExecutorParameterBase p = (Top10FloatHolderExecutorParameterBase)Parameter;

            //过滤出新进的股东
            var allHolderNumber = _top10FloatHolderRepository.Entities.Where(p => p.HoldChangeCharacter == 2)
                .ToList().GroupBy(p => p.TSCode);

            foreach (var parameter in p.Top10FloatHolderExecutorParameters)
            {
                foreach (var holder in allHolderNumber)
                {

                    //新进股东持股比例
                    var r = holder.Where(e => e.HoldRatio >= parameter.MinNewHolderRatio &&
                    e.HoldRatio <= parameter.MaxNewHolderRatio).ToList();

                    if (r.Count >= parameter.MinCount && r.Sum(p => p.HoldRatio) >= parameter.NewHolderTotalRatio)
                    {
                        result.Add(holder.Key);
                    }
                }
            }
            return result;

        }

    }
}
