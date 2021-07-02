using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DawnQuant.AShare.Strategy.Descriptor
{
    /// <summary>
    /// 选股因子实例描述
    /// </summary>
    
    public class FactorExecutorInsDescriptor
    {

        
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
