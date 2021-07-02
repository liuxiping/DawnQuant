using DawnQuant.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Entities
{
    /// <summary>
    /// 策略任务计划
    /// </summary>
    public class StrategyScheduledTask:BaseEntity<long>
    {
        public override long GetKeyValue()
        {
            return Id;
        }

        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 策略ID 中间以逗号分隔
        /// </summary>
        public string StrategyIds { get; set; }

        /// <summary>
        /// 输出自定义分类
        /// </summary>
        public long OutputStockCategoryId { get; set; }

        /// <summary>
        /// 策略计划任务名称
        /// </summary>
        [MaxLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(255)]
        public string Desc { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortNum { get; set; }

        /// <summary>
        /// 最近执行时间
        /// </summary>
        public DateTime? LatestExecuteTime { get; set; }


        /// <summary>
        /// 是否加入服务器服务器执行计划
        /// </summary>
        public bool IsJoinServerScheduleTask { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
