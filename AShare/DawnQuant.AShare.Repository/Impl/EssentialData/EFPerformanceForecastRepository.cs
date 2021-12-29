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
    public class EFPerformanceForecastRepository : EFCoreRepositoryBase<PerformanceForecast, long>
        , IPerformanceForecastRepository
    {
        public EFPerformanceForecastRepository(StockEDDbContext stockDbContext) : base(stockDbContext)
        {

        }

        public override IEnumerable<PerformanceForecast> Save(IEnumerable<PerformanceForecast> entities)
        {
            foreach (var entity in entities)
            {
                //查询数据是否存在
                var performanceForecast = _entityContext.Entities
                 .Where(p => p.TSCode == entity.TSCode &&
                 p.Source == entity.Source && p.EndDate==entity.EndDate).SingleOrDefault();
                if (performanceForecast != null)
                {
                    //更新Id
                    entity.Id = performanceForecast.Id;
                }
            }

            return base.Save(entities);
        }

        public override PerformanceForecast Save(PerformanceForecast entity)
        {
            //查询数据是否存在
            var performanceForecast = _entityContext.Entities
             .Where(p => p.TSCode == entity.TSCode &&
             p.Source == entity.Source && p.EndDate == entity.EndDate).SingleOrDefault();
            if (performanceForecast != null)
            {
                //更新Id
                entity.Id = performanceForecast.Id;
            }

            return base.Save(entity);
        }

        public void Empty()
        {
            string sql = "delete from performanceforecasts";
            _entityContext.Context.Database.ExecuteSqlRaw(sql);

        }
    }
}
