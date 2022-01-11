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
    public class THSIndexMemberService : THSIndexMemberApi.THSIndexMemberApiBase
    {
        private readonly ILogger<HolderService> _logger;
        private readonly ITHSIndexMemberRepository _thsIndexMemberRepository;
        private readonly IBasicStockInfoRepository _basicStockInfoRepository;

        private readonly IMapper _imapper;


        public THSIndexMemberService(ILogger<HolderService> logger,
          ITHSIndexMemberRepository thsIndexMemberRepository, IMapper imapper,
          IBasicStockInfoRepository basicStockInfoRepository)
        {
            _logger = logger;
            _thsIndexMemberRepository = thsIndexMemberRepository;
            _basicStockInfoRepository= basicStockInfoRepository;
            _imapper = imapper;
        }

        public override Task<Empty> SaveTHSIndexMembers(SaveTHSIndexMembersRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
             {


                 if (request.Entities.Count > 0)
                 {

                     var datas = _imapper.Map<IEnumerable<THSIndexMember>>(request.Entities);

                     _thsIndexMemberRepository.Save(datas);
                 }

                 return new Empty();

             });
        }
        public override Task<Empty> EmptyTHSIndexMembers(Empty request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                _thsIndexMemberRepository.Empty();
                return new Empty();
            });

        }

        public override Task<GetTHSIndexMembersByTSCodeResponse> GetTHSIndexMembersByTSCode(GetTHSIndexMembersByTSCodeRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {
                GetTHSIndexMembersByTSCodeResponse response = new GetTHSIndexMembersByTSCodeResponse();

                var datas = _thsIndexMemberRepository.Entities.Where(p => p.TSCode == request.TSCode);

                response.Entities.AddRange(_imapper.Map<IEnumerable<THSIndexMemberDto>>(datas));

                return response;
            });
        }


        public override Task<Empty> SaveTHSIndexMembersByName(SaveTHSIndexMembersByNameRequest request, ServerCallContext context)
        {
            return Task.Run(() =>
            {

                List<THSIndexMember> members = new List<THSIndexMember>();
                foreach(var name in request.Names)
                {
                    var b=_basicStockInfoRepository.Entities.Where(p=>p.StockName== name).FirstOrDefault(); 
                    if(b!=null)
                    {
                        //查询是否存在
                        var isExist = _thsIndexMemberRepository.Entities.Where(
                             p => p.TSCode == request.TSCode && p.Code == b.TSCode).Any();
                        if(!isExist)
                        {
                            THSIndexMember member = new THSIndexMember();
                            member.TSCode = request.TSCode;
                            member.Name = name;
                            member.Code = b.TSCode;
                            member.IsNew = "N";

                            members.Add(member);
                        }
                    }

                    
                }
                if (members.Count > 0)
                {
                    _thsIndexMemberRepository.Save(members);
                }
                return new Empty();
            });
           

        }
    }
}
