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
    /// 交易日历
    /// </summary>
    public class TradingCalendar: BaseEntity<int>
    {

        public override int GetKeyValue()
        {
            return Id;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }


        /// <summary>
        /// 前一个交易日
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? PreDate { get; set; }

        /// <summary>
        /// 是否开市
        /// </summary>
        [Required]
        public bool IsOpen { get; set; }


        /// <summary>
        /// 证券交易所类型
        /// </summary>
        public string Exchange { get; set; }
    }
}
