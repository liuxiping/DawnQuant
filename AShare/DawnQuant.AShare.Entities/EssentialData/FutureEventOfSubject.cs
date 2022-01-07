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
    /// 题材前瞻
    /// </summary>  
    public class FutureEventOfSubject : BaseEntity<long>
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
        /// 时间
        /// </summary>
        public DateTime  Date { get; set; }

        /// <summary>
        /// 事件前瞻
        /// </summary>
        [MaxLength(255)]
        public string Event { get; set; }


        /// <summary>
        /// 影响题材
        /// </summary>
        [MaxLength(255)]
        public string Subject { get; set; }


        /// <summary>
        /// 影响股票 逗号分割
        /// </summary>

       
        public string RelateStocks { get; set; }
    }
}
