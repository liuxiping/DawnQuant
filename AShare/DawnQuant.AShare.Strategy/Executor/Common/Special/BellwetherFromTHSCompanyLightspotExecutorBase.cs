using DawnQuant.AShare.Repository.Abstract.EssentialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Common
{
    /// <summary>
    /// 从同花顺公司亮点中分析提取的龙头股
    /// </summary>
    public class BellwetherFromTHSCompanyLightspotExecutorBase 
    {
        public object Parameter { get; set; }

        private readonly IBellwetherRepository _bellwetherRepository;

        public BellwetherFromTHSCompanyLightspotExecutorBase(IBellwetherRepository bellwetherRepository)
        {
            _bellwetherRepository = bellwetherRepository;
        }

        public List<string> Execute()
        {
            var datas = _bellwetherRepository.Entities.Where(p => p.Source == 1).Select(p => p.TSCode).ToList();
            return datas;
        }
    }

   
}
