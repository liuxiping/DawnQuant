using DawnQuant.AShare.Repository.Abstract.EssentialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Common
{

    /// <summary>
    /// 从股东人数追踪主力
    /// </summary>
    public class HolderNumberExecutorBase 
    {
        public object Parameter { get; set; }

        private readonly IHolderNumberRepository _holderNumberRepository;

        public HolderNumberExecutorBase(IHolderNumberRepository holderNumberRepository)
        {
            _holderNumberRepository = holderNumberRepository;
        }

        public List<string> Execute()
        {

            List<string> result = new List<string>();

            HolderNumberExecutorParameterBase p = (HolderNumberExecutorParameterBase)Parameter;

            var allHolderNumber = _holderNumberRepository.Entities.ToList().GroupBy(p=>p.TSCode);

            foreach (var parameter in p.HolderNumberExecutorParameters)
            {
                foreach (var holder in allHolderNumber)
                {
                    var r = holder.Where(p => p.Change >= parameter.MinHolderNumberChangeRatio
                      && p.Change <= parameter.MaxHolderNumberChangeRatio).ToList();

                    if (r.Count >= parameter.MinCount)
                    {
                        result.Add(holder.Key);
                    }
                }
            }
            return result;

        }
    }
    
    
}
