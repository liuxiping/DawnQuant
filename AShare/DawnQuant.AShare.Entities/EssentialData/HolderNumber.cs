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
    /// 股东人数
    /// </summary>
    public class HolderNumber : BaseEntity<long>
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
        /// 公告日期
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime ReportingPeriod { get; set; }
     

        /// <summary>
        /// 股东人数(单位万)
        /// </summary>
        public double HolderNum { get; set; }


        /// <summary>
        /// 较上期变化
        /// </summary>
        public double Change { get; set; }
    }
}
