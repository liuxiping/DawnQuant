
using DawnQuant.AShare.Repository.Impl.UserProfile;
using DawnQuant.Repository;
using DawnQuant.AShare.Entities.UserProfile;
using DawnQuant.AShare.Repository.Abstract.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DawnQuant.AShare.Entities;

namespace DawnQuant.AShare.Repository.Impl.UserProfile
{
    public class EFStrategyScheduledTaskRepository : EFCoreRepositoryBase<StrategyScheduledTask, long>,
         IStrategyScheduledTaskRepository
    {
        public EFStrategyScheduledTaskRepository(UserProfileDbContext dbContext) : base(dbContext)
        {

        }
    }
}
