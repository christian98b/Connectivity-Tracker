using System.Net.NetworkInformation;

namespace Connectivity_Tracker.Services
{
    public class TrafficMonitorService
    {
        private System.Threading.Timer? _timer;
        private long _lastBytesSent;
        private long _lastBytesReceived;
        private DateTime _lastUpdate;
        private NetworkInterface? _activeInterface;

        public event EventHandler<(double downloadSpeed, double uploadSpeed)>? TrafficUpdated;

        public void Start(int intervalSeconds = 2)
        {
            _activeInterface = GetActiveNetworkInterface();
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

        private NetworkInterface? GetActiveNetworkInterface()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
                            ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                            ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel &&
                            ni.GetIPProperties().UnicastAddresses.Any(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork))
                .OrderByDescending(ni => ni.Speed)
                .ToList();

            return interfaces.FirstOrDefault();
        }

        private void InitializeCounters()
        {
            if (_activeInterface == null) return;

            var stats = _activeInterface.GetIPv4Statistics();
            _lastBytesSent = stats.BytesSent;
            _lastBytesReceived = stats.BytesReceived;
        }

        private void UpdateTraffic()
        {
            try
            {
                _activeInterface = GetActiveNetworkInterface();
                if (_activeInterface == null) return;

                var stats = _activeInterface.GetIPv4Statistics();
                var currentTime = DateTime.Now;

                var bytesSent = stats.BytesSent;
                var bytesReceived = stats.BytesReceived;

                var timeDiff = (currentTime - _lastUpdate).TotalSeconds;

                if (timeDiff > 0)
                {
                    var uploadSpeed = (bytesSent - _lastBytesSent) / timeDiff;
                    var downloadSpeed = (bytesReceived - _lastBytesReceived) / timeDiff;

                    _lastBytesSent = bytesSent;
                    _lastBytesReceived = bytesReceived;
                    _lastUpdate = currentTime;

                    TrafficUpdated?.Invoke(this, (downloadSpeed, uploadSpeed));
                }
            }
            catch
            {
                TrafficUpdated?.Invoke(this, (0, 0));
            }
        }
    }
}
