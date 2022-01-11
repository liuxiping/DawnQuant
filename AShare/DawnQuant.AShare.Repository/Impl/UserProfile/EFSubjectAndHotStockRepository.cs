using DawnQuant.AShare.Entities.UserProfile;
using DawnQuant.AShare.Repository.Abstract.UserProfile;
using DawnQuant.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.AShare.Repository.Impl.UserProfile
{
    public class EFSubjectAndHotStockRepository : EFCoreRepositoryBase<SubjectAndHotStock, long>, ISubjectAndHotStockRepository
    {
        public EFSubjectAndHotStockRepository(UserProfileDbContext dbContext) : base(dbContext)
        {


        }

        public override IEnumerable<SubjectAndHotStock> Save(IEnumerable<SubjectAndHotStock> entities)
        {
            foreach (var entity in entities)
            {
                //查询数据是否存在
                var subjectAndHot = _entityContext.Entities.Where(p => p.TSCode == entity.TSCode &&
                 p.UserId == entity.UserId && p.CategoryId == entity.CategoryId).AsNoTracking().SingleOrDefault();
                if (subjectAndHot != null)
                {
                    //更新Id
                    entity.Id = subjectAndHot.Id;
                    var s = _entityContext.Entities.Attach(entity);
                    s.State = EntityState.Modified;
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

        public override SubjectAndHotStock Save(SubjectAndHotStock entity)
        {
            //查询数据是否存在
            var subjectAndHot = _entityContext.Entities.Where(p => p.TSCode == entity.TSCode &&
             p.UserId == entity.UserId && p.CategoryId == entity.CategoryId).AsNoTracking().SingleOrDefault();
            if (subjectAndHot == null)
            {
                //更新Id
                entity.Id = subjectAndHot.Id;
                var s = _entityContext.Entities.Attach(entity);
                s.State = EntityState.Modified;
            }
            else
            {
                //不存在则保存
                _entityContext.Entities.Add(entity);

            }

            _entityContext.SaveChanges();
            return entity;
        }

    }
}
