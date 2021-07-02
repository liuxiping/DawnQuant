using System;

using DawnQuant.AShare.Repository.Impl.UserProfile;
using DawnQuant.Repository;
using DawnQuant.AShare.Entities.UserProfile;
using DawnQuant.AShare.Repository.Abstract.UserProfile;


namespace DawnQuant.AShare.Repository.Impl.UserProfile
{
    public class EFSelfSelectStockRepository : EFCoreRepositoryBase<SelfSelectStock,long>, ISelfSelectStockRepository
    {
        public EFSelfSelectStockRepository(UserProfileDbContext dbContext):base(dbContext)
        {

        }
    }
}
