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
    public class EFSelectScopeMetadataCategoryRepository : EFCoreRepositoryBase<SelectScopeMetadataCategory, long>,
          ISelectScopeMetadataCategoryRepository
    {
        public EFSelectScopeMetadataCategoryRepository(StrategyDbContext dbContext) : base(dbContext)
        {

        }

    }
}
