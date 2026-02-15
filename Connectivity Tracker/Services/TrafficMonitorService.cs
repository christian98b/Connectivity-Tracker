using System.Net.NetworkInformation;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Connectivity_Tracker.Services
{
    public class TrafficMonitorService
    {
        private System.Threading.Timer? _timer;
        private Dictionary<string, (long lastBytesSent, long lastBytesReceived)> _interfaceStats = new Dictionary<string, (long, long)>();
        private DateTime _lastUpdate;

        public event EventHandler<(double downloadSpeed, double uploadSpeed)>? TrafficUpdated;

        public void Start(int intervalSeconds = 2)
        {
            _lastUpdate = DateTime.Now;
            InitializeCounters();

            _timer = new System.Threading.Timer(
                _ => UpdateTraffic(),
                null,
                intervalSeconds * 1000,
                intervalSeconds * 1000
            );
        }

        public void Stop()
        {
            _timer?.Dispose();
            _timer = null;
        }

        private IEnumerable<NetworkInterface> GetAllRelevantInterfaces()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
                            ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                            ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel);
        }

        private void InitializeCounters()
        {
            _interfaceStats.Clear();
            foreach (var ni in GetAllRelevantInterfaces())
            {
                var stats = ni.GetIPStatistics();
                _interfaceStats[ni.Id] = (stats.BytesSent, stats.BytesReceived);
            }
        }

        private void UpdateTraffic()
        {
            try
            {
                var currentTime = DateTime.Now;
                var timeDiff = (currentTime - _lastUpdate).TotalSeconds;

                if (timeDiff <= 0) return;

                long totalBytesSent = 0;
                long totalBytesReceived = 0;

                var currentInterfaces = GetAllRelevantInterfaces().ToList();
                var currentInterfaceIds = new HashSet<string>(currentInterfaces.Select(ni => ni.Id));

                // Remove interfaces that are no longer relevant
                var keysToRemove = _interfaceStats.Keys.Where(id => !currentInterfaceIds.Contains(id)).ToList();
                foreach (var key in keysToRemove)
                {
                    _interfaceStats.Remove(key);
                }

                foreach (var ni in currentInterfaces)
                {
                    var stats = ni.GetIPStatistics();
                    totalBytesSent += stats.BytesSent;
                    totalBytesReceived += stats.BytesReceived;

                    if (!_interfaceStats.ContainsKey(ni.Id))
                    {
                        _interfaceStats[ni.Id] = (stats.BytesSent, stats.BytesReceived);
                    }
                }

                long lastTotalBytesSent = _interfaceStats.Values.Sum(s => s.lastBytesSent);
                long lastTotalBytesReceived = _interfaceStats.Values.Sum(s => s.lastBytesReceived);

                var uploadSpeed = (totalBytesSent - lastTotalBytesSent) / timeDiff;
                var downloadSpeed = (totalBytesReceived - lastTotalBytesReceived) / timeDiff;

                // Update stats for next run
                foreach (var ni in currentInterfaces)
                {
                    var stats = ni.GetIPStatistics();
                    _interfaceStats[ni.Id] = (stats.BytesSent, stats.BytesReceived);
                }

                _lastUpdate = currentTime;

                TrafficUpdated?.Invoke(this, (downloadSpeed, uploadSpeed));
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Error updating traffic: {ex.Message}");
                TrafficUpdated?.Invoke(this, (0, 0));
            }
        }
    }
}
