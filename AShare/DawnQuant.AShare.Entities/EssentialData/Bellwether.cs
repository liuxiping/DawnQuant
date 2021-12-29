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
    /// 龙头股
    /// </summary>  
    public class Bellwether : BaseEntity<long>
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
        /// 龙头股来源渠道 1同花顺公司亮点 ，2同花顺行业分析 营业额 利润前三
        /// </summary>
        public int Source { get; set; }


        /// <summary>
        /// 备注 
        /// </summary>
        
        [MaxLength(255)]
        public string Remark { get; set; }
    }
}
