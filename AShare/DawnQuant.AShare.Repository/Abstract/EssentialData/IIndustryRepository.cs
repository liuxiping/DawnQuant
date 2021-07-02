using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.Repository;
using System;
using System.Linq;

namespace DawnQuant.AShare.Repository.Abstract.EssentialData
{

    /// <summary>
    /// 申万行业分类
    /// </summary>
    public interface IIndustryRepository : IRepository<Industry, int>
    {

        /// <summary>
        /// 获取父行业
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Industry GetParentIndustry(int id);


        /// <summary>
        /// 获取所有子行业
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IQueryable<Industry> GetSubIndustry(int id);


        /// <summary>
        /// 解析行业，如果数据库没有则插入
        /// </summary>
        /// <param name="industry">有色金属 -- 稀有金属 -- 其他稀有小金属 （共</param>
        /// <returns>返回第三级别行业</returns>
        Industry ParseIndustry((string first, string second, string three) industry);   
    }
}
