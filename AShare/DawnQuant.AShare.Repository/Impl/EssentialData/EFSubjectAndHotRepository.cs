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
    public class EFSubjectAndHotRepository : EFCoreRepositoryBase<SubjectAndHot, long>
        , ISubjectAndHotRepository
    {
        public EFSubjectAndHotRepository(StockEDDbContext stockDbContext) : base(stockDbContext)
        {

        }

        public override IEnumerable<SubjectAndHot> Save(IEnumerable<SubjectAndHot> entities)
        {
            foreach (var entity in entities)
            {
                //查询数据是否存在
                var subjectAndHot = _entityContext.Entities.Where(p => p.TSCode == entity.TSCode &&
                 p.Source == entity.Source).AsNoTracking().SingleOrDefault();
                if (subjectAndHot != null)
                {
                    //更新Id
                    entity.Id = subjectAndHot.Id;
                    var s = _entityContext.Entities.Attach(entity);
                    s.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                else
                {
                    //不存在则保存
                    _entityContext.Entities.Add(entity);
                   
                }
            }
            _entityContext.SaveChanges();
            return entities;
        }

        public override SubjectAndHot Save(SubjectAndHot entity)
        {
            var subjectAndHot = _entityContext.Entities.Where(p => p.TSCode == entity.TSCode &&
                p.Source == entity.Source).AsNoTracking().SingleOrDefault();
            if (subjectAndHot != null)
            {
                //更新Id
                entity.Id = subjectAndHot.Id;
                var s = _entityContext.Entities.Attach(entity);
                s.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            else
            {
                _entityContext.Entities.Add(entity);
            }
            _entityContext.SaveChanges();
            return entity;
            
        }
    }
}
