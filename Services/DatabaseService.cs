using GameTimeMonitor.Models;
using Microsoft.Data.Sqlite;

namespace GameTimeMonitor.Services
{
    public class DatabaseService
    {
        private string dbFilePath = "playtime.db";

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
    }
}
