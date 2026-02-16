using System.Net.NetworkInformation;
using Connectivity_Tracker.Models;

namespace Connectivity_Tracker.Services
{
    public class NetworkMonitorService
    {
        private System.Threading.Timer? _pingTimer;
        private string _pingTarget;
        private int _pingInterval;
        private readonly TrafficMonitorService _trafficMonitor;
        private readonly LocationService _locationService;
        private NetworkMetrics _currentMetrics;
        private readonly Queue<bool> _recentPingResults;
        private const int MaxPingHistorySize = 20;

        public event EventHandler<NetworkMetrics>? MetricsUpdated;
        public event EventHandler<(double downloadSpeed, double uploadSpeed)>? TrafficUpdated;

        public NetworkMonitorService(string pingTarget = "8.8.8.8", int pingIntervalSeconds = 10)
        {
            _pingTarget = pingTarget;
            _pingInterval = pingIntervalSeconds * 1000;
            _trafficMonitor = new TrafficMonitorService();
            _locationService = new LocationService();
            _currentMetrics = new NetworkMetrics();
            _recentPingResults = new Queue<bool>(MaxPingHistorySize);

            _trafficMonitor.TrafficUpdated += OnTrafficSampled;
        }

        public async Task InitializeAsync()
        {
            await _locationService.InitializeAsync();
        }

        public LocationService LocationService => _locationService;

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

        public void UpdatePingTarget(string pingTarget)
        {
            if (!string.IsNullOrWhiteSpace(pingTarget))
            {
                _pingTarget = pingTarget;
            }
        }

        private void OnTrafficSampled(object? sender, (double downloadSpeed, double uploadSpeed) traffic)
        {
            _currentMetrics.DownloadSpeed = traffic.downloadSpeed;
            _currentMetrics.UploadSpeed = traffic.uploadSpeed;
            TrafficUpdated?.Invoke(this, traffic);
        }

        private void TrackPingResult(bool success)
        {
            if (_recentPingResults.Count >= MaxPingHistorySize)
            {
                _recentPingResults.Dequeue();
            }
            _recentPingResults.Enqueue(success);
        }

        private double CalculatePacketLossPercentage()
        {
            if (_recentPingResults.Count == 0)
                return 0;

            int failedPings = _recentPingResults.Count(result => !result);
            return (failedPings / (double)_recentPingResults.Count) * 100.0;
        }

        private async Task PerformPingAsync()
        {
            try
            {
                var currentPingTarget = _pingTarget;
                using var pinger = new Ping();
                var reply = await pinger.SendPingAsync(currentPingTarget, 5000);

                bool success = reply.Status == IPStatus.Success;
                TrackPingResult(success);

                var (latitude, longitude) = await _locationService.GetCurrentLocationAsync();

                var metrics = new NetworkMetrics
                {
                    Timestamp = DateTime.Now,
                    PingSuccess = success,
                    PingLatency = reply.RoundtripTime,
                    DownloadSpeed = _currentMetrics.DownloadSpeed,
                    UploadSpeed = _currentMetrics.UploadSpeed,
                    Latitude = latitude,
                    Longitude = longitude,
                    PacketLossPercentage = CalculatePacketLossPercentage()
                };

                MetricsUpdated?.Invoke(this, metrics);
            }
            catch
            {
                TrackPingResult(false);

                var (latitude, longitude) = await _locationService.GetCurrentLocationAsync();

                var metrics = new NetworkMetrics
                {
                    Timestamp = DateTime.Now,
                    PingSuccess = false,
                    PingLatency = -1,
                    DownloadSpeed = _currentMetrics.DownloadSpeed,
                    UploadSpeed = _currentMetrics.UploadSpeed,
                    Latitude = latitude,
                    Longitude = longitude,
                    PacketLossPercentage = CalculatePacketLossPercentage()
                };

                MetricsUpdated?.Invoke(this, metrics);
            }
        }

        public async Task<NetworkMetrics> GetCurrentMetricsAsync()
        {
            try
            {
                var currentPingTarget = _pingTarget;
                using var pinger = new Ping();
                var reply = await pinger.SendPingAsync(currentPingTarget, 5000);

                var (latitude, longitude) = await _locationService.GetCurrentLocationAsync();

                return new NetworkMetrics
                {
                    Timestamp = DateTime.Now,
                    PingSuccess = reply.Status == IPStatus.Success,
                    PingLatency = reply.RoundtripTime,
                    DownloadSpeed = _currentMetrics.DownloadSpeed,
                    UploadSpeed = _currentMetrics.UploadSpeed,
                    Latitude = latitude,
                    Longitude = longitude,
                    PacketLossPercentage = CalculatePacketLossPercentage()
                };
            }
            catch
            {
                var (latitude, longitude) = await _locationService.GetCurrentLocationAsync();

                return new NetworkMetrics
                {
                    Timestamp = DateTime.Now,
                    PingSuccess = false,
                    PingLatency = -1,
                    DownloadSpeed = _currentMetrics.DownloadSpeed,
                    UploadSpeed = _currentMetrics.UploadSpeed,
                    Latitude = latitude,
                    Longitude = longitude,
                    PacketLossPercentage = CalculatePacketLossPercentage()
                };
            }
        }
    }
}
