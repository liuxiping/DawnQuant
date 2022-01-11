using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System.Collections.ObjectModel;
using DawnQuant.Passport;
using DawnQuant.App.Models;
using DawnQuant.AShare.Api.UserProfile;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.App.Utils;
using Autofac;
using DawnQuant.App.Models.AShare.EssentialData;
using Google.Protobuf.WellKnownTypes;

namespace DawnQuant.App.Services.AShare
{
    /// <summary>
    /// 同花顺指数服务
    /// </summary>
    public class THSIndexService
    {
        private readonly GrpcChannelSet _grpcChannelSet;
        private readonly IPassportProvider _passportProvider;
        private readonly IMapper _mapper;

        public THSIndexService()
        {

            _grpcChannelSet = IOCUtil.Container.Resolve<GrpcChannelSet>();
            _passportProvider = IOCUtil.Container.Resolve<IPassportProvider>();
            _mapper = IOCUtil.Container.Resolve<IMapper>(); ;
        }

        /// <summary>
        /// 获取所有同花顺指数
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<THSIndex> GetAllTHSIndex()
        {
            ObservableCollection<THSIndex> rdata = new ObservableCollection<THSIndex>();

            var client = new THSIndexApi.THSIndexApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

            var reponse = client.GetAllTHSIndexes(new Empty(), meta);

            if (reponse?.Entities?.Count > 0)
            {
                rdata = _mapper.Map<ObservableCollection<THSIndex>>(reponse.Entities.OrderByDescending(p => p.ListDate));
            }
            return rdata;

        }


        /// <summary>
        /// 获取所有同花顺指数成分股
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<THSIndexMember> GetTHSIndexMembersByTSCode(string tscode)
        {
            ObservableCollection<THSIndexMember> rdata = new ObservableCollection<THSIndexMember>();

            var client = new THSIndexMemberApi.THSIndexMemberApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

            var reponse = client.GetTHSIndexMembersByTSCode(new GetTHSIndexMembersByTSCodeRequest { TSCode=tscode}, meta);

            if (reponse?.Entities?.Count > 0)
            {
                rdata = _mapper.Map<ObservableCollection<THSIndexMember>>(reponse.Entities);
            }
            return rdata;

        }


        /// <summary>
        /// 当前指数加入重点关注题材热点
        /// </summary>
        /// <param name="tsCode"></param>
        public void  AddToSubjectAndHot(string tsCode)
        {
            var client = new THSIndexApi.THSIndexApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();
            meta.AddAuthorization(_passportProvider?.AccessToken);

            var reponse = client.AddToSubjectAndHot(new 
                AddToSubjectAndHotRequest { TSCode = tsCode,UserId=_passportProvider.UserId }, meta);
        }
    }
}
