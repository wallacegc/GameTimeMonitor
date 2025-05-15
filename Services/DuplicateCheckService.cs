using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace GameTimeMonitor.Services
{
    public class DuplicateCheckService
    {
        private readonly string dbFilePath;
        private readonly int timeToleranceSeconds = 6;

        public DuplicateCheckService()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "GameTimeMonitor"
            );

            dbFilePath = Path.Combine(appDataPath, "playtime.db");
        }

        public int RemoveDuplicateSessions()
        {
            using var connection = new SqliteConnection($"Data Source={dbFilePath}");
            connection.Open();

            // Get all sessions ordered by game and time
            var sessions = new List<SessionRecord>();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = @"
                SELECT id, game_name, start_time, end_time
                FROM sessions
                ORDER BY game_name, start_time, end_time;
            ";

            using (var reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sessions.Add(new SessionRecord
                    {
                        Id = reader.GetInt64(0),
                        GameName = reader.GetString(1),
                        StartTime = DateTime.ParseExact(reader.GetString(2), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        EndTime = DateTime.ParseExact(reader.GetString(3), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                    });
                }
            }

            var idsToRemove = new List<long>();
            var seenSessions = new List<SessionRecord>();

            foreach (var current in sessions)
            {
                bool isDuplicate = false;

                foreach (var seen in seenSessions)
                {
                    if (current.GameName == seen.GameName &&
                        AreTimesClose(current.StartTime, seen.StartTime, timeToleranceSeconds) &&
                        AreTimesClose(current.EndTime, seen.EndTime, timeToleranceSeconds))
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (isDuplicate)
                {
                    idsToRemove.Add(current.Id);
                }
                else
                {
                    seenSessions.Add(current);
                }
            }

            // Delete duplicates
            int deleted = 0;
            foreach (var id in idsToRemove)
            {
                var deleteCmd = connection.CreateCommand();
                deleteCmd.CommandText = "DELETE FROM sessions WHERE id = $id";
                deleteCmd.Parameters.AddWithValue("$id", id);
                deleted += deleteCmd.ExecuteNonQuery();
            }

            return deleted;
        }

        private bool AreTimesClose(DateTime t1, DateTime t2, int toleranceSeconds)
        {
            return Math.Abs((t1 - t2).TotalSeconds) <= toleranceSeconds;
        }

        private class SessionRecord
        {
            public long Id { get; set; }
            public string GameName { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
        }
    }
}
