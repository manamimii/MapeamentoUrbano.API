using System.Linq.Expressions;

namespace MapeamentoUrbano.Domain.Interfaces.Repository
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        void AddOrUpdate(TEntity entity);
        void AddOrUpdateRange(IEnumerable<TEntity> entities);
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        IQueryable<TEntity> GetAll();
        void Delete(TEntity entity);
        void DeleteRange(IEnumerable<TEntity> entities);
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        TEntity? Get(Expression<Func<TEntity, bool>> predicate);
    }
}
