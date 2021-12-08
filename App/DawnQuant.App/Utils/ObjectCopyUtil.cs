using DawnQuant.App.Models.AShare.Strategy.Executor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.Utils
{
  public  class ObjectCopyUtil
    {
        public static PropertyInfo[] GetPropertyInfos(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }
        /// <summary>
        /// 实体属性反射
        /// </summary>
        /// <typeparam name="S">赋值对象</typeparam>
        /// <typeparam name="T">被赋值对象</typeparam>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public static void CopyTo<S, T>(S s, T t)
        {
            PropertyInfo[] pps = GetPropertyInfos(s.GetType());
            Type target = t.GetType();

            foreach (var pp in pps)
            {
                PropertyInfo targetPP = target.GetProperty(pp.Name);
                object value = pp.GetValue(s, null);

                if (targetPP != null && value != null)
                {
                    targetPP.SetValue(t, value, null);
                }
            }
        }

        public static void CopyTo<T>(T s, T t)
        {
            PropertyInfo[] pps = GetPropertyInfos(s.GetType());
            Type target = t.GetType();

            foreach (var pp in pps)
            {
                PropertyInfo targetPP = target.GetProperty(pp.Name);
                object value = pp.GetValue(s, null);
                if (targetPP != null && value != null)
                {
                    targetPP.SetValue(t, value, null);
                }
            }
        }

        /// <summary>
        /// 相同对象复制
        /// </summary>
        /// <param name="type"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void CopyTo(Type type, ExecutorParameter source, ExecutorParameter target )
        {
            PropertyInfo[] pps = GetPropertyInfos(type);
          
            foreach (var pp in pps)
            {
                object value = pp.GetValue(source, null);
                if (value != null)
                {
                    pp.SetValue(target, value, null);
                }
            }
        }
    }
}
