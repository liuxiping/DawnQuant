
using DawnQuant.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Entities.EssentialData
{
    /// <summary>
    /// 复权因子，每日计算
    /// </summary>
    public class ReversionFactor : BaseEntity<DateTime>
    {
        public override DateTime GetKeyValue()
        {
            return TradeDate;
        }

        /// <summary>
        /// 交易时间
        /// </summary>
        [Key]
        public DateTime TradeDate { get; set; }

        /// <summary>
        /// 前复权因子
        /// </summary>
        public double  Before { get; set; }


        /// <summary>
        /// 后复权因子
        /// </summary>
        public double After { get; set; }
    }

}
