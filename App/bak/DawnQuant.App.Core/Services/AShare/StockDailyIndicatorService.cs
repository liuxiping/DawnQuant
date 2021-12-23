using AutoMapper;
using DawnQuant.App.Core.Models;
using DawnQuant.Passport;

namespace DawnQuant.App.Core.Services.AShare
{
    class StockDailyIndicatorService 
    {
        private readonly GrpcChannelSet _grpcChannelSet;
        private readonly IPassportProvider _passportProvider;
        private readonly IMapper _mapper;


        public StockDailyIndicatorService(GrpcChannelSet grpcChannelSet,
          IPassportProvider passportProvider, IMapper mapper)
        {
            _grpcChannelSet = grpcChannelSet;
            _passportProvider = passportProvider;
            _mapper = mapper;
        }
    }
}
