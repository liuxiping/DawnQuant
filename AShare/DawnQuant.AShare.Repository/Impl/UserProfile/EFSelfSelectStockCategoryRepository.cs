using DawnQuant.Repository;
using DawnQuant.AShare.Entities.UserProfile;
using DawnQuant.AShare.Repository.Abstract.UserProfile;
using DawnQuant.AShare.Repository.Impl.UserProfile;
using Microsoft.EntityFrameworkCore;
using System;

namespace DawnQuant.AShare.Repository.Impl.UserProfile
{
    public class EFSelfSelectStockCategoryRepository : EFCoreRepositoryBase<SelfSelectStockCategory, long>,
        ISelfSelectStockCategoryRepository
    {
        public EFSelfSelectStockCategoryRepository(UserProfileDbContext dbContext ):base(dbContext)
        {

        }
    }
}
