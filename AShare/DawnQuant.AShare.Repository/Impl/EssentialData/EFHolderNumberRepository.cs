using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using DawnQuant.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Repository.Impl.EssentialData
{
    public class EFHolderNumberRepository : EFCoreRepositoryBase<HolderNumber, long>,
        IHolderNumberRepository
    {
        public EFHolderNumberRepository(StockEDDbContext dbContext) : base(dbContext)
        {

        }

        public override HolderNumber Save(HolderNumber entity)
        {

            //查询数据是否存在
            var holderNumber = _dbContext.Entities.Where(p => p.TSCode == entity.TSCode &&
              p.ReportingPeriod == entity.ReportingPeriod).SingleOrDefault();

            if (holderNumber != null)
            {
                //更新Id
                entity.Id = holderNumber.Id;
            }

            return base.Save(entity);
        }

        public override IEnumerable<HolderNumber> Save(IEnumerable<HolderNumber> entities)
        {
            foreach (var entity in entities)
            {
                //查询数据是否存在
                var holderNumber = _dbContext.Entities.Where(p => p.TSCode == entity.TSCode &&
                  p.ReportingPeriod == entity.ReportingPeriod ).SingleOrDefault();

                if (holderNumber != null)
                {
                    //更新Id
                    entity.Id = holderNumber.Id;
                }
            }
            return base.Save(entities);
        }
    }
}
