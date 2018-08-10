using System.Linq;
using System.Threading.Tasks;
using WebSapper.Enum;
using WebSapper.Exception;
using WebSapper.Models;
using WebSapper.ViewModel;

namespace WebSapper.Service.GameStates
{
    public class WonState : IGameState
    {
        public GameState State => GameState.Won;

        public GameModel Get(Game game)
        {
            return new GameModel
            {
                Id = game.Id,
                Name = game.Name,
                Width = game.Width,
                Height = game.Height,
                State = game.State,
                Cells = game.Cells.Select(it => new GameCellModel
                {
                    X = it.X,
                    Y = it.Y,
                    IsOpened = it.IsOpened,
                    IsBomb = it.IsBomb,
                    CountBombAround = it.CountBombAround
                }).ToList()
            };
        }

        public GameCellModel Open(Game game, int x, int y)
        {
            throw new GameException("Игра выиграна, нельзя ходить больше");
        }
    }
}