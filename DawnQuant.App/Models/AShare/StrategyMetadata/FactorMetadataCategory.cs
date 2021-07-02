
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Models.AShare.StrategyMetadata
{
    public class FactorMetadataCategory
    {
        

        /// <summary>
        /// 主键ID
        /// </summary>
        public long Id { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        public int SortNum { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }


        public List<FactorMetadata>  FactorMetadatas { set; get; }


    }
}
