using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using System;

namespace DawnQuant.AutoMapper.Converter
{
    public class GrpcTimestampToDateTimeConverter : ITypeConverter<Timestamp, DateTime>
    {
        public DateTime Convert(Timestamp source, DateTime destination, ResolutionContext context)
        {
            return source.ToDateTime();
        }
    }

    public class DateTimeToGrpcTimestampConverter : ITypeConverter<DateTime, Timestamp>
    {
        public Timestamp Convert(DateTime source, Timestamp destination, ResolutionContext context)
        {
            return Timestamp.FromDateTime(DateTime.SpecifyKind(source, DateTimeKind.Utc));
        }
    }


    public class GrpcTimestampToNullableDateTimeConverter : ITypeConverter<Timestamp, DateTime?>
    {
    

        public DateTime? Convert(Timestamp source, DateTime? destination, ResolutionContext context)
        {
            if (source == null)
            {
                return null;
            }
            else
            {
                return source.ToDateTime();
            }
        }
    }

    public class NullableDateTimeToGrpcTimestampConverter : ITypeConverter<DateTime?, Timestamp>
    {
        public Timestamp Convert(DateTime? source, Timestamp destination, ResolutionContext context)
        {
            if (source == null)
            {
                return null;
            }
            else
            {
                return Timestamp.FromDateTime(DateTime.SpecifyKind(source.Value, DateTimeKind.Utc));
            }
        }
    }
}
