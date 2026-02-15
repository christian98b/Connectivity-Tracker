namespace Connectivity_Tracker.Models
{
    public class NetworkMetrics
    {
        public DateTime Timestamp { get; set; }
        public long PingLatency { get; set; }
        public bool PingSuccess { get; set; }
        public double DownloadSpeed { get; set; }
        public double UploadSpeed { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Context { get; set; } = string.Empty;
    }
}
