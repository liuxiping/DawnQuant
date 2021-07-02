using System;
using System.Linq.Expressions;

namespace DawnQuant.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class Extensions
    {

        /// <summary>
        /// 获取类的属性名称
        /// </summary>
        /// <typeparam name="TClassType"></typeparam>
        /// <typeparam name="TPropertyType"></typeparam>
        /// <param name="obj"></param>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static string GetPropertyName<TClassType, TPropertyType>(this object obj , Expression<Func<TClassType, TPropertyType>> expr)
        {
            var name = ((MemberExpression)expr.Body).Member.Name;
            return name;
        }


        /// <summary>
        /// 根据当前日期获取周的开始与结束
        /// </summary>
        /// <param name="date"></param>
        /// <param name="dtBeginDate"></param>
        /// <param name="dtEndDate"></param>
        public static void GetWeekSpan(this DateTime date, out DateTime dtBeginDate, out DateTime dtEndDate)
        {
            int week = (int)date.DayOfWeek;
            if (week == 0) week = 7; //周日
            dtBeginDate = DateTime.Today.AddDays(-(week - 1));
            dtEndDate = dtBeginDate.AddDays(6);
        }


        /// <summary>
        /// 根据当前日期或月的你开始与结束
        /// </summary>
        /// <param name="date"></param>
        /// <param name="dtBeginDate"></param>
        /// <param name="dtEndDate"></param>
        public static void GetMonthSpan(this DateTime date, out DateTime dtBeginDate, out DateTime dtEndDate)
        {
            dtBeginDate=new(date.Year,date.Month,1);
            dtEndDate = dtBeginDate.AddMonths(1).AddDays(-1); 
        }

    }
}
