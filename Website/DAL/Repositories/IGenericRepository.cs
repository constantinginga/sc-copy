using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StartupCentral.DAL.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class, new()
    {
        IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "");

        IQueryable<TEntity> WhereLazy(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "");

        Task<IEnumerable<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "");

        IQueryable<TEntity> AsQueryable(string includeProperties = "");

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "");

        TEntity GetFirst(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "");

        IEnumerable<TEntity> GetAll();

        IQueryable<TEntity> All();

        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate);

        TEntity GetById(params object[] id);

        Task<TEntity> GetByIdAsync(params object[] id);

        void Insert(TEntity entity);

        void InsertRange(List<TEntity> entities);

        void Delete(object id);

        void Delete(TEntity entityToDelete);

        void Update(TEntity entityToUpdate);

        void Reload(TEntity entity);

        IQueryable<TEntity> Include<TProperty>(Expression<Func<TEntity, TProperty>> path);

        bool Any(Expression<Func<TEntity, bool>> predicate);

        IEnumerable<T> Select<T>(Expression<Func<TEntity, T>> selector);

        IOrderedQueryable<TEntity> OrderBy<T>(Expression<Func<TEntity, T>> selector);

        IOrderedQueryable<TEntity> OrderByDescending<T>(Expression<Func<TEntity, T>> selector);

        int Count();

        decimal Sum(Expression<Func<TEntity, decimal>> selector);
    }
}