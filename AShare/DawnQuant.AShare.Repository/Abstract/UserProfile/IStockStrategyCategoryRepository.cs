
using DawnQuant.AShare.Entities.UserProfile;
using DawnQuant.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Repository.Abstract.UserProfile
{
    public interface IStockStrategyCategoryRepository : IRepository<StockStrategyCategory, long>
    {
    }
}
