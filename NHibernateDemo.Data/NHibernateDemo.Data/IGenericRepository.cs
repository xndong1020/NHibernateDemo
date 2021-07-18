using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NHibernateDemo.Entity.Models;

namespace NHibernateDemo.Data
{
    public interface IGenericRepository<TEntity> where TEntity : class, IEntity
    {
        IQueryable<TEntity> GetAll();

        Task<TEntity> GetById(int id);

        TEntity FindBy(Expression<Func<TEntity, bool>> expression);

        IQueryable<TEntity> FilterBy(Expression<Func<TEntity, bool>> expression);

        Task Create(TEntity entity);

        Task Update(int id, TEntity entity);

        Task Delete(TEntity entity);
    }
}
