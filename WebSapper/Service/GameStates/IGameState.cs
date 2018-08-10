using System.Threading.Tasks;
using WebSapper.Enum;
using WebSapper.Models;
using WebSapper.ViewModel;

namespace WebSapper.Service.GameStates
{
    public interface IGameState
    {
        GameState State { get; }
        
        /// <summary>
        /// Возвращает модель представления игроку исходя из текущего состояния игры
        /// </summary>
        /// <param name="game">Экземпляр игры</param>
        /// <returns>Модель представления</returns>
        GameModel Get(Game game);

        /// <summary>
        /// Открывает ячейку X, Y
        /// </summary>
        /// <param name="game">Экземпляр игры</param>
        /// <param name="x">X coord</param>
        /// <param name="y">Y coord</param>
        /// <returns>Открытая ячейка</returns>
        GameCellModel Open(Game game, int x, int y);
    }
}