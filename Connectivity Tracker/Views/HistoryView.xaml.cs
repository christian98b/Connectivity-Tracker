using Connectivity_Tracker.Controllers;
using Connectivity_Tracker.Services;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;

namespace Connectivity_Tracker.Views
{
    public partial class HistoryView : System.Windows.Controls.UserControl
    {
        private readonly HistoryController _historyController;
        private readonly PlotModel _historyPlotModel;

        public PlotModel HistoryPlotModel => _historyPlotModel;

        public HistoryView(DatabaseRepository databaseRepository)
        {
            InitializeComponent();

            _historyController = new HistoryController(databaseRepository);
            _historyPlotModel = new PlotModel { Title = "Connectivity Metrics" };
            DataContext = this;

            EndDatePicker.SelectedDate = DateTime.Now.Date;
            StartDatePicker.SelectedDate = DateTime.Now.Date.AddDays(-1);

            LoadAndRenderData();
        }

        public void RefreshData()
        {
            LoadAndRenderData();
        }

        private void LoadAndRenderData()
        {
            if (!TryBuildDateRange(out var startDateTime, out var endDateTime))
            {
                return;
            }

            var metrics = _historyController.GetMetrics(startDateTime, endDateTime);
            var orderedMetrics = metrics.OrderBy(x => x.Timestamp).ToList();

            BuildPlotModel(orderedMetrics, startDateTime, endDateTime);
        }

        private void BuildPlotModel(List<Models.NetworkMetrics> metrics, DateTime startDateTime, DateTime endDateTime)
        {
            _historyPlotModel.Series.Clear();
            _historyPlotModel.Axes.Clear();
            _historyPlotModel.Legends.Clear();

            _historyPlotModel.Legends.Add(new Legend
            {
                LegendPosition = LegendPosition.TopRight,
                LegendPlacement = LegendPlacement.Outside,
                LegendOrientation = LegendOrientation.Vertical
            });

            var xAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Time",
                StringFormat = "MM-dd HH:mm",
                IntervalType = DateTimeIntervalType.Hours,
                MinorIntervalType = DateTimeIntervalType.Minutes,
                IsZoomEnabled = true,
                IsPanEnabled = true,
                Minimum = DateTimeAxis.ToDouble(startDateTime),
                Maximum = DateTimeAxis.ToDouble(endDateTime)
            };

            var latencyAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Ping Latency (ms)",
                Key = "LatencyAxis",
                IsZoomEnabled = true,
                IsPanEnabled = true,
                Minimum = 0
            };

            var trafficAxis = new LinearAxis
            {
                Position = AxisPosition.Right,
                Title = "Traffic (bytes/s)",
                Key = "TrafficAxis",
                IsZoomEnabled = true,
                IsPanEnabled = true,
                Minimum = 0
            };

            _historyPlotModel.Axes.Add(xAxis);
            _historyPlotModel.Axes.Add(latencyAxis);
            _historyPlotModel.Axes.Add(trafficAxis);

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
                StrokeThickness = 2,
                YAxisKey = "TrafficAxis"
            };

            var uploadSeries = new LineSeries
            {
                Title = "Upload",
                Color = OxyColor.Parse("#FF9800"),
                StrokeThickness = 2,
                YAxisKey = "TrafficAxis"
            };

            var maxPing = metrics.Where(m => m.PingSuccess).Select(m => m.PingLatency).DefaultIfEmpty(1).Max();
            var failureMarkerY = maxPing * 1.05;

            var failureSeries = new ScatterSeries
            {
                Title = "Connection Failures",
                MarkerType = MarkerType.Circle,
                MarkerFill = OxyColor.Parse("#F44336"),
                MarkerStroke = OxyColor.Parse("#F44336"),
                MarkerSize = 4,
                YAxisKey = "LatencyAxis"
            };

            foreach (var metric in metrics)
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

                downloadSeries.Points.Add(new DataPoint(x, metric.DownloadSpeed));
                uploadSeries.Points.Add(new DataPoint(x, metric.UploadSpeed));
            }

            _historyPlotModel.Series.Add(pingSeries);
            _historyPlotModel.Series.Add(downloadSeries);
            _historyPlotModel.Series.Add(uploadSeries);
            _historyPlotModel.Series.Add(failureSeries);

            _historyPlotModel.Subtitle = metrics.Count == 0
                ? "No data for selected range"
                : $"Showing {metrics.Count} measurements";

            _historyPlotModel.InvalidatePlot(true);
        }

        private bool TryBuildDateRange(out DateTime startDateTime, out DateTime endDateTime)
        {
            startDateTime = DateTime.MinValue;
            endDateTime = DateTime.MinValue;

            if (!StartDatePicker.SelectedDate.HasValue || !EndDatePicker.SelectedDate.HasValue)
            {
                return false;
            }

            if (!TryParseTime(StartTimeTextBox.Text, out var startTime) || !TryParseTime(EndTimeTextBox.Text, out var endTime))
            {
                return false;
            }

            startDateTime = StartDatePicker.SelectedDate.Value.Date.Add(startTime);
            endDateTime = EndDatePicker.SelectedDate.Value.Date.Add(endTime);

            return endDateTime >= startDateTime;
        }

        private static bool TryParseTime(string input, out TimeSpan time)
        {
            return TimeSpan.TryParseExact(
                input,
                @"hh\:mm",
                System.Globalization.CultureInfo.InvariantCulture,
                out time
            );
        }

        private void ApplyFilter(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadAndRenderData();
        }

        private void SetLast24Hours(object sender, System.Windows.RoutedEventArgs e)
        {
            var end = DateTime.Now;
            var start = end.AddHours(-24);

            StartDatePicker.SelectedDate = start.Date;
            EndDatePicker.SelectedDate = end.Date;
            StartTimeTextBox.Text = start.ToString("HH:mm");
            EndTimeTextBox.Text = end.ToString("HH:mm");

            LoadAndRenderData();
        }

        private void OnFilterChanged(object sender, EventArgs e)
        {
            // Intentionally empty; user applies filter explicitly via button.
        }
    }
}
