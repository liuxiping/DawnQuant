using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.Repository
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract class BaseEntity<TKey>
    {
        /// <summary>
        /// 获取主键的值
        /// </summary>
        /// <returns></returns>
        public virtual TKey GetKeyValue()
        {
            return default(TKey);
        }
    }
}
