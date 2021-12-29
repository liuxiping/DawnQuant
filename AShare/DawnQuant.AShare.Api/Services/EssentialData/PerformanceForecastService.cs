using AutoMapper;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using DawnQuant.AShare.Entities.EssentialData;
using System.Linq;

namespace DawnQuant.AShare.Api.EssentialData
{
    public class PerformanceForecastService: PerformanceForecastApi.PerformanceForecastApiBase
    {
        private readonly ILogger<HolderService> _logger;
        private readonly IPerformanceForecastRepository _forecastRepository;

        private readonly IMapper _imapper;


        public PerformanceForecastService(ILogger<HolderService> logger,
         IPerformanceForecastRepository forecastRepository,
        IMapper imapper)
        {
            _logger = logger;
            _forecastRepository = forecastRepository;
            _imapper = imapper;
        }

        public override Task<Empty> SavePerformanceForecasts(SavePerformanceForecastsRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {

                if(request.Entities.Count>0)
                { 
                    //先清空之前的数据
                    _forecastRepository.Empty();
                   // var tsCodes= request.Entities.Select(e => e.TSCode).Distinct().ToList();

                  // var fs= _forecastRepository.Entities.Where(e => tsCodes.Contains(e.TSCode));
                  //  _forecastRepository.Delete(fs);

                    //保存数据
                    var datas = _imapper.Map<IEnumerable<PerformanceForecast>>(request.Entities);
                    _forecastRepository.Save(datas);
                }

               
              
                return new Empty();
            });


        }

    }
}
