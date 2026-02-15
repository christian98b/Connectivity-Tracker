using Connectivity_Tracker.Models;
using Connectivity_Tracker.Services;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Connectivity_Tracker.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private string _latency = "--";
        private string _downloadRate = "--";
        private string _uploadRate = "--";
        private System.Windows.Media.Brush _latencyColor = System.Windows.Media.Brushes.Gray;
        private string _connectionStatus = "Connecting...";
        private PlotModel _historyPlotModel;

        public DashboardViewModel()
        {
            _historyPlotModel = CreateHistoryPlotModel();
        }

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

        public PlotModel HistoryPlotModel
        {
            get => _historyPlotModel;
            set => SetProperty(ref _historyPlotModel, value);
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

        private PlotModel CreateHistoryPlotModel()
        {
            var plotModel = new PlotModel();

            var xAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "HH:mm",
                IntervalType = DateTimeIntervalType.Minutes,
                MinorIntervalType = DateTimeIntervalType.Minutes,
                IsZoomEnabled = false,
                IsPanEnabled = false
            };

            var latencyAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Ping (ms)",
                Key = "LatencyAxis",
                IsZoomEnabled = false,
                IsPanEnabled = false,
                Minimum = 0
            };

            var trafficAxis = new LinearAxis
            {
                Position = AxisPosition.Right,
                Title = "Traffic (KB/s)",
                Key = "TrafficAxis",
                IsZoomEnabled = false,
                IsPanEnabled = false,
                Minimum = 0
            };

            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(latencyAxis);
            plotModel.Axes.Add(trafficAxis);

            return plotModel;
        }

        public void UpdateHistoryChart(List<NetworkMetrics> recentMetrics)
        {
            _historyPlotModel.Series.Clear();

            var pingSeries = new LineSeries
            {
                Title = "Ping",
                Color = OxyColor.Parse("#2196F3"),
                StrokeThickness = 2,
                YAxisKey = "LatencyAxis"
            };

            var downloadSeries = new LineSeries
            {
                Title = "Download",
                Color = OxyColor.Parse("#4CAF50"),
                StrokeThickness = 1.5,
                YAxisKey = "TrafficAxis"
            };

            var uploadSeries = new LineSeries
            {
                Title = "Upload",
                Color = OxyColor.Parse("#FF9800"),
                StrokeThickness = 1.5,
                YAxisKey = "TrafficAxis"
            };

            var maxPing = recentMetrics.Where(m => m.PingSuccess).Select(m => m.PingLatency).DefaultIfEmpty(1).Max();
            var failureMarkerY = maxPing * 1.05;

            var failureSeries = new ScatterSeries
            {
                Title = "Failures",
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColor.Parse("#F44336"),
                MarkerStroke = OxyColor.Parse("#F44336"),
                MarkerSize = 3,
                YAxisKey = "LatencyAxis"
            };

            foreach (var metric in recentMetrics)
            {
                var x = DateTimeAxis.ToDouble(metric.Timestamp);

                if (metric.PingSuccess)
                {
                    pingSeries.Points.Add(new DataPoint(x, metric.PingLatency));
                }
                else
                {
                    failureSeries.Points.Add(new ScatterPoint(x, failureMarkerY));
                }

                downloadSeries.Points.Add(new DataPoint(x, metric.DownloadSpeed / 1024.0));
                uploadSeries.Points.Add(new DataPoint(x, metric.UploadSpeed / 1024.0));
            }

            _historyPlotModel.Series.Add(pingSeries);
            _historyPlotModel.Series.Add(downloadSeries);
            _historyPlotModel.Series.Add(uploadSeries);
            if (failureSeries.Points.Count > 0)
            {
                _historyPlotModel.Series.Add(failureSeries);
            }

            _historyPlotModel.InvalidatePlot(true);
        }
    }
}
