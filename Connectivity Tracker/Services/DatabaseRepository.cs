using Microsoft.Data.Sqlite;
using Connectivity_Tracker.Models;
using System.IO;

namespace Connectivity_Tracker.Services
{
    public class DatabaseRepository
    {
        private readonly string _connectionString;

        public DatabaseRepository()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ConnectivityTracker", "metrics.db");
            var directory = Path.GetDirectoryName(dbPath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS NetworkMetrics (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Timestamp TEXT NOT NULL,
                    PingLatency INTEGER NOT NULL,
                    PingSuccess INTEGER NOT NULL,
                    DownloadSpeed REAL NOT NULL,
                    UploadSpeed REAL NOT NULL,
                    Latitude REAL,
                    Longitude REAL,
                    Context TEXT,
                    PacketLossPercentage REAL DEFAULT 0
                )";
            command.ExecuteNonQuery();

            // Migration: Add PacketLossPercentage column if it doesn't exist
            try
            {
                var alterCommand = connection.CreateCommand();
                alterCommand.CommandText = @"
                    ALTER TABLE NetworkMetrics
                    ADD COLUMN PacketLossPercentage REAL DEFAULT 0";
                alterCommand.ExecuteNonQuery();
            }
            catch (SqliteException)
            {
                // Column already exists, ignore error
            }
        }

        public async Task SaveMetricsAsync(NetworkMetrics metrics)
        {
            await Task.Run(() =>
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO NetworkMetrics
                    (Timestamp, PingLatency, PingSuccess, DownloadSpeed, UploadSpeed, Latitude, Longitude, Context, PacketLossPercentage)
                    VALUES (@timestamp, @latency, @success, @download, @upload, @lat, @lon, @context, @packetLoss)";

                command.Parameters.AddWithValue("@timestamp", metrics.Timestamp.ToString("O"));
                command.Parameters.AddWithValue("@latency", metrics.PingLatency);
                command.Parameters.AddWithValue("@success", metrics.PingSuccess ? 1 : 0);
                command.Parameters.AddWithValue("@download", metrics.DownloadSpeed);
                command.Parameters.AddWithValue("@upload", metrics.UploadSpeed);
                command.Parameters.AddWithValue("@lat", (object?)metrics.Latitude ?? DBNull.Value);
                command.Parameters.AddWithValue("@lon", (object?)metrics.Longitude ?? DBNull.Value);
                command.Parameters.AddWithValue("@context", metrics.Context);
                command.Parameters.AddWithValue("@packetLoss", metrics.PacketLossPercentage);

                command.ExecuteNonQuery();
            });
        }

        public List<NetworkMetrics> GetMetrics(DateTime? startDate = null, DateTime? endDate = null, int limit = 1000)
        {
            var metrics = new List<NetworkMetrics>();

            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();

            if (startDate.HasValue && endDate.HasValue)
            {
                command.CommandText = @"
                    SELECT * FROM NetworkMetrics
                    WHERE Timestamp >= @start AND Timestamp <= @end
                    ORDER BY Timestamp DESC
                    LIMIT @limit";
                command.Parameters.AddWithValue("@start", startDate.Value.ToString("O"));
                command.Parameters.AddWithValue("@end", endDate.Value.ToString("O"));
            }
            else
            {
                command.CommandText = @"
                    SELECT * FROM NetworkMetrics
                    ORDER BY Timestamp DESC
                    LIMIT @limit";
            }

            command.Parameters.AddWithValue("@limit", limit);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                metrics.Add(new NetworkMetrics
                {
                    Timestamp = DateTime.Parse(reader.GetString(1)),
                    PingLatency = reader.GetInt64(2),
                    PingSuccess = reader.GetInt32(3) == 1,
                    DownloadSpeed = reader.GetDouble(4),
                    UploadSpeed = reader.GetDouble(5),
                    Latitude = reader.IsDBNull(6) ? null : reader.GetDouble(6),
                    Longitude = reader.IsDBNull(7) ? null : reader.GetDouble(7),
                    Context = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                    PacketLossPercentage = reader.IsDBNull(9) ? 0 : reader.GetDouble(9)
                });
            }

            return metrics;
        }

        public async Task DeleteOldMetricsAsync(int daysToKeep = 30)
        {
            await Task.Run(() =>
            {
                using var connection = new SqliteConnection(_connectionString);
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    DELETE FROM NetworkMetrics
                    WHERE Timestamp < @cutoff";

                var cutoff = DateTime.Now.AddDays(-daysToKeep);
                command.Parameters.AddWithValue("@cutoff", cutoff.ToString("O"));

                command.ExecuteNonQuery();
            });
        }

        public List<NetworkMetrics> ExportAllMetrics()
        {
            return GetMetrics(limit: int.MaxValue);
        }
    }
}
