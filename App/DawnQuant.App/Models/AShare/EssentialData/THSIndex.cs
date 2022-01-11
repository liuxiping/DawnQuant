using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Models.AShare.EssentialData
{


    /// <summary>
    /// 同花顺概念和行业指数
    /// </summary>
    public class THSIndex 
    {

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
        public string Exchange { get; set; }

        /// <summary>
        /// 上市日期
        /// </summary>
        public DateTime? ListDate { get; set; }


        /// <summary>
        /// N概念指数S特色指数
        /// </summary>
        public string Type { get; set; }

    }


    /// <summary>
    /// 指数类型
    /// </summary>
    public class THSIndexType
    {

        /// <summary>
        /// 指数类型 N-板块指数 I-行业指数 S-同花顺特色指数 A-
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }


        static THSIndexType()
        {
            _allSupportType = new List<THSIndexType>();
            _allSupportType.Add(new THSIndexType() { Name = "所有指数", Code = "A" });
            _allSupportType.Add(new THSIndexType() { Name = "板块指数", Code = "N" });
            _allSupportType.Add(new THSIndexType() { Name = "行业指数", Code = "I" });
            _allSupportType.Add(new THSIndexType() { Name = "特色指数", Code = "S" });

        }

        /// <summary>
        /// 所有支持的类型
        /// </summary>
        static List<THSIndexType> _allSupportType;
        public static List<THSIndexType> AllSupportType { get { return _allSupportType; } }

    }
}
