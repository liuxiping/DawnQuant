using AutoMapper;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AutoMapper.Converter
{
    public class ByteStringToByteArrayConverter : ITypeConverter<ByteString, byte[]>
    {
        public byte[] Convert(ByteString source, byte[] destination, ResolutionContext context)
        {
           if(source==null || source.Length<=0)
            {
                return null;
            }
           else
            {
                return source.ToByteArray();
            }
        }
    }

    public class ByteArrayToByteStringConverter : ITypeConverter<byte[],ByteString>
    {
        public ByteString Convert(byte[] source, ByteString destination, ResolutionContext context)
        {
            if(source==null)
            {
                return  ByteString.Empty; ;
            }
            else
            {
               return ByteString.CopyFrom(source);
            }
        }
    }
}
