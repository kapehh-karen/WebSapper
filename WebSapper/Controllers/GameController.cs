using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;
using WebSapper.Controllers.Models;
using WebSapper.Models;
using WebSapper.Service;
using WebSapper.ViewModel;

namespace WebSapper.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost]
        public async Task<ActionResult<GameModel>> New([FromBody] NewGameModel model)
        {
            if (model.BombChance < 0 || model.BombChance > 100 ||
                model.Width < 1 || model.Height < 1 ||
                model.Width > 20 || model.Height > 20)
                return BadRequest("Некорректные входные данные");
            
            var gameModel = await _gameService.Create(model.Width, model.Height, model.BombChance);
            return Ok(gameModel);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameModel>>> List()
        {
            var gameModels = await _gameService.GetAll();
            return Ok(gameModels);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GameModel>> Get(int id)
        {
            var gameModel = await _gameService.Get(id);
            if (gameModel is null)
                return NotFound();
            return Ok(gameModel);
        }

        [HttpPut]
        public async Task<ActionResult<GameCellModel>> Open([FromBody] OpenGameCellModel model)
        {
            var cellModel = await _gameService.Open(model.GameId, model.X, model.Y);
            if (cellModel is null)
                return NotFound();
            return Ok(cellModel);
        }
    }
}