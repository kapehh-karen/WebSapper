using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using WebSapper.Enum;
using WebSapper.Models;
using WebSapper.Repository;
using WebSapper.Service.GameStates;
using WebSapper.ViewModel;

namespace WebSapper.Service
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly Dictionary<GameState, IGameState> _gameStates;
        private readonly Random _random = new Random();

        public GameService(IGameRepository gameRepository, IEnumerable<IGameState> gameStates)
        {
            _gameRepository = gameRepository;
            _gameStates = gameStates.ToDictionary(key => key.State);
        }

        private void GenerateGameCells(Game game, int bombChance)
        {
            // Все числа менее 0 это бомбы, а все числа >= 0 это пустая клетка
            var mapsBomb = new sbyte[game.Width, game.Height];

            // Расставляем бомбы
            for (var x = 0; x < game.Width; x++)
            for (var y = 0; y < game.Height; y++)
                mapsBomb[x, y] = (sbyte) (_random.Next() % 100 < bombChance ? -1 : 0);

            // Для каждой ячейки считаем сколько бомб вокруг
            for (var x = 0; x < game.Width; x++)
            for (var y = 0; y < game.Height; y++)
                if (mapsBomb[x, y] == 0) // Если не бомба, разумеется
                {
                    sbyte mines = 0;

                    if (x - 1 >= 0 && y - 1 >= 0 && mapsBomb[x - 1, y - 1] < 0)
                        mines++;
                    if (y - 1 >= 0 && mapsBomb[x, y - 1] < 0)
                        mines++;
                    if (x + 1 < game.Width && y - 1 >= 0 && mapsBomb[x + 1, y - 1] < 0)
                        mines++;
                    if (x - 1 >= 0 && mapsBomb[x - 1, y] < 0)
                        mines++;
                    if (x + 1 < game.Width && mapsBomb[x + 1, y] < 0)
                        mines++;
                    if (x - 1 >= 0 && y + 1 < game.Height && mapsBomb[x - 1, y + 1] < 0)
                        mines++;
                    if (y + 1 < game.Height && mapsBomb[x, y + 1] < 0)
                        mines++;
                    if (x + 1 < game.Width && y + 1 < game.Height && mapsBomb[x + 1, y + 1] < 0)
                        mines++;

                    mapsBomb[x, y] = mines;
                }

            // Создаем поле для игры
            for (var x = 1; x <= game.Width; x++)
            for (var y = 1; y <= game.Height; y++)
            {
                var isBomb = mapsBomb[x - 1, y - 1] < 0;
                game.Cells.Add(new GameCell
                {
                    Game = game,
                    X = x,
                    Y = y,
                    IsOpened = false,
                    IsBomb = isBomb,
                    CountBombAround = isBomb ? 0 : mapsBomb[x - 1, y - 1]
                });
            }
        }

        public async Task<GameModel> Create(int width, int height, int bombChance)
        {
            var game = new Game
            {
                Name = $"Игра {DateTime.UtcNow.ToLocalTime()}",
                Width = width,
                Height = height,
                State = GameState.Running,
                Cells = new List<GameCell>()
            };

            GenerateGameCells(game, bombChance);
            await _gameRepository.SaveOrUpdate(game);

            return new GameModel
            {
                Id = game.Id,
                Name = game.Name,
                Width = game.Width,
                Height = game.Height,
                State = game.State
            };
        }

        public async Task<IEnumerable<GameModel>> GetAll()
        {
            var games = await _gameRepository.GetAll();
            var gameModels = games.Select(it => new GameModel
            {
                Id = it.Id,
                Name = it.Name,
                Width = it.Width,
                Height = it.Height,
                State = it.State
            });
            return gameModels;
        }

        public async Task<GameModel> Get(int id)
        {
            var game = await _gameRepository.Get(id);
            return game is null ? null : _gameStates[game.State].Get(game);
        }

        public async Task<GameCellModel> Open(int id, int x, int y)
        {
            var game = await _gameRepository.Get(id);
            if (game is null)
                return null;
            var cell = _gameStates[game.State].Open(game, x, y);
            await _gameRepository.SaveOrUpdate(game);
            return cell;
        }
    }
}