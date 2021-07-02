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
    public class EFTradingCalendarRepository : EFCoreRepositoryBase<TradingCalendar, int>
        ,ITradingCalendarRepository
    {

        public EFTradingCalendarRepository(StockEDDbContext dbContext) : base(dbContext)
        {

        }

        public override IEnumerable<TradingCalendar> Save(IEnumerable<TradingCalendar> entities)
        {
            foreach (var entity in entities)
            {
                //保存数据

                var tradingCalendar = _dbContext.Entities.Where(p => p.Exchange == entity.Exchange &&
                  p.Date == entity.Date).SingleOrDefault();

                if (tradingCalendar!=null)
                {
                    //更新数据
                    tradingCalendar.IsOpen = entity.IsOpen;
                    tradingCalendar.PreDate = entity.PreDate;
                }
                else
                {
                    _dbContext.Entities.Add(entity);
                }
            }
            _dbContext.SaveChanges();
            return entities;
        }


        public override TradingCalendar Save(TradingCalendar entity)
        {
            //保存数据
            var tradingCalendar = _dbContext.Entities.Where(p => p.Exchange == entity.Exchange &&
                 p.Date == entity.Date).SingleOrDefault();

            if (tradingCalendar != null)
            {
                //更新数据
                tradingCalendar.IsOpen = entity.IsOpen;
                tradingCalendar.PreDate = entity.PreDate;
            }
            else
            {
                _dbContext.Entities.Add(entity);
            }

            _dbContext.SaveChanges();
            return entity;
        }

    }
}
