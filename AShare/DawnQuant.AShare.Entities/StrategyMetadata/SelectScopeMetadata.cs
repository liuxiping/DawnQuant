
using DawnQuant.Repository;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DawnQuant.AShare.Entities.StrategyMetadata
{

    /// <summary>
    /// 选股范围
    /// </summary>
    public class SelectScopeMetadata : BaseEntity<long>
    {

        public override long GetKeyValue()
        {
            return Id;
        }


        public long Id { get; set; }

        public long CategoryId { get; set; }


        /// <summary>
        /// 显示名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 实现类名称
        /// </summary>
        public string ImplClassName { get; set; }

        /// <summary>
        /// 实现的程序集
        /// </summary>
        public string ImplAssemblyName { get; set; }


        /// <summary>
        /// 参数类名称
        /// </summary>
        public string ParameterClassName { get; set; }

        /// <summary>
        /// 参数所在的程序集
        /// </summary>
        public string ParameterAssemblyName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }


        public int SortNum { get; set; }

        [ForeignKey("CategoryId")]
        public SelectScopeMetadataCategory Category { get; set; }

    }
}
