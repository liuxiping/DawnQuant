
using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace DawnQuant.AShare.Repository.Abstract.EssentialData
{
    public interface ITHSIndexTradeDataRepository : IRepository<THSIndexTradeData, DateTime>
    {
        /// <summary>
        /// 清空数据
        /// </summary>
        void Empty();
    }
}
