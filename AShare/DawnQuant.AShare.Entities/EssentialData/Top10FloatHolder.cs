using DawnQuant.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Entities.EssentialData
{
    /// <summary>
    /// 前十大流通股
    /// </summary>
    public class Top10FloatHolder: BaseEntity<long>
    {
        public override long GetKeyValue()
        {
            return Id;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }


        public string TSCode { get; set; }


        /// <summary>
        /// 报告期
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 股东名称
        /// </summary>
        public string HolderName { get; set; }

        /// <summary>
        /// 持股数量
        /// </summary>
        public double HoldAmount { get; set; }


        /// <summary>
        /// 持股比例
        /// </summary>
        public double HoldRatio { get; set; }


        /// <summary>
        /// 持股变化（股）
        /// </summary>
        public double HoldChange { get; set; }


        /// <summary>
        /// 变动性质:1 不变，2新进，3增加，4减少
        /// </summary>
        public int HoldChangeCharacter { get; set; }
    }

   
}
