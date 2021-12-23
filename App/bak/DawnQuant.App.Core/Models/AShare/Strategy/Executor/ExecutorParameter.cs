using DawnQuant.App.Core.Utils;
using System.Text.Json;

namespace DawnQuant.App.Core.Models.AShare.Strategy.Executor
{
    public abstract class ExecutorParameter
    {

        public ExecutorParameter( )
        {
            
        }
        public virtual void Initialize(string json,IServiceProvider serviceProvider=null)
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
