namespace GameTimeMonitor.Models
{
    public class Game
    {
        // The name of the game
        public string Name { get; set; }

        // The process name used to detect if the game is running
        public string Process { get; set; }
    }
}