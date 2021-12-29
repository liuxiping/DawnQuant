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
    public class EFBellwetherRepository : EFCoreRepositoryBase<Bellwether, long>
        , IBellwetherRepository
    {
        public EFBellwetherRepository(StockEDDbContext stockDbContext) : base(stockDbContext)
        {

            
        }

        public override Bellwether Save(Bellwether entity)
        {

            //查询数据是否存在
            var bellwether = _entityContext.Entities.Where(p => p.TSCode == entity.TSCode &&
            p.Source == entity.Source).AsNoTracking().SingleOrDefault();

            //保存数据
            if (bellwether == null)
            {
                //不存在则保存
                _entityContext.Entities.Add(entity);
               
            }
            else
            {
                entity.Id = bellwether.Id;
                var s = _entityContext.Entities.Attach(entity);
                s.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            _entityContext.SaveChanges();
            return entity;

        }



        public override IEnumerable<Bellwether> Save(IEnumerable<Bellwether> entities)
        {
    
            foreach (var entity in entities)
            {
                //查询数据是否存在
               var bellwether = _entityContext.Entities.Where(p => p.TSCode == entity.TSCode &&
               p.Source == entity.Source).AsNoTracking().SingleOrDefault();

                //保存数据
                if (bellwether==null)
                {
                    _entityContext.Entities.Add(entity);
                }
                else
                {
                    entity.Id = bellwether.Id;
                    var s = _entityContext.Entities.Attach(entity);
                    s.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
            }
            _entityContext.SaveChanges();
            return entities;
        }

      
        
    }
}
