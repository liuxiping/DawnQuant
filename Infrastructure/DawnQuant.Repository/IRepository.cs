using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.Repository
{
    /// <summary>
    /// 仓储接口，定义公共的泛型CRUD
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity, Tkey>:IDisposable where TEntity  : BaseEntity<Tkey>
    {
      
        IQueryable<TEntity> Entities { get; }
       
        TEntity Insert(TEntity entity);
        IEnumerable<TEntity> Insert(IEnumerable<TEntity> entities);

        TEntity Delete(Tkey id);
        TEntity Delete(TEntity entity);
        IEnumerable<TEntity> Delete(IEnumerable<TEntity> entities);

        TEntity Update(TEntity entity);
        IEnumerable<TEntity> Update(IEnumerable<TEntity> entities);

        /// <summary>
        /// Save方法是存在则更新 不存在这插入
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity Save(TEntity entity);
        IEnumerable<TEntity> Save(IEnumerable<TEntity> entities);

        
       

    }
}
