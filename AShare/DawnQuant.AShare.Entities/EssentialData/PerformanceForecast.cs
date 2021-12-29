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
    /// 业绩预测
    /// </summary>
    public class PerformanceForecast : BaseEntity<long>
    {
        public override long GetKeyValue()
        {
            return Id;
        }

        /// <summary>
        /// Id
        /// </summary>
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// TS_Code
        /// </summary>
        public string TSCode { get; set; }

        /// <summary>
        /// 截止日期
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }


        /// <summary>
        /// 预测机构数量
        /// </summary>
        public int ForecastOrgCount { get; set; }

        /// <summary>
        /// 每股收益
        /// </summary>
        public  double EarningsPerShare { get; set; }

        /// <summary>
        /// 每股收益变动比率
        /// </summary>
        public double EarningsPerShareChangeRatio { get; set; }


        /// <summary>
        /// 净利润
        /// </summary>
        public double RetainedProfits { get; set; }

        /// <summary>
        /// 净利润变动比率
        /// </summary>
        public double RetainedProfitsChangeRatio { get; set; }

        /// <summary>
        /// 龙头股来源渠道 1同花顺F10
        /// </summary>
        public int Source { get; set; }
    }
    
}
