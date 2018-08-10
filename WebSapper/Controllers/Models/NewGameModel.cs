namespace WebSapper.Controllers.Models
{
    public class NewGameModel
    {
        /// <summary>
        /// Шанс выпадения бомб
        /// </summary>
        public int BombChance { get; set; }
        
        /// <summary>
        /// Ширина
        /// </summary>
        public int Width { get; set; }
        
        /// <summary>
        /// Высота
        /// </summary>
        public int Height { get; set; }
    }
}