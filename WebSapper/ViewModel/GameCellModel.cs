namespace WebSapper.ViewModel
{
    public class GameCellModel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsOpened { get; set; }
        public bool? IsBomb { get; set; }
        public int? CountBombAround { get; set; }
    }
}