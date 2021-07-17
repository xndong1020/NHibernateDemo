using System.Collections.Generic;

namespace NHibernateDemo.Api.Dtos
{
    public class SamuraiUpdateInput
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IList<QuoteCreateInput> Quotes { get; set; } = new List<QuoteCreateInput>();
    }
}
