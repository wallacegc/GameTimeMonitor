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

            // Get all sessions ordered by game and start time
            var sessions = new List<SessionRecord>();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = @"
        SELECT id, game_name, start_time, end_time
        FROM sessions
        ORDER BY game_name, start_time;
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

            var idsToRemove = new HashSet<long>();
            var seenSessions = new List<SessionRecord>();

            // Remove sessions with duration less than 1 minute
            foreach (var session in sessions)
            {
                var duration = (session.EndTime - session.StartTime).TotalMinutes;
                if (duration < 1)
                {
                    idsToRemove.Add(session.Id);
                }
            }

            // Remove exact duplicates with time tolerance
            foreach (var current in sessions)
            {
                // Skip sessions already marked for removal
                if (idsToRemove.Contains(current.Id)) continue;

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

            // Remove overlapping sessions (partial or full overlap)
            var groupedByGame = sessions.GroupBy(s => s.GameName);

            foreach (var group in groupedByGame)
            {
                var sortedSessions = group.OrderBy(s => s.StartTime).ToList();

                for (int i = 0; i < sortedSessions.Count - 1; i++)
                {
                    var current = sortedSessions[i];
                    var next = sortedSessions[i + 1];

                    // Skip sessions already marked for removal
                    if (idsToRemove.Contains(current.Id) || idsToRemove.Contains(next.Id))
                        continue;

                    // Check for overlap
                    if (current.EndTime >= next.StartTime)
                    {
                        if (current.EndTime >= next.EndTime)
                        {
                            // Next session is fully contained within the current session
                            idsToRemove.Add(next.Id);
                        }
                        else
                        {
                            // Partial overlap
                            var overlapDuration = (current.EndTime - next.StartTime).TotalMinutes;

                            if (overlapDuration >= 1) // minimum overlap threshold
                            {
                                var currentDuration = (current.EndTime - current.StartTime).TotalMinutes;
                                var nextDuration = (next.EndTime - next.StartTime).TotalMinutes;

                                // Remove shorter session
                                if (nextDuration < currentDuration)
                                    idsToRemove.Add(next.Id);
                                else
                                    idsToRemove.Add(current.Id);
                            }
                        }
                    }
                }
            }

            // Delete all marked sessions
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
