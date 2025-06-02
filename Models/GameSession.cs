using System;

namespace GameTimeMonitor.Models
{
    public class GameSession
    {
        // Start time of the game session
        public DateTime StartTime { get; set; }

        // End time of the game session
        public DateTime EndTime { get; set; }

        // Duration of the session in whole minutes
        public int DurationMinutes => (int)(EndTime - StartTime).TotalMinutes;
    }
}
