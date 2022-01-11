using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DawnQuant.Repository;

namespace DawnQuant.AShare.Entities.EssentialData
{


    /// <summary>
    /// 同花顺概念和行业指数
    /// </summary>
    public class THSIndex : BaseEntity<string>
    {
        public override string GetKeyValue()
        {
            return TSCode;
        }


        /// <summary>
        /// 代码
        /// </summary>
        public string TSCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 成分个数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 交易所
        /// </summary>
        [MaxLength(50)]
        public string Exchange { get; set; }

        /// <summary>
        /// 上市日期
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? ListDate { get; set; }


        /// <summary>
        /// N概念指数S特色指数
        /// </summary>
        public string Type { get; set; }





    }
}
