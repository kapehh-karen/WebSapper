using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using WebSapper.Enum;
using WebSapper.Exception;
using WebSapper.Models;
using WebSapper.ViewModel;

namespace WebSapper.Service.GameStates
{
    public class RunningState : IGameState
    {
        public GameState State => GameState.Running;

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
                    IsBomb =
                        it.IsOpened
                            ? it.IsBomb
                            : (bool?) null, // Скрываем признак бомбы от пользователя, если ячейка закрыта
                    CountBombAround = it.IsOpened ? it.CountBombAround : (int?) null // ... по аналогии с IsBomb
                }).ToList()
            };
        }

        // Проверка на оставшиеся пустые клетки
        private bool ExistsEmptyCells(Game game)
        {
            // Если есть закрытые пустые клетки, то true
            return game.Cells.Any(it => !it.IsOpened && !it.IsBomb);
        }

        // Открытие всех пустых ячеек по соседству (сверху снизу справа слева)
        private void RunWaveEmptyOpen(Game game, int x, int y)
        {
            // NOTE: Если мы оказались здесь, значит мы в пустой клетке без бомбы!

            var map = new sbyte[game.Width, game.Height];
            const sbyte openCell = 1, emptyCell = 0, bombCell = -1, countCell = -2;

            // Отображаем на массиве наши ячейки
            foreach (var gameCell in game.Cells)
            {
                // -1 непроходимый участок (бомба) | -2 непроходимый участок (цифры) | 0 проходимый участок
                if (gameCell.IsBomb)
                    map[gameCell.X - 1, gameCell.Y - 1] = bombCell;
                else if (gameCell.CountBombAround > 0)
                    map[gameCell.X - 1, gameCell.Y - 1] = countCell;
                else
                    map[gameCell.X - 1, gameCell.Y - 1] = emptyCell;
            }

            var waveFlag = true;
            map[x - 1, y - 1] = openCell; // Инициализация начальной точки распространения волны

            while (waveFlag)
            {
                waveFlag = false;
                
                for (var _x = 0; _x < game.Width; _x++)
                for (var _y = 0; _y < game.Height; _y++)
                {
                    if (map[_x, _y] != openCell) continue;
                    
                    // Для данной клетки пытаемся распространить волну

                    if (_x - 1 >= 0 && map[_x - 1, _y] == emptyCell)
                    {
                        map[_x - 1, _y] = openCell;
                        waveFlag = true;
                    }
                    
                    if (_x + 1 < game.Width && map[_x + 1, _y] == emptyCell)
                    {
                        map[_x + 1, _y] = openCell;
                        waveFlag = true;
                    }
                    
                    if (_y - 1 >= 0 && map[_x, _y - 1] == emptyCell)
                    {
                        map[_x, _y - 1] = openCell;
                        waveFlag = true;
                    }
                    
                    if (_y + 1 < game.Height && map[_x, _y + 1] == emptyCell)
                    {
                        map[_x, _y + 1] = openCell;
                        waveFlag = true;
                    }
                }
            }
            
            // Открываем все открытые клетки
            foreach (var gameCell in game.Cells)
            {
                var _x = gameCell.X - 1;
                var _y = gameCell.Y - 1;
                
                if (map[_x, _y] == openCell) // Если клетка должна быть открыта
                {
                    gameCell.IsOpened = true;
                }
                else if (map[_x, _y] == countCell) // Если клетка с цифрой
                {
                    // Ищем вокруг openCell и если есть, то открываем текущую countCell
                    if (_x - 1 >= 0 && _y - 1 >= 0 && map[_x - 1, _y - 1] == openCell)
                        gameCell.IsOpened = true;
                    if (_y - 1 >= 0 && map[_x, _y - 1] == openCell)
                        gameCell.IsOpened = true;
                    if (_x + 1 < game.Width && _y - 1 >= 0 && map[_x + 1, _y - 1] == openCell)
                        gameCell.IsOpened = true;
                    if (_x - 1 >= 0 && map[_x - 1, _y] == openCell)
                        gameCell.IsOpened = true;
                    if (_x + 1 < game.Width && map[_x + 1, _y] == openCell)
                        gameCell.IsOpened = true;
                    if (_x - 1 >= 0 && _y + 1 < game.Height && map[_x - 1, _y + 1] == openCell)
                        gameCell.IsOpened = true;
                    if (_y + 1 < game.Height && map[_x, _y + 1] == openCell)
                        gameCell.IsOpened = true;
                    if (_x + 1 < game.Width && _y + 1 < game.Height && map[_x + 1, _y + 1] == openCell)
                        gameCell.IsOpened = true;
                }
            }
        }

        public GameCellModel Open(Game game, int x, int y)
        {
            var cell = game.Cells.FirstOrDefault(it => it.X == x && it.Y == y);

            if (cell is null)
                throw new GameException($"Ячейка по координатам X:{x} Y:{y} не найдена");

            if (cell.IsOpened)
                throw new GameException($"Ячейка по координатам X:{x} Y:{y} уже открыта");

            cell.IsOpened = true;

            if (cell.IsBomb) // Если бомба - переводим в состояние проигрыша
                game.State = GameState.Lose;
            else if (!ExistsEmptyCells(game)) // Есть ли ещё закрытые пустые клетки, если нету, то это победа
                game.State = GameState.Won;
            else // Если это обычный ход без последствий для состояния, то открываем все пустые клетки по соседству
                RunWaveEmptyOpen(game, x, y);

            return new GameCellModel
            {
                X = cell.X,
                Y = cell.Y,
                IsBomb = cell.IsBomb,
                IsOpened = cell.IsOpened,
                CountBombAround = cell.CountBombAround
            };
        }
    }
}