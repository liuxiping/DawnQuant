using Grpc.Net.Client;

namespace DawnQuant.App.Core.Models
{
    /// <summary>
    /// GRPC 通道集合
    /// </summary>
    public class GrpcChannelSet
    {
        public GrpcChannel AShareGrpcChannel { get; set; }
    }
}
