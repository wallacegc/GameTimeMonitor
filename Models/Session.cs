namespace GameTimeMonitor.Models
{
    public class Session
    {
        public int Id { get; set; }
        public string GameName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double DurationMinutes { get; set; }
    }
}
