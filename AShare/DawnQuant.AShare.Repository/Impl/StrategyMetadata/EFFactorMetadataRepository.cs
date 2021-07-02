
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
    public class EFFactorMetadataRepository : EFCoreRepositoryBase<FactorMetadata, long>,
        IFactorMetadataRepository
    {

        public EFFactorMetadataRepository(StrategyDbContext dbContext) : base(dbContext)
        {

        }
    }
}
