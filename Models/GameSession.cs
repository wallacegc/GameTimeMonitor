using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTimeMonitor.Models
{
    public class GameSession
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }


        // Calculate the duration in minutes
        public int DurationMinutes => (int)(EndTime - StartTime).TotalMinutes;
    }

}
