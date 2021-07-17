using System.Linq;
using System.Threading.Tasks;
using NHibernateDemo.Entity.Models;

namespace NHibernateDemo.Services
{
    public interface ISamuraiService
    {
        IQueryable<Samurai> GetAll();

        Task<Samurai> GetById(int id);

        Task Create(Samurai entity);

        Task Update(int id, Samurai entity);

        Task Delete(Samurai entity);
    }
}
