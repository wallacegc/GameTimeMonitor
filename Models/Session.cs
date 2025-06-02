namespace GameTimeMonitor.Models
{
    public class Session
    {
        // Unique identifier for the session
        public int Id { get; set; }

        // Name of the game played in this session
        public string GameName { get; set; }

        // When the session started
        public DateTime StartTime { get; set; }

        // When the session ended
        public DateTime EndTime { get; set; }

        // Duration of the session in minutes
        public double DurationMinutes { get; set; }
    }
}
