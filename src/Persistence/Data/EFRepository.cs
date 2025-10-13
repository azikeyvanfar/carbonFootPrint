using System;
using System.Collections.Generic;
using System.Linq;
using ContractorBackend.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ContractorBackend.Persistence.Data
{
    public class EFRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly IApplicationDbContext _context;
        private DbSet<TEntity> _entities;
        private IQueryable<TEntity> _source;
        public EFRepository(IApplicationDbContext context)
        {
            _context = context;
            _entities = _context.Set<TEntity>();
            _source = _context.Set<TEntity>();
        }

        public void Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _entities.Remove(entity);

            _context.SaveChanges();
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            if (!entities.Any())
                throw new ArgumentNullException("TEntity count is zero");

            _entities.RemoveRange(entities);
            _context.SaveChanges();
        }

        public IQueryable<TEntity> GetAll()
        {
            return _entities.AsQueryable();
        }

        public IQueryable<TEntity> GetAllAsNoTracking()
        {
            return _entities.AsNoTracking();
        }

        public TEntity GetById(Guid id)
        {
            return _entities.Find(id);
        }

        public TEntity GetById(long id)
        {
            return _entities.Find(id);
        }

        public TEntity GetByIds(Guid[] ids)
        {
            return _entities.Find(ids);
        }

        public TEntity GetByIds(long[] ids)
        {
            return _entities.Find(ids);
        }
        public IQueryable<TEntity> GetList()
        {
            return _source;
        }
        public void Insert(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                _entities.Add(entity);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.ToString());
                throw;
            }

        }

        public int InsertRange(ICollection<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            try
            {
                _entities.AddRange(entities);
                var res = _context.SaveChanges();
                return res;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.ToString());
                throw;
            }
        }

        public bool InsertEntity(TEntity entity)
        {
            var res = false;
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            try
            {
                _entities.Add(entity);
                res = _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.ToString());
                throw;
            }
            return res;
        }

        public void Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            try
            {
                _entities.Update(entity);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.ToString());
                throw;
            }
        }
        public int UpdateRange(ICollection<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));
            try
            {
                _entities.UpdateRange(entities);
                var res = _context.SaveChanges();
                return res;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.ToString());
                throw;
            }
        }

        public bool UpdateEntity(TEntity entity)
        {
            var res = false;
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            try
            {
                _entities.Update(entity);
                res = _context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.ToString());
                throw;
            }
            return res;
        }
    }
}
