

namespace DawnQuant.App.Core.Models.AShare.StrategyMetadata
{
    public class SelectScopeMetadataCategory 
    {
       
        /// <summary>
        /// 主键ID
        /// </summary>
        public long Id { get; set; }


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


        public List<SelectScopeMetadata> SelectScopeMetadatas { set; get; }
    }
}
