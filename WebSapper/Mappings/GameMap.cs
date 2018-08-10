using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using WebSapper.Models;

namespace WebSapper.Mappings
{
    public class GameMap : ClassMapping<Game>
    {
        public GameMap()
        {
            Id(x => x.Id, map => map.Generator(Generators.Native));
            Property(x => x.Name);
            Property(x => x.Width);
            Property(x => x.Height);
            Property(x => x.State);
            Bag(x => x.Cells,
                c =>
                {
                    c.Key(k => k.Column("GameId"));
                    c.Inverse(true);
                    c.Cascade(Cascade.Persist);
                },
                r => r.OneToMany());
        }
    }
}