using DawnQuant.AShare.Entities.UserProfile;
using DawnQuant.AShare.Repository.Abstract.UserProfile;
using DawnQuant.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Repository.Impl.UserProfile
{
    public class EFBellwetherStockCategoryRepository : EFCoreRepositoryBase<BellwetherStockCategory, long>, IBellwetherStockCategoryRepository
    {
        public EFBellwetherStockCategoryRepository(UserProfileDbContext dbContext) : base(dbContext)
        {

        }
    }
}
