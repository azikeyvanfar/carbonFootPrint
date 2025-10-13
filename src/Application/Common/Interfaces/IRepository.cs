using System;
using System.Collections.Generic;
using System.Linq;

namespace ContractorBackend.Application.Common.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity GetById(Guid id);
        TEntity GetByIds(Guid[] ids);
        TEntity GetById(long id);
        TEntity GetByIds(long[] ids);
        void Insert(TEntity entity);
        int InsertRange(ICollection<TEntity> entities);
        bool InsertEntity(TEntity entity);
        bool UpdateEntity(TEntity entity);
        void Update(TEntity entity);
        int UpdateRange(ICollection<TEntity> entities);
        void Delete(TEntity entity);
        void DeleteRange(IEnumerable<TEntity> entities);
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetAllAsNoTracking();
        IQueryable<TEntity> GetList();
    }
}
