using AutoMapper;
using DawnQuant.App.Models;
using DawnQuant.Passport;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Services.AShare
{
    public class SelfSelStockItemService 
    {
        private readonly ILogger<SelfSelStockItemService> _logger;
        private readonly GrpcChannelSet _grpcChannelSet;
        private readonly IPassportProvider _passportProvider;
        private readonly IMapper _mapper;

        public SelfSelStockItemService(ILogger<SelfSelStockItemService> logger,
            GrpcChannelSet grpcChannelSet, IPassportProvider passportProvider,
            IMapper mapper)
        {
            _logger = logger;
            _grpcChannelSet = grpcChannelSet;
            _passportProvider = passportProvider;
            _mapper = mapper;
        }

       
    }
}
