using AutoMapper;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Api.EssentialData
{
    public class THSIndexService : THSIndexApi.THSIndexApiBase
    {
        private readonly ILogger<HolderService> _logger;
        private readonly ITHSIndexRepository _thsIndexRepository;

        private readonly IMapper _imapper;


        public THSIndexService(ILogger<HolderService> logger,
          ITHSIndexRepository thsIndexRepository, IMapper imapper)
        {
            _logger = logger;
            _thsIndexRepository = thsIndexRepository;
            _imapper = imapper;
        }


        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<Empty> SaveTHSIndexes(SaveTHSIndexesRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                if (request.Entities.Count > 0)
                {

                    var datas = _imapper.Map<IEnumerable<THSIndex>>(request.Entities);

                    _thsIndexRepository.Save(datas);
                }

                return new Empty();
            });

        }


        public override Task<GetAllTHSIndexesReponse> GetAllTHSIndexes(Empty request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetAllTHSIndexesReponse reponse = new GetAllTHSIndexesReponse();

                var datas = _thsIndexRepository.Entities.ToList();

                if (datas!=null && datas.Count > 0)
                {
                    var dtos = _imapper.Map<IEnumerable<THSIndexDto>>(datas);
                    reponse.Entities.AddRange(dtos);
                }
                return reponse;

            });
        }
    }
}
