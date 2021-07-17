using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using NHibernateDemo.Data;
using NHibernateDemo.Entity.Models;

namespace NHibernateDemo.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SamuraiController : ControllerBase
    {
        private readonly ILogger<SamuraiController> _logger;

        private readonly ISession _session;
        private readonly IGenericRepository<Samurai> _repository;

        public SamuraiController(ILogger<SamuraiController> logger, ISession session, IGenericRepository<Samurai> repository)
        {
            _logger = logger;
            _session = session;
            _repository = repository;
        }

        [HttpGet]
        public async Task<IEnumerable<Samurai>> Get()
        {
            try
            {
                _repository.BeginTransaction();
                var newSamurai = new Samurai
                {
                    Name = "Hahahaahah007"

                };

                newSamurai.Quotes = new HashSet<Quote>
                {
                    new Quote() {Text = "Hahahaahah007 text1", Samurai = newSamurai},
                    new Quote() {Text = "Hahahaahah007 text2", Samurai = newSamurai},
                    new Quote() {Text = "Hahahaahah007 text3", Samurai = newSamurai}
                };


                await _repository.Create(newSamurai);

                await _repository.Commit();

                //var samurais = await _session.Samurais.ToListAsync();
                var samurais = await _repository.GetAll().ToListAsync();

                return samurais;
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
