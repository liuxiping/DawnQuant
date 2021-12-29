using DawnQuant.AShare.Repository.Abstract.EssentialData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Executor.Common
{

    /// <summary>
    /// 业绩预测 
    /// </summary>
    public class PerformanceForecastFromTHSExecutorBase 
    {
        public object Parameter { get; set; }

        private readonly IPerformanceForecastRepository _forecastRepository;

        public PerformanceForecastFromTHSExecutorBase(IPerformanceForecastRepository forecastRepository)
        {
            _forecastRepository = forecastRepository;
        }

        public List<string> Execute()
        {

            PerformanceForecastFromTHSExecutorParameterBase p = (PerformanceForecastFromTHSExecutorParameterBase)Parameter;

            var datas = _forecastRepository.Entities.Where(
              e => (e.ForecastOrgCount >= p.MinForecastOrgCount && e.ForecastOrgCount<=p.MaxForecastOrgCount) &&
             ( e.EarningsPerShareChangeRatio >= p.MinEarningsPerShareChangeRatio && e.EarningsPerShareChangeRatio <= p.MaxEarningsPerShareChangeRatio) &&
              e.RetainedProfitsChangeRatio >= p.MinRetainedProfitsChangeRatio && e.RetainedProfitsChangeRatio <= p.MaxRetainedProfitsChangeRatio)
                 .Select(p => p.TSCode).ToList();

            return datas;
        }
    
    }
}
