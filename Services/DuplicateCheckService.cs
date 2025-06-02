using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

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

        // Removes duplicate and overlapping sessions from the database
        // Returns the number of deleted records
        public int RemoveDuplicateSessions()
        {
            using var connection = new SqliteConnection($"Data Source={dbFilePath}");
            connection.Open();

            // Load all sessions ordered by game and start time
            var sessions = LoadAllSessions(connection);

            var idsToRemove = new HashSet<long>();

            // Mark sessions shorter than 1 minute for removal
            foreach (var session in sessions)
            {
                if ((session.EndTime - session.StartTime).TotalMinutes < 1)
                    idsToRemove.Add(session.Id);
            }

            // Remove exact duplicates within time tolerance
            RemoveExactDuplicates(sessions, idsToRemove);

            // Remove overlapping sessions
            RemoveOverlappingSessions(sessions, idsToRemove);

            // Delete sessions marked for removal from the database
            return DeleteSessionsByIds(connection, idsToRemove);
        }

        private List<SessionRecord> LoadAllSessions(SqliteConnection connection)
        {
            var sessions = new List<SessionRecord>();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = @"
                SELECT id, game_name, start_time, end_time
                FROM sessions
                ORDER BY game_name, start_time;
            ";

            using var reader = selectCmd.ExecuteReader();
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

            return sessions;
        }

        private void RemoveExactDuplicates(List<SessionRecord> sessions, HashSet<long> idsToRemove)
        {
            var seenSessions = new List<SessionRecord>();

            foreach (var current in sessions)
            {
                if (idsToRemove.Contains(current.Id)) continue;

                bool isDuplicate = seenSessions.Any(seen =>
                    current.GameName == seen.GameName &&
                    AreTimesClose(current.StartTime, seen.StartTime, timeToleranceSeconds) &&
                    AreTimesClose(current.EndTime, seen.EndTime, timeToleranceSeconds)
                );

                if (isDuplicate)
                    idsToRemove.Add(current.Id);
                else
                    seenSessions.Add(current);
            }
        }

        private void RemoveOverlappingSessions(List<SessionRecord> sessions, HashSet<long> idsToRemove)
        {
            var groupedByGame = sessions.GroupBy(s => s.GameName);

            foreach (var group in groupedByGame)
            {
                var sortedSessions = group.OrderBy(s => s.StartTime).ToList();

                for (int i = 0; i < sortedSessions.Count - 1; i++)
                {
                    var current = sortedSessions[i];
                    var next = sortedSessions[i + 1];

                    if (idsToRemove.Contains(current.Id) || idsToRemove.Contains(next.Id))
                        continue;

                    // Check if sessions overlap
                    if (current.EndTime >= next.StartTime)
                    {
                        if (current.EndTime >= next.EndTime)
                        {
                            // Next session is fully within current session, remove next
                            idsToRemove.Add(next.Id);
                        }
                        else
                        {
                            // Partial overlap - remove shorter session if overlap >= 1 minute
                            double overlapMinutes = (current.EndTime - next.StartTime).TotalMinutes;

                            if (overlapMinutes >= 1)
                            {
                                double currentDuration = (current.EndTime - current.StartTime).TotalMinutes;
                                double nextDuration = (next.EndTime - next.StartTime).TotalMinutes;

                                if (nextDuration < currentDuration)
                                    idsToRemove.Add(next.Id);
                                else
                                    idsToRemove.Add(current.Id);
                            }
                        }
                    }
                }
            }
        }

        private int DeleteSessionsByIds(SqliteConnection connection, HashSet<long> idsToRemove)
        {
            int deletedCount = 0;
            foreach (var id in idsToRemove)
            {
                using var deleteCmd = connection.CreateCommand();
                deleteCmd.CommandText = "DELETE FROM sessions WHERE id = $id";
                deleteCmd.Parameters.AddWithValue("$id", id);
                deletedCount += deleteCmd.ExecuteNonQuery();
            }
            return deletedCount;
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
