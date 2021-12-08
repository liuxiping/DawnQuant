
using System;
using System.Text.Json;

namespace DawnQuant.App.Models.AShare.Strategy.Descriptor
{

    /// <summary>
    /// 选股范围描述
    /// </summary>
   
    public class SelectScopeExecutorInsDescriptor
    {
        public SelectScopeExecutorInsDescriptor(long id ,string p)
        {
            MetadataId = id;
            Parameter = p;
        }

        public SelectScopeExecutorInsDescriptor()
        {

        }
        /// <summary>
        /// 元数据标识
        /// </summary>
        
        public long MetadataId { get; set; }

        /// <summary>
        /// 序列化的参数
        /// </summary>
        public string  Parameter { get; set; }


    }
}
