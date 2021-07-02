using DawnQuant.App.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DawnQuant.App.Models.AShare.Strategy.Executor
{
    public abstract class ExecutorParameter
    {

        public ExecutorParameter( )
        {
            
        }
        public virtual void Initialize(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                ExecutorParameter p = (ExecutorParameter)JsonSerializer.Deserialize(json, this.GetType());

                //复制属性
                ObjectCopyUtil.CopyTo(this.GetType(), p, this);
            }

        }
        public abstract string Serialize();
       
    }
}
