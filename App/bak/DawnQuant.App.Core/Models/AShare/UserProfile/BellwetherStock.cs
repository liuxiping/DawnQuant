

namespace DawnQuant.App.Core.Models.AShare.UserProfile
{
    /// <summary>
    /// 龙头股
    /// </summary>
    public class BellwetherStock
    {
     

        /// <summary>
        /// 主键
        /// </summary>
       
        public long Id { get; set; }

        public long UserId { get; set; }

        public long CategoryId { get; set; }

        /// <summary>
        /// 股票代码
        /// </summary>
     
        public string TSCode { get; set; }

        /// <summary>
        /// 股票名称
        /// </summary>
     
        public string Name { get; set; }


        /// <summary>
        /// 排序
        /// </summary>
        public int SortNum { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
