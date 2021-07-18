using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NHibernateDemo.Entity.Models;

namespace NHibernateDemo.Services
{
    public interface ISamuraiService
    {
        IQueryable<Samurai> GetAll();

        Task<Samurai> GetById(int id);

        Samurai FindBy(Expression<Func<Samurai, bool>> expression);

        Task Create(Samurai entity);

        Task Update(int id, Samurai entity);

        Task Delete(Samurai entity);
    }
}
