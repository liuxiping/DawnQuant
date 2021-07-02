using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.Repository
{
    public class EFCoreRepositoryBase<TEntity, Tkey> : IRepository<TEntity, Tkey>
        where TEntity : BaseEntity<Tkey>
    {
       protected EntityDbContext<TEntity, Tkey> _dbContext;

        public EFCoreRepositoryBase(DbContext dbContext)
        {
            _dbContext = new EntityDbContext<TEntity, Tkey>(dbContext);
        }

        public  IQueryable<TEntity> Entities => _dbContext.Entities;

        public virtual  TEntity Delete(Tkey id)
        {
            TEntity entity = _dbContext.Entities.Find(id);
            if (entity != null)
            {
                _dbContext.Entities.Remove(entity);
            }
            _dbContext.SaveChanges();
            return entity;
        }

        public virtual TEntity Delete(TEntity entity)
        {

            entity = _dbContext.Entities.Find(entity.GetKeyValue());
            if (entity != null)
            {
                _dbContext.Entities.Remove(entity);
            }
            _dbContext.SaveChanges();
            return entity;
        }

        public virtual IEnumerable<TEntity> Delete(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                return null;
            }

            List<TEntity> delEntities = new List<TEntity>();
            foreach (TEntity entity in entities)
            {
                var delEntity = _dbContext.Entities.Find(entity.GetKeyValue());
                if (entity != null)
                {
                    delEntities.Add(delEntity);
                    _dbContext.Entities.Remove(entity);
                }


            }
            _dbContext.SaveChanges();
            return delEntities;
        }

        public virtual TEntity Insert(TEntity entity)
        {
            _dbContext.Entities.Add(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        public virtual IEnumerable<TEntity> Insert(IEnumerable<TEntity> entities)
        {
            foreach (TEntity entity in entities)
            {
                _dbContext.Entities.Add(entity);
            }
            _dbContext.SaveChanges();
            return entities;
        }

        public virtual TEntity Save(TEntity entity)
        {
            //保存数据
            if (_dbContext.Entities.Contains(entity))
            {

                var s = _dbContext.Entities.Attach(entity);
                s.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            else
            {
                _dbContext.Entities.Add(entity);
            }

            _dbContext.SaveChanges();

            return entity;
        }

        public virtual IEnumerable<TEntity> Save(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                //保存数据
                if (_dbContext.Entities.Contains(entity))
                {

                    var s = _dbContext.Entities.Attach(entity);
                    s.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                else
                {
                    _dbContext.Entities.Add(entity);
                }
            }
            _dbContext.SaveChanges();
            return entities;
        }

        public virtual TEntity Update(TEntity entity)
        {
            var s = _dbContext.Entities.Attach(entity);
            s.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _dbContext.SaveChanges();
            return entity;
        }

        public virtual IEnumerable<TEntity> Update(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                var s = _dbContext.Entities.Attach(entity);
                s.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
            _dbContext.SaveChanges();
            return entities;
        }


        #region dispose
        ~EFCoreRepositoryBase()
        {
            Dispose(false);
        }

        bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return; //如果已经被回收，就中断执行

            if (disposing)
            {
                //TODO:释放本对象中管理的托管资源
                _dbContext.Dispose();
            }
            //TODO:释放非托管资源

            _disposed = true;
        }

        #endregion
    }
}
