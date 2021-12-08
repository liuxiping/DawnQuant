using Autofac;
using AutoMapper;
using DawnQuant.App.Models.AShare;
using DawnQuant.App.Models.AShare.EssentialData;
using DawnQuant.App.Models.AShare.Strategy;
using DawnQuant.App.Models.AShare.StrategyMetadata;
using DawnQuant.App.Models.AShare.UserProfile;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.AShare.Api.StrategyMetadata;
using DawnQuant.AShare.Api.UserProfile;
using DawnQuant.AutoMapper.Converter;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DawnQuant.App.Utils
{
    public static class AutoMapperExtension
    {
        public static void AddAutoMapper(this ContainerBuilder builder)
        {
            builder.Register<IMapper>((c) =>
            {
                var config = new MapperConfiguration(config =>
                {
                    config.CreateMap<string, string>().ConvertUsing((s, d) =>
                    {
                        return s ?? "";
                    });

                    config.CreateMap<DateTime, Timestamp>().ConvertUsing(new DateTimeToGrpcTimestampConverter());
                    config.CreateMap<Timestamp, DateTime>().ConvertUsing(new GrpcTimestampToDateTimeConverter());
                    config.CreateMap<DateTime?, Timestamp>().ConvertUsing(new NullableDateTimeToGrpcTimestampConverter());
                    config.CreateMap<Timestamp, DateTime?>().ConvertUsing(new GrpcTimestampToNullableDateTimeConverter());
                    config.CreateMap<ByteString, Byte[]>().ConvertUsing(new ByteStringToByteArrayConverter());
                    config.CreateMap<Byte[], ByteString>().ConvertUsing(new ByteArrayToByteStringConverter());


                  //  config.CreateMap<StockDailyIndicator, StockDailyIndicatorDto>().ReverseMap();


                    config.CreateMap<SelfSelectStockCategory, SelfSelectStockCategoryDto>().ReverseMap();
                    config.CreateMap<SelfSelectStock, SelfSelectStockDto>().ReverseMap();
                    config.CreateMap<SelfSelectStock, RelatedStockItem>().ReverseMap();

                    config.CreateMap<BellwetherStockCategory, BellwetherStockCategoryDto>().ReverseMap();
                    config.CreateMap<BellwetherStock, BellwetherStockDto>().ReverseMap();
                    config.CreateMap<BellwetherStock, RelatedStockItem>().ReverseMap();


                    config.CreateMap<SubjectAndHotStockCategory, SubjectAndHotStockCategoryDto>().ReverseMap();
                    config.CreateMap<SubjectAndHotStock, SubjectAndHotStockDto>().ReverseMap();
                    config.CreateMap<SubjectAndHotStock, RelatedStockItem>().ReverseMap();


                    config.CreateMap<StockStrategy, StockStrategyDto>().ReverseMap();
                    config.CreateMap<StockStrategyCategory, StockStrategyCategoryDto>().ReverseMap();
                    config.CreateMap<StrategyScheduledTask, StrategyScheduledTaskDto>().ReverseMap();


                    config.CreateMap<FactorMetadataDto, FactorMetadata>().ReverseMap();
                    config.CreateMap<FactorMetadataCategoryDto, FactorMetadataCategory>().ReverseMap();
                    config.CreateMap<SelectScopeMetadataDto, SelectScopeMetadata>().ReverseMap();
                    config.CreateMap<SelectScopeMetadataCategoryDto, SelectScopeMetadataCategory>().ReverseMap();


                    config.CreateMap<StockTradeDataDto, StockTradeData>().ReverseMap();
                    config.CreateMap<StockTradeData, StockPlotData>();
                    config.CreateMap<AdjustedStateDto, AdjustedState>();
                    config.CreateMap<KCycleDto, KCycle>();




                });

                return config.CreateMapper();

            });
        }
    }
}
