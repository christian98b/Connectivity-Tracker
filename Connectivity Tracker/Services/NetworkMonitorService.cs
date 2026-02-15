using System.Net.NetworkInformation;
using Connectivity_Tracker.Models;

namespace Connectivity_Tracker.Services
{
    public class NetworkMonitorService
    {
        private System.Threading.Timer? _pingTimer;
        private readonly string _pingTarget;
        private int _pingInterval;
        private readonly TrafficMonitorService _trafficMonitor;
        private NetworkMetrics _currentMetrics;

        public event EventHandler<NetworkMetrics>? MetricsUpdated;

        public NetworkMonitorService(string pingTarget = "8.8.8.8", int pingIntervalSeconds = 10)
        {
            _pingTarget = pingTarget;
            _pingInterval = pingIntervalSeconds * 1000;
            _trafficMonitor = new TrafficMonitorService();
            _currentMetrics = new NetworkMetrics();

            _trafficMonitor.TrafficUpdated += OnTrafficUpdated;
        }

        public void Start()
        {
            _pingTimer = new System.Threading.Timer(
                async _ => await PerformPingAsync(),
                null,
                0,
                _pingInterval
            );

            _trafficMonitor.Start();
        }

        public void Stop()
        {
            _pingTimer?.Dispose();
            _pingTimer = null;
            _trafficMonitor.Stop();
        }

        public void UpdateInterval(int intervalSeconds)
        {
            _pingInterval = intervalSeconds * 1000;
            if (_pingTimer != null)
            {
                Stop();
                Start();
            }
        }

        private void OnTrafficUpdated(object? sender, (double downloadSpeed, double uploadSpeed) traffic)
        {
            _currentMetrics.DownloadSpeed = traffic.downloadSpeed;
            _currentMetrics.UploadSpeed = traffic.uploadSpeed;
        }

        private async Task PerformPingAsync()
        {
            try
            {
                using var pinger = new Ping();
                var reply = await pinger.SendPingAsync(_pingTarget, 5000);

                var metrics = new NetworkMetrics
                {
                    Timestamp = DateTime.Now,
                    PingSuccess = reply.Status == IPStatus.Success,
                    PingLatency = reply.RoundtripTime,
                    DownloadSpeed = _currentMetrics.DownloadSpeed,
                    UploadSpeed = _currentMetrics.UploadSpeed
                };

                MetricsUpdated?.Invoke(this, metrics);
            }
            catch
            {
                var metrics = new NetworkMetrics
                {
                    Timestamp = DateTime.Now,
                    PingSuccess = false,
                    PingLatency = -1,
                    DownloadSpeed = _currentMetrics.DownloadSpeed,
                    UploadSpeed = _currentMetrics.UploadSpeed
                };

                MetricsUpdated?.Invoke(this, metrics);
            }
        }

        public async Task<NetworkMetrics> GetCurrentMetricsAsync()
        {
            try
            {
                using var pinger = new Ping();
                var reply = await pinger.SendPingAsync(_pingTarget, 5000);

                return new NetworkMetrics
                {
                    Timestamp = DateTime.Now,
                    PingSuccess = reply.Status == IPStatus.Success,
                    PingLatency = reply.RoundtripTime,
                    DownloadSpeed = _currentMetrics.DownloadSpeed,
                    UploadSpeed = _currentMetrics.UploadSpeed
                };
            }
            catch
            {
                return new NetworkMetrics
                {
                    Timestamp = DateTime.Now,
                    PingSuccess = false,
                    PingLatency = -1,
                    DownloadSpeed = _currentMetrics.DownloadSpeed,
                    UploadSpeed = _currentMetrics.UploadSpeed
                };
            }
        }
    }
}
