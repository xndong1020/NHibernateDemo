using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NHibernate;
using NHibernateDemo.Data;
using NHibernateDemo.Entity.Models;

namespace NHibernateDemo.Services
{
    public class SamuraiService : ISamuraiService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Samurai> _repository;
        public SamuraiService(ISession session, IGenericRepository<Samurai> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
        public IQueryable<Samurai> GetAll()
        {
            return _repository.GetAll();
        }

        public Task<Samurai> GetById(int id)
        {
            return _repository.GetById(id);
        }

        public Samurai FindBy(Expression<Func<Samurai, bool>> expression)
        {
            return _repository.FindBy(expression);
        }

        public async Task Create(Samurai samurai)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                await _repository.Create(samurai);
                await _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await _unitOfWork.Rollback();
                throw;
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public async Task Update(int id, Samurai samurai)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                await _repository.Update(id, samurai);
                await _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await _unitOfWork.Rollback();
                throw;
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public async Task Delete(Samurai entity)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                await _repository.Delete(entity);
                await _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await _unitOfWork.Rollback();
                throw;
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
    }
}
