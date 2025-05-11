using System.Diagnostics;
using GameTimeMonitor.Models;
using GameTimeMonitor.Services;

namespace GameTimeMonitor.Services
{
    public class GameMonitoringService
    {
        private readonly DatabaseService _databaseService;
        private Dictionary<string, DateTime> gameStartTimes = new();

        public event Action<string, string> GameStatusChanged; // Nome do jogo, Status (Rodando/Parou)


        // Evento para notificar a interface de que os dados foram atualizados
        public event Action GameUpdated;

        public GameMonitoringService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public void StartMonitoring(List<Game> games)
        {
            new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        MonitorRunningGames(games);
                        Thread.Sleep(5000); // Aguarda 5 segundos antes de verificar novamente
                    }
                }
                catch (Exception ex)
                {
                    // Captura exceções que possam ocorrer na thread
                    MessageBox.Show($"Erro na monitorização do jogo: {ex.Message}");
                }
            }).Start();
        }
        private void MonitorRunningGames(List<Game> games)
        {
            foreach (var game in games)
            {
                if (IsProcessRunning(game.Process))
                {
                    if (!gameStartTimes.ContainsKey(game.Name))
                    {
                        gameStartTimes[game.Name] = DateTime.Now;
                        GameStatusChanged?.Invoke(game.Name, "Rodando");
                    }
                }
                else if (gameStartTimes.ContainsKey(game.Name))
                {
                    DateTime start = gameStartTimes[game.Name];
                    DateTime end = DateTime.Now;
                    TimeSpan duration = end - start;

                    _databaseService.SaveSessionToDatabase(new Session
                    {
                        GameName = game.Name,
                        StartTime = start,
                        EndTime = end,
                        DurationMinutes = duration.TotalMinutes
                    });

                    gameStartTimes.Remove(game.Name);
                    GameUpdated?.Invoke();
                    GameStatusChanged?.Invoke(game.Name, $"Parou - Duração: {Math.Round(duration.TotalMinutes)} min");
                }
            }
        }
        private bool IsProcessRunning(string processName) =>
            Process.GetProcessesByName(processName).Length > 0;
    }

}
