using System.Collections.Generic;
using WebSapper.Enum;

namespace WebSapper.ViewModel
{
    public class GameModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public GameState State { get; set; }
        public IList<GameCellModel> Cells { get; set; }
    }
}