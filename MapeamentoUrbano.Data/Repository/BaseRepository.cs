using MapeamentoUrbano.Data.Context;
using MapeamentoUrbano.Domain.Interfaces.Repository;
using System.Linq.Expressions;

namespace MapeamentoUrbano.Data.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        public CoreContext CoreContext { get; }


        public BaseRepository(CoreContext context)
        {
            CoreContext = context;
        }   

        public void AddOrUpdate(TEntity entity)
        {
            CoreContext.Set<TEntity>().Update(entity);
        }

        public void AddOrUpdateRange(IEnumerable<TEntity> entities)
        {
            CoreContext.Set<TEntity>().UpdateRange(entities);
        }

        public void Add(TEntity entity)
        {
            CoreContext.Set<TEntity>().Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities) 
        {
            CoreContext.Set<TEntity>().AddRange(entities);
        }

        public void Delete(TEntity entity)
        {
            CoreContext.Set<TEntity>().Remove(entity);
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            CoreContext.Set<TEntity>().RemoveRange(entities);
        }

        public IQueryable<TEntity> GetAll()
        {
            return CoreContext.Set<TEntity>().AsQueryable();
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return CoreContext.Set<TEntity>().Where(predicate).AsQueryable();
        }

        public void Update(TEntity entity)
        {
            CoreContext.Set<TEntity>().Update(entity);
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            CoreContext.Set<TEntity>().UpdateRange(entities);
        }

        public TEntity? Get(Expression<Func<TEntity, bool>> predicate)
        {
            return CoreContext.Set<TEntity>().Where(predicate).FirstOrDefault();
        }
    }
}
