namespace Connectivity_Tracker.Models
{
    public enum TrayMetricType
    {
        Ping,
        PacketLoss
    }

    public class AppSettings
    {
        public int PingIntervalSeconds { get; set; } = 10;
        public string PingTarget { get; set; } = "8.8.8.8";
        public int AlertThresholdMs { get; set; } = 200;
        public double PacketLossAlertThreshold { get; set; } = 10.0;
        public bool StartWithWindows { get; set; } = false;
        public bool MinimizeToTrayOnStartup { get; set; } = true;
        public bool ShowPingInTaskbar { get; set; } = true;
        public bool ShowPingInTray { get; set; } = true;
        public TrayMetricType TrayMetric { get; set; } = TrayMetricType.Ping;
    }
}
