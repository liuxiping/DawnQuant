using AutoMapper;
using DawnQuant.App.Core.Models.AShare.EssentialData;
using DawnQuant.App.Core.Models.AShare.StrategyMetadata;
using DawnQuant.App.Core.Models.AShare.UserProfile;
using DawnQuant.AShare.Api.EssentialData;
using DawnQuant.AShare.Api.StrategyMetadata;
using DawnQuant.AShare.Api.UserProfile;
using DawnQuant.AutoMapper.Converter;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.DependencyInjection;
using static DawnQuant.App.Core.Views.AShare.Common.StockList;

namespace DawnQuant.App.Core.Utils
{
    /// <summary>
    /// 自动映射注册
    /// </summary>
    public static class AutoMapperExtension
    {
        public static void AddAutoMapper(this ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IMapper>((c) =>
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

                    config.CreateMap<SelfSelectStock, Stock>().ForMember("AddDate", opt => { opt.MapFrom("CreateTime"); });
                   

                });
                return config.CreateMapper();

            });
        }
    }
}
