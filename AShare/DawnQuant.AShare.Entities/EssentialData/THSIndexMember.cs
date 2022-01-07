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
    /// 同花顺概念和行业指数成员
    /// </summary>
    public class THSIndexMember : BaseEntity<long>
    {
        public override long GetKeyValue()
        {
            return Id;
        }

        [Key]
        public long Id { get; set; }

        /// <summary>
        /// 指数代码
        /// </summary>
       
        public string TSCode { get; set; }


       /// <summary>
       /// 股票代码
       /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 股票名称
        /// </summary>
        public string Name { get; set; }

        
        /// <summary>
        /// 权重
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// 纳入日期
        /// </summary>
        public DateTime InDate { get; set; }

        /// <summary>
        /// 剔除日期
        /// </summary>
        public DateTime OutDate { get; set; }


        /// <summary>
        /// 是否最新Y是N否
        /// </summary>
        public string IsNew { get; set; }





















    }
}
