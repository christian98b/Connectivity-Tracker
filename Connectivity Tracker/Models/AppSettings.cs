namespace Connectivity_Tracker.Models
{
    public class AppSettings
    {
        public int PingIntervalSeconds { get; set; } = 10;
        public string PingTarget { get; set; } = "8.8.8.8";
        public int AlertThresholdMs { get; set; } = 200;
        public bool StartWithWindows { get; set; } = false;
        public bool MinimizeToTrayOnStartup { get; set; } = true;
    }
}
