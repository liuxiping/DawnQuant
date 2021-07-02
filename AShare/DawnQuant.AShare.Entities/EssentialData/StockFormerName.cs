
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
    /// 股票曾用名
    /// </summary>
    public class StockFormerName: BaseEntity<int>
    {
        public override int GetKeyValue()
        {
            return Id;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// TS_Code
        /// </summary>
        [MaxLength(50)]
        public string TSCode { get; set; }

        /// <summary>
        /// 股票简称
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string StockName { get; set; }


        /// <summary>
        /// 开始日期
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }


        /// <summary>
        /// 结束日期
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }


        /// <summary>
        /// 公告日期
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? AnnounceDate { get; set; }


        /// <summary>
        /// 变更原因
        /// </summary>
        [MaxLength(255)]
        public string ChangeReason { get; set; }

    }
}
