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
    public class EFIndustryRepository : EFCoreRepositoryBase<Industry, int>,
        IIndustryRepository
    {

        public EFIndustryRepository(StockEDDbContext dbContext) : base(dbContext)
        {

        }



        public Industry ParseIndustry((string first, string second, string three) industry)
        {
            //第一级
            Industry fIndustry = _entityContext.Entities.SingleOrDefault(p => p.Name == industry.first && p.Level == 1);

            if (fIndustry == null)
            {
                fIndustry = new Industry { Name = industry.first, Level = 1, ParentId = 0 };
                fIndustry = Insert(fIndustry);
            }

            //第二级别
            Industry sIndustry = _entityContext.Entities.SingleOrDefault(p => p.Name == industry.second && p.Level == 2);
            if (sIndustry == null)
            {
                sIndustry = new Industry { Name = industry.second, Level = 2, ParentId = fIndustry.Id };
                sIndustry = Insert(sIndustry);

            }

            //第三级别
            Industry tIndustry = _entityContext.Entities.SingleOrDefault(p => p.Name == industry.three && p.Level == 3);
            if (tIndustry == null)
            {
                tIndustry = new Industry { Name = industry.three, Level = 3, ParentId = sIndustry.Id };
                tIndustry = Insert(tIndustry);

            }
            return tIndustry;
        }

        public Industry GetParentIndustry(int id)
        {
            Industry industry = _entityContext.Entities.SingleOrDefault(p => p.Id == id);
            return industry == null ? null :
                 _entityContext.Entities.SingleOrDefault(p => p.Id == industry.ParentId);
        }

        public IQueryable<Industry> GetSubIndustry(int id)
        {
            return _entityContext.Entities.Where(p => p.ParentId == id);
        }

    }
}
