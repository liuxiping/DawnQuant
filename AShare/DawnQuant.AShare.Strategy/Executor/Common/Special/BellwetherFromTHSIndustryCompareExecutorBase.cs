using DawnQuant.AShare.Repository.Abstract.EssentialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Common
{
    public class BellwetherFromTHSIndustryCompareExecutorBase 
    {
        public object Parameter { get; set; }

        private readonly IBellwetherRepository _bellwetherRepository;

        public BellwetherFromTHSIndustryCompareExecutorBase(IBellwetherRepository bellwetherRepository)
        {
            _bellwetherRepository = bellwetherRepository;
        }

        public List<string> Execute()
        {
            var datas = _bellwetherRepository.Entities.Where(p => p.Source == 2).Select(p => p.TSCode).ToList();
            return datas;
        }

    }
}
