using GameTimeMonitor.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameTimeMonitor.Services
{
    public class DatabaseService
    {
        private readonly string dbFilePath;

        // Constructor initializes the database file path inside AppData/GameTimeMonitor
        public DatabaseService()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "GameTimeMonitor"
            );

            Directory.CreateDirectory(appDataPath);

            dbFilePath = Path.Combine(appDataPath, "playtime.db");
        }

        // Creates the sessions table if the database file doesn't exist yet
        public void InitializeDatabase()
        {
            if (!File.Exists(dbFilePath))
            {
                using var connection = new SqliteConnection($"Data Source={dbFilePath}");
                connection.Open();

                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS sessions (
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        game_name TEXT NOT NULL,
                        start_time TEXT NOT NULL,
                        end_time TEXT NOT NULL,
                        duration_minutes REAL NOT NULL
                    );
                ";
                cmd.ExecuteNonQuery();
            }
        }

        // Inserts a new game session record into the database
        public void SaveSessionToDatabase(Session session)
        {
            using var connection = new SqliteConnection($"Data Source={dbFilePath}");
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO sessions (game_name, start_time, end_time, duration_minutes)
                VALUES ($game, $start, $end, $duration);
            ";

            cmd.Parameters.AddWithValue("$game", session.GameName);
            cmd.Parameters.AddWithValue("$start", session.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("$end", session.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("$duration", session.DurationMinutes);
            cmd.ExecuteNonQuery();
        }

        // Retrieves total game time in minutes for a game between two dates
        public double GetGameTime(string game, DateTime from, DateTime to)
        {
            using var connection = new SqliteConnection($"Data Source={dbFilePath}");
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT SUM(duration_minutes)
                FROM sessions
                WHERE game_name = $game
                AND start_time >= $from
                AND end_time <= $to
            ";
            cmd.Parameters.AddWithValue("$game", game);
            cmd.Parameters.AddWithValue("$from", from.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("$to", to.ToString("yyyy-MM-dd HH:mm:ss"));

            var result = cmd.ExecuteScalar();
            return result != DBNull.Value ? Convert.ToDouble(result) : 0;
        }

        // Returns a list of game sessions for the specified game ordered by most recent end time
        public List<GameSession> GetSessionsForGame(string gameName)
        {
            var sessions = new List<GameSession>();

            using var connection = new SqliteConnection($"Data Source={dbFilePath}");
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT start_time, end_time
                FROM sessions
                WHERE game_name = $game AND end_time IS NOT NULL
                ORDER BY end_time DESC
            ";
            cmd.Parameters.AddWithValue("$game", gameName);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var start = reader.GetDateTime(0);
                var end = reader.GetDateTime(1);
                sessions.Add(new GameSession { StartTime = start, EndTime = end });
            }

            return sessions;
        }

        // Updates all session records in the database changing old game name to new game name
        public void UpdateGameNameInDatabase(string oldName, string newName)
        {
            using var connection = new SqliteConnection($"Data Source={dbFilePath}");
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                UPDATE sessions
                SET game_name = $newName
                WHERE game_name = $oldName;
            ";
            cmd.Parameters.AddWithValue("$newName", newName);
            cmd.Parameters.AddWithValue("$oldName", oldName);

            cmd.ExecuteNonQuery();
        }
    }
}
