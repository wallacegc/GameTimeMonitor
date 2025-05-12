using System.Diagnostics;
using GameTimeMonitor.Models;

namespace GameTimeMonitor.Services
{
    public class GameMonitoringService
    {
        private readonly DatabaseService _databaseService;
        private Dictionary<string, DateTime> gameStartTimes = new();

        // Event to notify the interface that the game status has changed (Game Name, Status - Running/Stopped)
        public event Action<string, string> GameStatusChanged;

        // Event to notify the interface that the game data has been updated
        public event Action GameUpdated;

        // Constructor to initialize the GameMonitoringService with a database service
        public GameMonitoringService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // Method to start monitoring the list of games
        public void StartMonitoring(List<Game> games)
        {
            new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        MonitorRunningGames(games);
                        Thread.Sleep(5000); // Wait 5 seconds before checking again
                    }
                }
                catch (Exception ex)
                {
                    // Catch any exceptions that may occur in the thread
                    MessageBox.Show($"Error in game monitoring: {ex.Message}");
                }
            }).Start();
        }

        // Method to monitor the running games and update status
        private void MonitorRunningGames(List<Game> games)
        {
            foreach (var game in games)
            {
                if (IsProcessRunning(game.Process))
                {
                    if (!gameStartTimes.ContainsKey(game.Name))
                    {
                        gameStartTimes[game.Name] = DateTime.Now;
                        GameStatusChanged?.Invoke(game.Name, "Running");
                    }
                }
                else if (gameStartTimes.ContainsKey(game.Name))
                {
                    DateTime start = gameStartTimes[game.Name];
                    DateTime end = DateTime.Now;
                    TimeSpan duration = end - start;

                    // Save the session to the database
                    _databaseService.SaveSessionToDatabase(new Session
                    {
                        GameName = game.Name,
                        StartTime = start,
                        EndTime = end,
                        DurationMinutes = duration.TotalMinutes
                    });

                    gameStartTimes.Remove(game.Name);
                    GameUpdated?.Invoke();
                    GameStatusChanged?.Invoke(game.Name, $"Stopped - Duration: {Math.Round(duration.TotalMinutes)} min");
                }
            }
        }

        // Method to check if a process is running by its name
        private bool IsProcessRunning(string processName) =>
            Process.GetProcessesByName(processName).Length > 0;
    }
}
