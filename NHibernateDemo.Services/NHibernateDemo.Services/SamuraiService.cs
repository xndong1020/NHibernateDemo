using System;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernateDemo.Data;
using NHibernateDemo.Entity.Models;

namespace NHibernateDemo.Services
{
    public class SamuraiService : ISamuraiService
    {
        private readonly IGenericRepository<Samurai> _repository;
        public SamuraiService(ISession session, IGenericRepository<Samurai> repository)
        {
            _repository = repository;
        }
        public IQueryable<Samurai> GetAll()
        {
            return _repository.GetAll();
        }

        public Task<Samurai> GetById(int id)
        {
            return _repository.GetById(id);
        }

        public async Task Create(Samurai samurai)
        {
            try
            {
                _repository.BeginTransaction();
                await _repository.Create(samurai);
                await _repository.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await _repository.Rollback();
                throw;
            }
            finally
            {
                _repository.CloseTransaction();
            }
        }

        public async Task Update(int id, Samurai samurai)
        {
            try
            {
                _repository.BeginTransaction();
                await _repository.Update(id, samurai);
                await _repository.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await _repository.Rollback();
                throw;
            }
            finally
            {
                _repository.CloseTransaction();
            }
        }

        public async Task Delete(Samurai entity)
        {
            try
            {
                _repository.BeginTransaction();
                await _repository.Delete(entity);
                await _repository.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await _repository.Rollback();
                throw;
            }
            finally
            {
                _repository.CloseTransaction();
            }
        }
    }
}
