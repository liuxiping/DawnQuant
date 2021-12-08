﻿

namespace DawnQuant.App.Core.Models.AShare.UserProfile
{
    /// <summary>
    /// 题材热点股票分类
    /// </summary>
    public class SubjectAndHotStockCategory 
    {
       

        /// <summary>
        /// 主键ID
        /// </summary>
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
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }


        /// <summary>
        /// 题材股
        /// </summary>
        public List<SubjectAndHotStock> SubjectAndHotStocks { get; set; }
    }
}
