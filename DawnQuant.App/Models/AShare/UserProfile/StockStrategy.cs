using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Models.AShare.UserProfile
{
    /// <summary>
    /// 用户选股策略
    /// </summary>
    public class StockStrategy 
    {
        
        /// <summary>
        /// 主键
        /// </summary>
        public long Id { get; set; }

        public long UserId { get; set; }
       
        public long CategoryId { get; set; }
       
        /// <summary>
        /// 策略名称
        /// </summary>
        public string  Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortNum { get; set; }

        /// <summary>
        /// 策略内容 json序列化方式存储
        /// </summary>
        public string StockStragyContent { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

      
    }
}
