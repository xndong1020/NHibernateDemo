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

        //private readonly ISession _session;
        private readonly ISamuraiAppDbContext _session;

        public SamuraiController(ILogger<SamuraiController> logger, ISamuraiAppDbContext session)
        {
            _logger = logger;
            _session = session;
        }

        [HttpGet]
        public async Task<IEnumerable<Samurai>> Get()
        {
            try
            {
                _session.BeginTransaction();
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


                await _session.Save(newSamurai);

                await _session.Commit();

                var samurais = await _session.Samurais.ToListAsync();

                return samurais;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await _session.Rollback();
                throw;
            }
            finally
            {
                _session.CloseTransaction();
            }
        }
    }
}
