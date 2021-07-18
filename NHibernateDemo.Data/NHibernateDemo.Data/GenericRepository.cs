using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NHibernate;
using NHibernateDemo.Entity.Models;

namespace NHibernateDemo.Data
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly ISession _session;

        public GenericRepository(ISession session)
        {
            _session = session;
        }

        public IQueryable<TEntity> GetAll()
        {
            return _session.Query<TEntity>();
        }

        public Task<TEntity> GetById(int id)
        {
            return _session.GetAsync<TEntity>(id);
        }

        public TEntity FindBy(Expression<Func<TEntity, bool>> expression)
        {
            return FilterBy(expression).Single();
        }

        public IQueryable<TEntity> FilterBy(Expression<Func<TEntity, bool>> expression)
        {
            return GetAll().Where(expression).AsQueryable();
        }

        public Task Create(TEntity entity)
        {
            return _session.SaveAsync(entity);
        }

        public Task Update(int id, TEntity entity)
        {
            return _session.UpdateAsync(entity, id);
        }

        public Task Delete(TEntity entity)
        {
            return _session.DeleteAsync(entity);
        }
    }
}
