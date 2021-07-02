using DawnQuant.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DawnQuant.AShare.Entities.StrategyMetadata
{
    /// <summary>
    /// 股票策略因子
    /// </summary>
    public class FactorMetadata : BaseEntity<long>
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
        /// 实现类的程序集
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

        /// <summary>
        /// 排序
        /// </summary>
        public int SortNum { get; set; }


        [ForeignKey("CategoryId")]
        public FactorMetadataCategory Category { get; set; }

    }
}
