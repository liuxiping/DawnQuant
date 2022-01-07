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
    public class EFFutureEventOfSubjectRepositoy : EFCoreRepositoryBase<FutureEventOfSubject, long>
        , IFutureEventOfSubjectRepository
    {
        public EFFutureEventOfSubjectRepositoy(StockEDDbContext stockDbContext) : base(stockDbContext)
        {
           
        }


        public override FutureEventOfSubject Save(FutureEventOfSubject entity)
        {
            //查询数据是否存在
            var subject = _entityContext.Entities.Where(p => p.Date == entity.Date &&
            p.Event == entity.Event).AsNoTracking().SingleOrDefault();

            //保存数据
            if (subject == null)
            {
                //不存在则保存
                _entityContext.Entities.Add(entity);

            }
            else
            {
                entity.Id = subject.Id;
                var s = _entityContext.Entities.Attach(entity);
                s.State = EntityState.Modified;
            }
            _entityContext.SaveChanges();
            return entity;
        }

        public override IEnumerable<FutureEventOfSubject> Save(IEnumerable<FutureEventOfSubject> entities)
        {

           foreach(var entity in entities)
            {
                //查询数据是否存在
                var subject = _entityContext.Entities.Where(p => p.Date == entity.Date &&
                p.Event == entity.Event).AsNoTracking().SingleOrDefault();

                //保存数据
                if (subject == null)
                {
                    //不存在则保存
                    _entityContext.Entities.Add(entity);

                }
                else
                {
                    entity.Id = subject.Id;
                    var s = _entityContext.Entities.Attach(entity);
                    s.State = EntityState.Modified;
                }
              
            }

            _entityContext.SaveChanges();

            return entities;
        }
    }
}
