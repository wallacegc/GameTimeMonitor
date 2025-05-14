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
        private readonly string _databasePath = "playtime.db";
        private readonly string _gamesJsonPath = Path.Combine("Database", "games.json");

        public void UpdateGamesJson()
        {
            if (!File.Exists(_databasePath))
                throw new FileNotFoundException("Database file 'playtime.db' not found.");

            using var connection = new SqliteConnection($"Data Source={_databasePath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"SELECT DISTINCT game_name FROM sessions";

            var reader = command.ExecuteReader();

            var newGames = new List<dynamic>();
            while (reader.Read())
            {
                string gameName = reader.GetString(0);
                newGames.Add(new
                {
                    Name = gameName,
                    Process = "no_process"
                });
            }

            Directory.CreateDirectory("Database");

            List<dynamic> existingGames;
            if (File.Exists(_gamesJsonPath))
            {
                existingGames = JsonConvert.DeserializeObject<List<dynamic>>(File.ReadAllText(_gamesJsonPath)) ?? new List<dynamic>();
            }
            else
            {
                existingGames = new List<dynamic>();
            }

            foreach (var newGame in newGames)
            {
                bool exists = existingGames.Any(g => g.Name == newGame.Name);
                if (!exists)
                {
                    existingGames.Add(newGame);
                }
            }

            File.WriteAllText(_gamesJsonPath, JsonConvert.SerializeObject(existingGames, Formatting.Indented));
        }
    }
}
