using DawnQuant.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DawnQuant.AShare.Entities.UserProfile
{
    /// <summary>
    /// 自选股票分类
    /// </summary>
    public class SelfSelectStockCategory:BaseEntity<long>
    {

        

        public override long GetKeyValue()
        {
            return Id;
        }

        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortNum { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [MaxLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(255)]
        public string Desc { get; set; }

        /// <summary>
        /// 根据行业自动分组
        /// </summary>
        public bool IsGroupByIndustry { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        


        [ForeignKey("CategoryId")]
        public List<SelfSelectStock> SelfSelectStocks { get; set; }
    }
}
