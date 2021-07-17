using FluentNHibernate.Mapping;

namespace NHibernateDemo.Entity.Models
{
    public class Quote : IEntity
    {
        public virtual int Id { get; set; }

        public virtual string Text { get; set; }

        public virtual Samurai Samurai { get; set; }
    }

    public class QuoteMap : ClassMap<Quote>
    {
        public QuoteMap()
        {
            Table("quotes");
            Schema("public");

            Id(x => x.Id).Column("id").GeneratedBy.Increment();
            Map(x => x.Text).Column("text");

            References(x => x.Samurai).Column("samurai_id");
        }
    }
}
