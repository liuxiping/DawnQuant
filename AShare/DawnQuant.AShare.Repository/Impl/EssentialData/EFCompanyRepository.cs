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
    public class EFCompanyRepository : EFCoreRepositoryBase<Company, string>,
        ICompanyRepository
    {
        public EFCompanyRepository(StockEDDbContext dbContext) : base(dbContext)
        {

        }
    }
}
