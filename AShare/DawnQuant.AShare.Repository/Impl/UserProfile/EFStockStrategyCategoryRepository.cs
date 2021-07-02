using DawnQuant.Repository;
using DawnQuant.AShare.Entities.UserProfile;
using DawnQuant.AShare.Repository.Abstract.UserProfile;
using DawnQuant.AShare.Repository.Impl.UserProfile;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Repository.Impl.UserProfile
{
    public class EFStockStrategyCategoryRepository : EFCoreRepositoryBase<StockStrategyCategory, long>,
           IStockStrategyCategoryRepository
    {
        public EFStockStrategyCategoryRepository(UserProfileDbContext dbContext) : base(dbContext)
        {

        }
    }
}

