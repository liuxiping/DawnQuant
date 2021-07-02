using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TuShareHttpSDKLibrary;
using TuShareHttpSDKLibrary.Model.BasicData;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Grpc.Core;
using DawnQuant.Passport;
using DawnQuant.DataCollector.Config;
using DawnQuant.AShare.Api.EssentialData;

namespace DawnQuant.DataCollector.Collectors.AShare
{
    /// <summary>
    /// 公司信息收集
    /// </summary>
    public class CompanyCollector 
    {

        public CompanyCollector(ILogger logger, CollectorConfig config,
           IPassportProvider passportProvider
            )
        {
            _logger = logger;
            _passportProvider = passportProvider;

            _tushareToken = config.TushareToken;
            _tushareUrl = config.TushareUrl;
            _apiUrl = config.AShareApiUrl;
        }

        string _tushareToken;
        string _tushareUrl;
        string _apiUrl;

        ILogger _logger;
        IPassportProvider _passportProvider;

        /// <summary>
        /// 收集上市公司基本信息
        /// </summary>
        /// <param name="id"></param>
        public void CollectAllCompanyInfo( )
        {

            TuShare tu = new TuShare(_tushareUrl, _tushareToken);

            StockCompanyRequestModel requestModelSSE = new StockCompanyRequestModel();
            requestModelSSE.Exchange = StockEntityConst.SSE;
            var taskSSE = tu.GetData(requestModelSSE);
            taskSSE.Wait();

            SaveCompanyInfo(taskSSE.Result);

            StockCompanyRequestModel requestModelSZSE = new StockCompanyRequestModel();
            requestModelSZSE.Exchange = StockEntityConst.SZSE;
            var taskSZSE = tu.GetData(requestModelSZSE);
            taskSZSE.Wait();

            SaveCompanyInfo(taskSZSE.Result);
           

        }


        private void SaveCompanyInfo(List<StockCompanyResponseModel> result)
        {
            SaveCompanysRequest request = new SaveCompanysRequest();

            foreach (var item in result)
            {
                CompanyDto company = new CompanyDto();

                company.TSCode = item.TsCode.ToUpper();
                company.Exchange = item.Exchange ?? "";
                company.Chairman = item.Chairman ?? "";
                company.GeneralManager = item.Manager ?? "";
                company.Secretary = item.Secretary ?? "";
                company.RegisteredCapital = item.RegCapital;

                DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                dtFormat.ShortDatePattern = "yyyyMMdd";

                company.EstablishmentDate=Timestamp.FromDateTime(
                  DateTime.SpecifyKind(  DateTime.ParseExact(item.SetupDate, "yyyyMMdd", dtFormat),
                  DateTimeKind.Utc));


                company.Province = item.Province ?? "";
                company.City = item.City ?? "";
                company.BriefIntroduction = item.Introduction ?? "";
                company.Website = item.Website ?? "";
                company.Email = item.Email ?? "";
                company.OfficeAddress = item.Office ?? "";
                company.EmployeeCount = item.Employees ;
                company.BusinessScope = item.BusinessScope ?? "";
                company.MainBusiness = item.MainBusiness ?? "";
                request.Entities.Add(company);
            }

            if (request.Entities.Count > 0)
            {
                GrpcChannel channel = null;

                try
                {
                    channel = GrpcChannel.ForAddress(_apiUrl,new GrpcChannelOptions { 
                        MaxReceiveMessageSize= null,
                   });

                    var client = new CompanyApi.CompanyApiClient(channel);

                    Metadata meta = new Metadata();
                    meta.AddAuthorization(_passportProvider.AccessToken);

                    client.SaveCompanys(request, meta);
                }
                finally
                {
                    if (channel != null)
                    {
                        channel.Dispose();

                    }
                }
            }
        }

      

    }
}
