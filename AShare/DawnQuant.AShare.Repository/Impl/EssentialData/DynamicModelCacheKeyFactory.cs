
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Repository.Impl.EssentialData
{
    public class DynamicModelCacheKeyFactory : IModelCacheKeyFactory
    {
        public object Create(DbContext context)
        {
            object obj = (object)context.GetType();
            if (context is StockTradeDataDbContext stockDataContext)
            {
                obj = stockDataContext.TableName;
            }
            else if (context is StockDailyIndicatorDbContext stockIndicatorDataContext)
            {
                obj = stockIndicatorDataContext.TableName;
            }

            else if (context is THSIndexTradeDataDbContext thsIndexTradeDataDbContext)
            {
                obj = thsIndexTradeDataDbContext.TableName;
            }

            return obj;

        }


    }
}
