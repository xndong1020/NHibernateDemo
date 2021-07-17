using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace NHibernateDemo.Entity.Models 
{
    public class Samurai : BaseEntity
    {
        public virtual string Name { get; set; }

        public virtual ISet<Quote> Quotes { get; set; } = new HashSet<Quote>();
    }


    public class SamuraiMap : ClassMap<Samurai>
    {
        public SamuraiMap()
        {
            Table("samurais");
            Schema("public");

            Id(x => x.Id).Column("id").GeneratedBy.Increment();
            Map(x => x.Name).Column("name");

            HasMany(x => x.Quotes).Inverse().Cascade.All();
        }
    }
}
