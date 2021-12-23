using AutoMapper;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.AShare.Api.StrategyExecutor;
using DawnQuant.AShare.Api.StrategyMetadata;
using DawnQuant.AShare.Api.UserProfile;
using DawnQuant.AShare.Entities;
using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Entities.StrategyMetadata;
using DawnQuant.AShare.Entities.UserProfile;
using DawnQuant.AutoMapper.Converter;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using System;


namespace DawnQuant.AShare.Api.Utils
{
    public static class AutoMapperExtensions
    {
        public static void AddMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(config =>
              {

                  config.CreateMap<string, string>().ConvertUsing((s, d) =>
                  {
                      return s ?? "";
                  });

                  config.CreateMap<DateTime, Timestamp>().ConvertUsing(new DateTimeToGrpcTimestampConverter());
                  config.CreateMap<Timestamp, DateTime>().ConvertUsing(new GrpcTimestampToDateTimeConverter());
                  config.CreateMap<DateTime?, Timestamp>().ConvertUsing(new NullableDateTimeToGrpcTimestampConverter());
                  config.CreateMap<Timestamp, DateTime?>().ConvertUsing(new GrpcTimestampToNullableDateTimeConverter());

                  config.CreateMap<KCycleDto, KCycle>().ReverseMap();

                  config.CreateMap<BasicStockInfoDto, BasicStockInfo>().ReverseMap();
                  config.CreateMap<CompanyDto, Company>().ReverseMap();
                  config.CreateMap<TradingCalendarDto, TradingCalendar>().ReverseMap();
                  config.CreateMap<IndustryDto, Industry>().ReverseMap();
                  config.CreateMap<StockTradeDataDto, StockTradeData>().ReverseMap();
                  config.CreateMap<StockDailyIndicatorDto, StockDailyIndicator>().ReverseMap();
                  config.CreateMap<HolderNumberDto, HolderNumber>().ReverseMap();
                  config.CreateMap<Top10FloatHolder, Top10FloatHolderDto>().ReverseMap();


                  config.CreateMap<FactorMetadataDto, FactorMetadata>().ReverseMap();
                  config.CreateMap<FactorMetadataCategoryDto, FactorMetadataCategory>().ReverseMap();
                  config.CreateMap<SelectScopeMetadataDto, SelectScopeMetadata>().ReverseMap();
                  config.CreateMap<SelectScopeMetadataCategoryDto, SelectScopeMetadataCategory>().ReverseMap();

               


                  config.CreateMap<DateTime, Timestamp>().ConvertUsing(new DateTimeToGrpcTimestampConverter());
                  config.CreateMap<Timestamp, DateTime>().ConvertUsing(new GrpcTimestampToDateTimeConverter());
                  config.CreateMap<DateTime?, Timestamp>().ConvertUsing(new NullableDateTimeToGrpcTimestampConverter());
                  config.CreateMap<Timestamp, DateTime?>().ConvertUsing(new GrpcTimestampToNullableDateTimeConverter());

                  config.CreateMap<ByteString, Byte[]>().ConvertUsing(new ByteStringToByteArrayConverter());
                  config.CreateMap<Byte[], ByteString>().ConvertUsing(new ByteArrayToByteStringConverter());

                  config.CreateMap<string, string>().ConvertUsing((s, d) =>
                  {
                      return s ?? "";
                  });

                  config.CreateMap<SelfSelectStockCategory, SelfSelectStockCategoryDto>().ReverseMap();
                  config.CreateMap<SelfSelectStock, SelfSelectStockDto>().ReverseMap();
                  config.CreateMap<BellwetherStockCategory, BellwetherStockCategoryDto>().ReverseMap();
                  config.CreateMap<BellwetherStock, BellwetherStockDto>().ReverseMap();

                  config.CreateMap<SubjectAndHotStockCategory, SubjectAndHotStockCategoryDto>().ReverseMap();
                  config.CreateMap<SubjectAndHotStock, SubjectAndHotStockDto>().ReverseMap();


                  config.CreateMap<ExecuteStrategyResult, SelfSelectStock>().ReverseMap();


                  config.CreateMap<StockStrategy, StockStrategyDto>().ReverseMap();
                  config.CreateMap<StockStrategyCategory, StockStrategyCategoryDto>().ReverseMap();
                  config.CreateMap<StrategyScheduledTask, StrategyScheduledTaskDto>().ReverseMap();
                  config.CreateMap<ExclusionStock, ExclusionStockDto>().ReverseMap();



              });
        }
    }
}
