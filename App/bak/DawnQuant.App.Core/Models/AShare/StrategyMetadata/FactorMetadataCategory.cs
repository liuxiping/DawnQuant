using System.ComponentModel.DataAnnotations;


namespace DawnQuant.App.Core.Models.AShare.StrategyMetadata
{
    public class FactorMetadataCategory
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
        [MaxLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }


        public List<FactorMetadata>  FactorMetadatas { set; get; }


    }
}
