using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DawnQuant.App.Models.AShare.UserProfile
{
    /// <summary>
    /// 自选股票分类
    /// </summary>
    public class SelfSelectStockCategory
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
        /// 根据行业自动分组
        /// </summary>
        public bool IsGroupByIndustry { get; set; }


        /// <summary>
        /// 分类列表默认排序字段   1 加入时间   2 行业
        /// </summary>
        public int StockSortFiled { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public List<SelfSelectStock> SelfSelectStocks { get; set; }
    }
}
