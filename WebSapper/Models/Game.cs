using System.Collections.Generic;
using WebSapper.Enum;

namespace WebSapper.Models
{
    public class Game
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }
        public virtual GameState State { get; set; }
        public virtual IList<GameCell> Cells { get; set; }
    }
}