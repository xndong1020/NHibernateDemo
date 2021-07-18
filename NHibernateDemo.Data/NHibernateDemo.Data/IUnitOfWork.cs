using System;
using System.Data;
using System.Threading.Tasks;
using NHibernate;

namespace NHibernateDemo.Data
{
    public interface IUnitOfWork : IDisposable
    {
        ITransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        Task Commit();

        Task Rollback();
    }
}
