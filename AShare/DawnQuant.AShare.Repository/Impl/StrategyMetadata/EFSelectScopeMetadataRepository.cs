using DawnQuant.AShare.Entities.StrategyMetadata;
using DawnQuant.AShare.Repository.Abstract.StrategyMetadata;
using DawnQuant.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Repository.Impl.StrategyMetadata
{
    public class EFSelectScopeMetadataRepository : EFCoreRepositoryBase<SelectScopeMetadata, long>,
        ISelectScopeMetadataRepository
    {

        public EFSelectScopeMetadataRepository(StrategyDbContext dbContext) : base(dbContext)
        {

        }
    }
}
