

namespace DawnQuant.App.Core.Models.AShare.StrategyMetadata
{

    /// <summary>
    /// 选股范围
    /// </summary>
    public class SelectScopeMetadata 
    {

       

        public long Id { get; set; }

        public long CategoryId { get; set; }


        /// <summary>
        /// 显示名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 实现类名称
        /// </summary>
        public string ImplClassName { get; set; }

        /// <summary>
        /// 实现的程序集
        /// </summary>
        public string ImplAssemblyName { get; set; }


        /// <summary>
        /// 参数类名称
        /// </summary>
        public string ParameterClassName { get; set; }

        /// <summary>
        /// 参数所在的程序集
        /// </summary>
        public string ParameterAssemblyName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }


        public int SortNum { get; set; }

        public SelectScopeMetadataCategory Category { get; set; }

    }
}
