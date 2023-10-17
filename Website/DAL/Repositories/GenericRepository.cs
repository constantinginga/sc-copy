using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace StartupCentral.DAL.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, new()
    {
        protected readonly DbContext Context;
        protected readonly DbSet<TEntity> DbSet;

        public GenericRepository(DbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        public IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = DbSet;

            query = Filter(query, filter);
            query = Include(query, includeProperties);

            try
            {
                return query.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public IQueryable<TEntity> WhereLazy(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = DbSet;

            query = Filter(query, filter);
            query = Include(query, includeProperties);
            return query;
        }

        public async Task<IEnumerable<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = DbSet;

            query = Filter(query, filter);
            query = Include(query, includeProperties);
            return await query.ToListAsync();
        }

        public IQueryable<TEntity> AsQueryable(string includeProperties = "")
        {
            IQueryable<TEntity> query = DbSet;

            query = Include(query, includeProperties);
            return query;
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = DbSet;

            query = Filter(query, filter);
            query = Include(query, includeProperties);

            return query.FirstOrDefault();
        }
        public TEntity GetFirst(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
        }

        public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = DbSet;

            query = Filter(query, filter);
            query = Include(query, includeProperties);

            return query.FirstOrDefaultAsync();
        }

        public IQueryable<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> path)
        {
            return DbSet.Include(path);
        }


        public IEnumerable<TEntity> GetAll()
        {
            return DbSet;
        }

        public IQueryable<TEntity> All()
        {
            return DbSet;
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }

        public TEntity GetById(params object[] id)
        {
            if (id == null) return null;

            return DbSet.Find(id);
        }

        public async Task<TEntity> GetByIdAsync(params object[] id)
        {
            if (id == null) return null;

            return await DbSet.FindAsync(id);
        }

        public void Insert(TEntity entity)
        {
            if (entity == null) return;

            DbSet.Add(entity);
        }

        public void InsertRange(List<TEntity> entities)
        {
            if (!entities.Any()) return;

            DbSet.AddRange(entities);
        }

        public void Delete(object id)
        {
            if (id == null) return;

            var entityToDelete = DbSet.Find(id);
            Delete(entityToDelete);
        }

        public void Delete(TEntity entityToDelete)
        {
            if (entityToDelete == null) return;

            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }
            DbSet.Remove(entityToDelete);
        }

        public void Update(TEntity entityToUpdate)
        {
            if (entityToUpdate == null) return;

            DbSet.Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        /// <summary></summary>
        private IQueryable<TEntity> Filter(IQueryable<TEntity> data, Expression<Func<TEntity, bool>> filter)
        {
            try
            {
                return filter == null ? data : data.Where(filter);
            }
            catch (Exception e)
            {

                throw;
            }

        }


        /// <summary></summary>
        private IQueryable<TEntity> Include(IQueryable<TEntity> data, string includeList)
        {
            foreach (var includeProperty in includeList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                data = data.Include(includeProperty);
            }

            return data;
        }

        public void Reload(TEntity entity)
        {
            Context.Entry(entity).Reload();
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Any(predicate);
        }

        public IEnumerable<T> Select<T>(Expression<Func<TEntity, T>> selector)
        {
            return DbSet.Select(selector);
        }

        public IOrderedQueryable<TEntity> OrderBy<T>(Expression<Func<TEntity, T>> selector)
        {
            return DbSet.OrderBy(selector);
        }

        public IOrderedQueryable<TEntity> OrderByDescending<T>(Expression<Func<TEntity, T>> selector)
        {
            return DbSet.OrderByDescending(selector);
        }

        public int Count()
        {
            return DbSet.Count();
        }

        public decimal Sum(Expression<Func<TEntity, decimal>> selector)
        {
            return DbSet.Sum(selector);
        }

    }
}