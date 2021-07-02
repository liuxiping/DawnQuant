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
   public class EFExclusionStockRepository : EFCoreRepositoryBase<ExclusionStock, long>,
        IExclusionStockRepository
    {
        public EFExclusionStockRepository(UserProfileDbContext dbContext) : base(dbContext)
        {

        }
    
    }
}
