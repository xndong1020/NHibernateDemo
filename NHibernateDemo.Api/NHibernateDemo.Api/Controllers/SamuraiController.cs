using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate.Linq;
using NHibernateDemo.Api.Dtos;
using NHibernateDemo.Entity.Models;
using NHibernateDemo.Services;

namespace NHibernateDemo.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SamuraiController : ControllerBase
    {
        private readonly ILogger<SamuraiController> _logger;
        private readonly ISamuraiService _samuraiService;

        public SamuraiController(ILogger<SamuraiController> logger, ISamuraiService samuraiService)
        {
            _logger = logger;
            _samuraiService = samuraiService;
        }

        [HttpGet]
        public async Task<IEnumerable<Samurai>> Get(int? id)
        {
            if (!id.HasValue) return await _samuraiService.GetAll().ToListAsync();

            var samurai = await _samuraiService.GetById(id.Value);
            return new List<Samurai>() { samurai };
        }


        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody] SamuraiCreateInput input)
        {
            var newSamurai = new Samurai { Name = input.Name };

            foreach (var quote in input.Quotes)
                newSamurai.Quotes.Add(new Quote() { Text = quote.Text, Samurai = newSamurai });

            await _samuraiService.Create(newSamurai);

            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult<int>> Put([FromBody] SamuraiUpdateInput input)
        {
            var oldSamurai = await _samuraiService.GetById(input.Id);

            if (oldSamurai == null) return NotFound();

            oldSamurai.Name = input.Name;
            oldSamurai.Quotes.Clear();
            foreach (var quote in input.Quotes)
                oldSamurai.Quotes.Add(new Quote() { Text = quote.Text, Samurai = oldSamurai });

            await _samuraiService.Update(oldSamurai.Id, oldSamurai);

            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult<int>> Delete([FromBody] int id)
        {
            var oldSamurai = await _samuraiService.GetById(id);
            if (oldSamurai == null) return NotFound();
            await _samuraiService.Delete(oldSamurai);
            return NoContent();
        }
    }
}
