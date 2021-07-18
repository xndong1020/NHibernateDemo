using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NHibernateDemo.Api.Dtos
{
    public class SamuraiCreateInput
    {
        [Required]
        [MinLength(2)]
        public string Name { get; set; }

        public IList<QuoteCreateInput> Quotes { get; set; } = new List<QuoteCreateInput>();
    }
}
