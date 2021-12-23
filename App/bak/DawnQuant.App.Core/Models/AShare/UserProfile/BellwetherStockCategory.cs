
using System.ComponentModel.DataAnnotations;


namespace DawnQuant.App.Core.Models.AShare.UserProfile
{
    /// <summary>
    /// 龙头股分类
    /// </summary>
    public class BellwetherStockCategory 
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
        [MaxLength(255)]
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
        /// 龙头股
        /// </summary>
        public List<BellwetherStock> BellwetherStocks { get; set; }
    }
}
