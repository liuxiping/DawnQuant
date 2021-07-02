using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Models
{
    /// <summary>
    /// GRPC 通道集合
    /// </summary>
    public class GrpcChannelSet
    {
        public GrpcChannel AShareGrpcChannel { get; set; }
    }
}
