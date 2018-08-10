using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using WebSapper.Models;

namespace WebSapper.Repository
{
    public class GameRepository : IGameRepository
    {
        private readonly ISession _session;
        
        public GameRepository(ISession session)
        {
            _session = session;
        }
        
        public async Task<Game> Get(int id)
        {
            return await _session.Query<Game>().FirstOrDefaultAsync(it => it.Id == id);
        }

        public async Task SaveOrUpdate(Game game)
        {
            await _session.SaveOrUpdateAsync(game);
            await _session.FlushAsync();
        }

        public async Task<IEnumerable<Game>> GetAll()
        {
            return await _session.Query<Game>().ToListAsync();
        }
    }
}