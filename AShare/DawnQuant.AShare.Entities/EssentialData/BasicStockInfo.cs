
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
    /// 股票基本信息
    /// </summary>
    public class BasicStockInfo:BaseEntity<string>
    {
        public override string GetKeyValue()
        {
            return TSCode;
        }

        /// <summary>
        /// TS_Code
        /// </summary>
        [Key]
        public string TSCode { get; set; }


        /// <summary>
        /// 股票代码/公司代码
        /// </summary>
        public string StockCode { get; set; }

        /// <summary>
        /// 股票简称
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string StockName { get; set; }


        /// <summary>
        /// 公司名称全称
        /// </summary>
        [MaxLength(255)]
        public string FullName { get; set; }

        /// <summary>
        /// 公司英文全称
        /// </summary>
        [MaxLength(255)]
        public string EnFullName { get; set; }


        /// <summary>
        /// 第一级行业
        /// </summary>
        [MaxLength(50)]
        public string PrimaryIndustry { set; get; }

        /// <summary>
        /// 三级行业，第三级行业
        /// </summary>
        public int IndustryId { set; get; }

        /// <summary>
        /// 上市日期
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime ListingDate { get; set; }

        /// <summary>
        /// 退市日期
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? DelistingDate { get; set; }

        /// <summary>
        ///  上市状态 上市 退市 暂停上市
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string ListedStatus { get; set; }


        /// <summary>
        /// 所属区域
        /// </summary>
        [MaxLength(50)]
        public string Area { get; set; }


        /// <summary>
        /// 市场类型
        /// </summary>
        [MaxLength(50)]
        public string MarketType { get; set; }


        /// <summary>
        /// 交易所代码
        /// </summary>
        [MaxLength(50)]
        public string Exchange { get; set; }


        /// <summary>
        /// 交易货币
        /// </summary>
        [MaxLength(50)]
        public string Currency { get; set; }



        /// <summary>
        /// 是否沪深港通标的，N否 H沪股通 S深股通
        /// </summary>
        [MaxLength(50)]
        public string StockConnect  { get; set; }
    }
}
