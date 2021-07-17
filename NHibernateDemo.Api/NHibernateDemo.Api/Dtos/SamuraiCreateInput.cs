using System.Collections.Generic;

namespace NHibernateDemo.Api.Dtos
{
    public class SamuraiCreateInput
    {
        public string Name { get; set; }

        public IList<QuoteCreateInput> Quotes { get; set; } = new List<QuoteCreateInput>();
    }
}
