using System.Collections.Generic;
using System.Threading.Tasks;
using WebSapper.Models;
using WebSapper.ViewModel;

namespace WebSapper.Service
{
    public interface IGameService
    {
        Task<GameModel> Create(int width, int height, int bombChance);
        Task<GameModel> Get(int id);
        Task<GameCellModel> Open(int id, int x, int y);
        Task<IEnumerable<GameModel>> GetAll();
    }
}