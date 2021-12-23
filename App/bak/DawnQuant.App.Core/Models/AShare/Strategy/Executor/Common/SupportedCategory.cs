

namespace DawnQuant.App.Core.Models.AShare.Strategy.Executor.Common
{
    public class SupportedCategory
    {
        /// <summary>
        /// 分类
        /// </summary>
        public long CategoryId { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
