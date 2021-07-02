using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Strategy.Common
{
    /// <summary>
    /// 获取支持的分类
    /// </summary>
    public interface ISupportedCategoriesService
    {
        List<SupportedCategory> GetSupportedCategories();
    }



}
