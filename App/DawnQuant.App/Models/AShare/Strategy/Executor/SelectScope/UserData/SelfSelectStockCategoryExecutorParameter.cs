
using Autofac;
using DawnQuant.App.Models.AShare.Strategy.Executor.Common;
using DawnQuant.App.Services.AShare;
using DawnQuant.App.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DawnQuant.App.Models.AShare.Strategy.Executor.SelectScope
{
    /// <summary>
    /// 自选分类
    /// </summary>

    public class SelfSelectStockCategoryExecutorParameter : ExecutorParameter
    {
        public SelfSelectStockCategoryExecutorParameter()
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
                        if (internalParameter.SupportedCategories.Contains(kv.Key.CategoryId))
                        {
                            Categories[kv.Key] = true;
                        }
                    }
                }
            }

        }


        public override string Serialize()
        {
            List<long> supportedCategories = new List<long>();

            if (Categories != null)
            {
                foreach (var kv in Categories)
                {
                    if (kv.Value)
                    {
                        supportedCategories.Add(kv.Key.CategoryId);
                    }
                }

            }

            if (supportedCategories.Count > 0)
            {
                return JsonSerializer.Serialize(
                    new InternalParameter { SupportedCategories = supportedCategories });
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
            public List<long> SupportedCategories { get; set; }
        }

        public class SupportedCategories : Dictionary<SupportedCategory, bool>
        {
            public override string ToString()
            {
                return "是否加入选股范围";
            }
        }
    }






}
