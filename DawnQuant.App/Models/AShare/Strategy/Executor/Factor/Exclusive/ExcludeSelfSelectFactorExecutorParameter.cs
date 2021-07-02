using Autofac;
using DawnQuant.App.Models.AShare.Strategy.Executor.Common;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DawnQuant.App.Models.AShare.Strategy.Executor.Factor
{

    /// <summary>
    /// 排除自选分类
    /// </summary>
    public class ExcludeSelfSelectFactorExecutorParameter : ExecutorParameter
    {
      
        public ExcludeSelfSelectFactorExecutorParameter()
        {
        }

        [DisplayName("自选股分类")]
        public SupportedCategories Categories { get; set; }


        public override void Initialize(string json)
        {
            //从服务器获取自选股分类
            SupportedCategoriesService service = IOCUtil.Container.Resolve<SupportedCategoriesService>();
            var supportedCategories = service.GetSupportedCategories();

            //初始化分类
            Categories = new SupportedCategories();
            foreach (var category in supportedCategories)
            {
                Categories.Add(category, false);
            }

            //获取选择自选分类
            InternalParameter internalParameter = null;
            if (!string.IsNullOrEmpty(json))
            {
                internalParameter = JsonSerializer.Deserialize<InternalParameter>(json);
            }

            if (Categories != null && Categories.Count > 0)
            {
                foreach (var kv in Categories)
                {
                    if (internalParameter != null)
                    {
                        if (internalParameter.ExcludeCategories.Contains(kv.Key.CategoryId))
                        {
                            Categories[kv.Key] = true;
                        }
                    }
                }
            }

        }


        public override string Serialize()
        {
            List<long> excludeCategories = new List<long>();

            if (Categories != null)
            {
                foreach (var kv in Categories)
                {
                    if (kv.Value)
                    {
                        excludeCategories.Add(kv.Key.CategoryId);
                    }
                }

            }

            if (excludeCategories.Count > 0)
            {
                return JsonSerializer.Serialize(
                    new InternalParameter { ExcludeCategories = excludeCategories });
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 内部存储参数数据
        /// </summary>
        public class InternalParameter
        {
            public List<long> ExcludeCategories { get; set; }
        }

        public class SupportedCategories : Dictionary<SupportedCategory, bool>
        {
            public override string ToString()
            {
                return "是否加入排除范围";
            }
        }
    }




}
