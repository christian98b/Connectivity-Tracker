using Connectivity_Tracker.Models;
using Connectivity_Tracker.Services;

namespace Connectivity_Tracker.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private string _latency = "--";
        private string _downloadRate = "--";
        private string _uploadRate = "--";
        private System.Windows.Media.Brush _latencyColor = System.Windows.Media.Brushes.Gray;
        private string _connectionStatus = "Connecting...";

        public string Latency
        {
            get => _latency;
            set => SetProperty(ref _latency, value);
        }

        public string DownloadRate
        {
            get => _downloadRate;
            set => SetProperty(ref _downloadRate, value);
        }

        public string UploadRate
        {
            get => _uploadRate;
            set => SetProperty(ref _uploadRate, value);
        }

        public System.Windows.Media.Brush LatencyColor
        {
            get => _latencyColor;
            set => SetProperty(ref _latencyColor, value);
        }

        public string ConnectionStatus
        {
            get => _connectionStatus;
            set => SetProperty(ref _connectionStatus, value);
        }

        public void UpdateMetrics(NetworkMetrics metrics)
        {
            if (metrics.PingSuccess)
            {
                Latency = $"{metrics.PingLatency} ms";
                ConnectionStatus = "Connected";

                LatencyColor = metrics.PingLatency switch
                {
                    < 50 => new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#4CAF50")),
                    < 100 => new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2196F3")),
                    < 200 => new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF9800")),
                    _ => new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F44336"))
                };
            }
            else
            {
                Latency = "Failed";
                ConnectionStatus = "Disconnected";
                LatencyColor = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F44336"));
            }

            UpdateTraffic(metrics.DownloadSpeed, metrics.UploadSpeed);
        }

        public void UpdateTraffic(double downloadSpeed, double uploadSpeed)
        {
            DownloadRate = FormatSpeed(Math.Max(0, downloadSpeed));
            UploadRate = FormatSpeed(Math.Max(0, uploadSpeed));
        }

        private string FormatSpeed(double bytesPerSecond)
        {
            if (bytesPerSecond < 1024)
                return $"{bytesPerSecond:F2} B/s";
            if (bytesPerSecond < 1024 * 1024)
                return $"{bytesPerSecond / 1024:F2} KB/s";
            return $"{bytesPerSecond / (1024 * 1024):F2} MB/s";
        }
    }
}
