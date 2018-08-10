using Newtonsoft.Json;

namespace WebSapper.Models
{
    public class GameCell
    {
        public virtual int Id { get; set; }
        public virtual int X { get; set; }
        public virtual int Y { get; set; }
        public virtual bool IsOpened { get; set; }
        public virtual bool IsBomb { get; set; }
        public virtual int CountBombAround { get; set; }
        [JsonIgnore] public virtual Game Game { get; set; }
    }
}