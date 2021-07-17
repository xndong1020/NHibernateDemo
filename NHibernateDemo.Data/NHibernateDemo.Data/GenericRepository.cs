using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernateDemo.Entity.Models;

namespace NHibernateDemo.Data
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly ISession _session;
        private ITransaction _transaction;

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

        public Task Create(TEntity entity)
        {
           return _session.SaveAsync(entity);
        }

        public Task Update(int id, TEntity entity)
        {
            return _session.UpdateAsync(entity, id);
        }

        public Task Delete(int id)
        {
            return _session.DeleteAsync(id);
        }

        public void BeginTransaction()
        {
            _transaction = _session.BeginTransaction();
        }

        public async Task Commit()
        {
            await _transaction.CommitAsync();
        }

        public async Task Rollback()
        {
            await _transaction.RollbackAsync();
        }

        public void CloseTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }
}
