
namespace DawnQuant.App.Core.Models.AShare.Strategy.Descriptor
{
    /// <summary>
    /// 选股因子实例描述
    /// </summary>
    
    public class FactorExecutorInsDescriptor
    {

        public FactorExecutorInsDescriptor(long id, string p)
        {
            MetadataId = id;
            Parameter = p;
        }

        public FactorExecutorInsDescriptor()
        {

        }
        /// <summary>
        /// /元数据标识
        /// </summary>
        public long MetadataId { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public string Parameter { get; set; }

        
    }
}
