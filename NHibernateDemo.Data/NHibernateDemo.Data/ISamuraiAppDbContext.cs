using System.Linq;
using System.Threading.Tasks;
using NHibernateDemo.Entity.Models;

namespace NHibernateDemo.Data
{
    public interface ISamuraiAppDbContext
    {
        void BeginTransaction();
        Task Commit();
        Task Rollback();
        void CloseTransaction();

        Task Save(Samurai entity);
        Task Delete(Samurai entity);

        IQueryable<Samurai> Samurais { get; }
        IQueryable<Quote> Quotes { get; }
    }
}
