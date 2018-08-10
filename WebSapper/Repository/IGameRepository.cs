using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebSapper.Models;

namespace WebSapper.Repository
{
    public interface IGameRepository
    {
        Task<Game> Get(int id);
        Task SaveOrUpdate(Game game);
        Task<IEnumerable<Game>> GetAll();
    }
}