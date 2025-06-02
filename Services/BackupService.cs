using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

namespace GameTimeMonitor.Services
{
    internal class BackupService
    {
        // Path to the SQLite database file in AppData/GameTimeMonitor folder
        private readonly string _databasePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GameTimeMonitor",
            "playtime.db"
        );

        // Path to the JSON file storing games info
        private readonly string _gamesJsonPath = Path.Combine("Database", "games.json");

        // Updates the games.json file with any new game names found in the database sessions
        public void UpdateGamesJson()
        {
            if (!File.Exists(_databasePath))
                throw new FileNotFoundException($"Database file not found at expected path: {_databasePath}");

            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT DISTINCT game_name FROM sessions";

            using var reader = command.ExecuteReader();

            var newGames = new List<dynamic>();

            while (reader.Read())
            {
                string gameName = reader.GetString(0);
                newGames.Add(new
                {
                    Name = gameName,
                    Process = "no_process" // Default placeholder for process name
                });
            }

            Directory.CreateDirectory("Database");

            // Read existing games.json or initialize an empty list
            List<dynamic> existingGames = File.Exists(_gamesJsonPath)
                ? JsonConvert.DeserializeObject<List<dynamic>>(File.ReadAllText(_gamesJsonPath)) ?? new List<dynamic>()
                : new List<dynamic>();

            // Add only new games that do not exist already
            foreach (var newGame in newGames)
            {
                bool exists = existingGames.Any(g => g.Name == newGame.Name);
                if (!exists)
                {
                    existingGames.Add(newGame);
                }
            }

            // Save updated list back to games.json with indentation for readability
            File.WriteAllText(_gamesJsonPath, JsonConvert.SerializeObject(existingGames, Formatting.Indented));
        }
    }
}
