using AutoMapper;
using DawnQuant.App.Core.Models;
using DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.App.Core.Utils;
using DawnQuant.AShare.Api.StrategyExecutor;
using DawnQuant.Passport;
using Grpc.Core;
using System.Collections.ObjectModel;

namespace DawnQuant.App.Core.Services.AShare
{
    public class StrategyExecutorService
    {
        private readonly GrpcChannelSet _grpcChannelSet;
        private readonly IPassportProvider _passportProvider;
        private readonly IMapper _mapper;


        public StrategyExecutorService(GrpcChannelSet grpcChannelSet,
            IPassportProvider passportProvider, IMapper mapper)
        {
            _grpcChannelSet = grpcChannelSet;
            _passportProvider = passportProvider;
            _mapper = mapper;
        }

        /// <summary>
        /// 执行策略
        /// </summary>
        /// <param name="strategyContent"></param>
        /// <returns></returns>
        public ObservableCollection<SelfSelectStock> ExecuteStrategyByContent(string strategyContent)
        {
            var client = new StrategyExecutorApi.StrategyExecutorApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();

            meta.AddAuthorization(_passportProvider?.AccessToken);
            ExecuteStrategyByContentRequest request = new ExecuteStrategyByContentRequest();
            request.StrategyContent = strategyContent;

            var dtos = client.ExecuteStrategyByContent(request, meta);

            ObservableCollection<SelfSelectStock> stocks = new ObservableCollection<SelfSelectStock>();

            foreach (var item in dtos.Entities)
            {
                stocks.Add(new SelfSelectStock { TSCode = item.TSCode, Name = item.Name, Industry = item.Industry });
            }

            return stocks;

            
        }

        /// <summary>
        /// 执行策略
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ObservableCollection<SelfSelectStock> ExecuteStrategyById(long id)
        {
            var client = new StrategyExecutorApi.StrategyExecutorApiClient(_grpcChannelSet.AShareGrpcChannel);

            Metadata meta = new Metadata();

            meta.AddAuthorization(_passportProvider?.AccessToken);
            ExecuteStrategyByIdRequest request = new ExecuteStrategyByIdRequest();
            request.StrategyId=id;

            var dtos = client.ExecuteStrategyById(request, meta);

            ObservableCollection<SelfSelectStock> stocks = new ObservableCollection<SelfSelectStock>();

            foreach (var item in dtos.Entities)
            {
                stocks.Add(new SelfSelectStock { TSCode = item.TSCode, Name = item.Name, Industry = item.Industry });
            }

            return stocks;
        }
    }
}
