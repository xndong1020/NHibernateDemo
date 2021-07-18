using System.Data;
using System.Threading.Tasks;
using NHibernate;

namespace NHibernateDemo.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISession _session;
        private ITransaction _transaction;

        public UnitOfWork(ISession session)
        {
            _session = session;
        }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            // if a transaction has already active, then don't start another transaction
            if (_transaction != null && _transaction.IsActive) return _transaction;

            // if a transaction is inactive, then dispose it
            _transaction?.Dispose();

            _transaction = _session.BeginTransaction(isolationLevel);

            return _transaction;
        }

        public async Task Commit()
        {
            try
            {
                await _transaction.CommitAsync();
            }
            catch
            {
                await _transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Rollback()
        {
            await _transaction.RollbackAsync();
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }
}
