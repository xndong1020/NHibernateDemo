using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NHibernateDemo.Api.Dtos
{
    public class SamuraiUpdateInput
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; }

        public IList<QuoteCreateInput> Quotes { get; set; } = new List<QuoteCreateInput>();
    }
}
