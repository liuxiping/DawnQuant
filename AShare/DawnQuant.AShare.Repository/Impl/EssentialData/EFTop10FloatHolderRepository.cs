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
    public class EFTop10FloatHolderRepository : EFCoreRepositoryBase<Top10FloatHolder, long>,
        ITop10FloatHolderRepository
    {
        public EFTop10FloatHolderRepository(StockEDDbContext dbContext) : base(dbContext)
        {

        }

        public override IEnumerable<Top10FloatHolder> Save(IEnumerable<Top10FloatHolder> entities)
        {
            foreach (var entity in entities)
            {
                //查询数据是否存在
                var holderNumber = _dbContext.Entities.Where(p => p.TSCode == entity.TSCode &&
                  p.EndDate == entity.EndDate && p.HolderName == entity.HolderName).SingleOrDefault();

                if (holderNumber != null)
                {
                    //更新Id
                    entity.Id = holderNumber.Id;
                   }
            }

                return base.Save(entities);
        }

        public override Top10FloatHolder Save(Top10FloatHolder entity)
        {
            //查询数据是否存在
            var holderNumber = _dbContext.Entities.Where(p => p.TSCode == entity.TSCode &&
              p.EndDate == entity.EndDate && p.HolderName== entity.HolderName).SingleOrDefault();

            if (holderNumber != null)
            {
                //更新Id
                entity.Id = holderNumber.Id;
            }

            return base.Save(entity);

            
        }
    }
}
