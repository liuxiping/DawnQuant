using DawnQuant.AShare.Entities.EssentialData;
using DawnQuant.AShare.Repository.Abstract.EssentialData;
using DawnQuant.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Repository.Impl.EssentialData
{
    public class EFTHSIndexMemberRepository : EFCoreRepositoryBase<THSIndexMember, long>
        , ITHSIndexMemberRepository
    {
        public EFTHSIndexMemberRepository(StockEDDbContext stockDbContext) : base(stockDbContext)
        {

            
        }
    }
}
