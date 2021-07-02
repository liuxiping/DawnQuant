using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.Repository
{
    public class EntityDbContext<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
       
        public EntityDbContext(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        DbContext _dbContext;
        public DbContext Context
        {
            get { return _dbContext; }
        }

        /// <summary>
        /// 根据传递的实体类型，返回相应的DbSet
        /// </summary>
        public DbSet<TEntity> Entities
        {
            get
            {
                var type = typeof(DbSet<TEntity>);
           
                var prop = _dbContext.GetType().GetProperties()
                    .Where(p => p.PropertyType== type).FirstOrDefault();
                return (DbSet<TEntity>)prop.GetValue(_dbContext);
            }
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        #region Dispose

        ~EntityDbContext()
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
