using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using WebSapper.Models;

namespace WebSapper.Mappings
{
    public class GameCellMap : ClassMapping<GameCell>
    {
        public GameCellMap()
        {
            Id(x => x.Id, map => map.Generator(Generators.Native));
            Property(x => x.X);
            Property(x => x.Y);
            Property(x => x.IsOpened);
            Property(x => x.IsBomb);
            Property(x => x.CountBombAround);
            ManyToOne(x => x.Game, c => c.Column("GameId"));
        }
    }
}