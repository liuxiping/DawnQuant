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
    /// 上市公司基本信息
    /// </summary>
    public class Company: BaseEntity<string>
    {
        public override string GetKeyValue()
        {
            return TSCode;
        }

        /// <summary>
        /// 股票代码/公司代码
        /// </summary>
        [Key]
        public string TSCode { get; set; }

        /// <summary>
        /// 交易所代码 ，SSE上交所 SZSE深交所
        /// </summary>
        [MaxLength(50)]
        public string Exchange { get; set; }

        /// <summary>
        /// 法人代表
        /// </summary>
        public string Chairman { get; set; }

        /// <summary>
        /// 总经理
        /// </summary>
        public string GeneralManager { get; set; }

        /// <summary>
        /// 董事会秘书
        /// </summary>
        public string Secretary { get; set; }

        /// <summary>
        /// 注册资本
        /// </summary>
        public double RegisteredCapital { get; set; }


        /// <summary>
        /// 成立日期
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime EstablishmentDate { get; set; }


        /// <summary>
        /// 所在省份
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 所在城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string BriefIntroduction { get; set; }

        /// <summary>
        /// 公司网址
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }


        /// <summary>
        /// 公司办公地址
        /// </summary>
        public string OfficeAddress { get; set; }


        /// <summary>
        /// 员工人数
        /// </summary>
        public int EmployeeCount { get; set; }


        /// <summary>
        /// 经营范围
        /// </summary>
        public string BusinessScope { get; set; }

        /// <summary>
        /// 主营业务
        /// </summary>
        public string MainBusiness { get; set; }

     
    }
}
