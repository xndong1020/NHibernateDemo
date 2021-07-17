using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernateDemo.Entity.Models;

namespace NHibernateDemo.Data
{
    public class SamuraiAppDbContext : ISamuraiAppDbContext
    {
        private readonly ISession _session;
        private ITransaction _transaction;

        public SamuraiAppDbContext(ISession session)
        {
            _session = session;
        }

        public IQueryable<Samurai> Samurais => _session.Query<Samurai>();
        public IQueryable<Quote> Quotes => _session.Query<Quote>();

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

        public async Task Save(Samurai entity)
        {
            await _session.SaveOrUpdateAsync(entity);
        }

        public async Task Delete(Samurai entity)
        {
            await _session.DeleteAsync(entity);
        }
    }
}
