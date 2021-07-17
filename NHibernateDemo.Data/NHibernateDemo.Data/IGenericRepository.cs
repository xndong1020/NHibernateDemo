using System.Linq;
using System.Threading.Tasks;
using NHibernateDemo.Entity.Models;

namespace NHibernateDemo.Data
{
    public interface IGenericRepository<TEntity> where TEntity : class, IEntity
    {
        IQueryable<TEntity> GetAll();

        Task<TEntity> GetById(int id);

        Task Create(TEntity entity);

        Task Update(int id, TEntity entity);

        Task Delete(int id);

        void BeginTransaction();
        Task Commit();
        Task Rollback();
        void CloseTransaction();
    }
}
