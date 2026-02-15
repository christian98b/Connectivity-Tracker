using Connectivity_Tracker.ViewModels;
using Connectivity_Tracker.Services;
using System;
using System.Windows.Threading;

namespace Connectivity_Tracker.Views
{
    public partial class DashboardView : System.Windows.Controls.UserControl, IDisposable
    {
        private readonly DashboardViewModel _viewModel;
        private readonly NetworkMonitorService _networkService;
        private readonly DatabaseRepository _databaseRepository;
        private readonly DispatcherTimer _historyUpdateTimer;
        private string _currentContext = "Home";
        private bool _disposed = false;

        public DashboardView(NetworkMonitorService networkService, DatabaseRepository databaseRepository)
        {
            InitializeComponent();

            _networkService = networkService;
            _databaseRepository = databaseRepository;
            _viewModel = new DashboardViewModel();
            DataContext = _viewModel;

            _networkService.MetricsUpdated += OnMetricsUpdated;
            _networkService.TrafficUpdated += OnTrafficUpdated;

            // Update history chart every 30 seconds
            _historyUpdateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(30)
            };
            _historyUpdateTimer.Tick += (s, e) => LoadRecentHistory();
            _historyUpdateTimer.Start();

            LoadRecentHistory();
        }

        private void LoadRecentHistory()
        {
            var endTime = DateTime.Now;
            var startTime = endTime.AddHours(-1);

            var recentMetrics = _databaseRepository.GetMetrics(startTime, endTime, 500);
            var orderedMetrics = recentMetrics.OrderBy(m => m.Timestamp).ToList();

            _viewModel.UpdateHistoryChart(orderedMetrics);
        }

        private void OnMetricsUpdated(object? sender, Models.NetworkMetrics metrics)
        {
            metrics.Context = _currentContext;
            Dispatcher.Invoke(() =>
            {
                _viewModel.UpdateMetrics(metrics);
            });
        }

        private void OnTrafficUpdated(object? sender, (double downloadSpeed, double uploadSpeed) traffic)
        {
            Dispatcher.Invoke(() =>
            {
                _viewModel.UpdateTraffic(traffic.downloadSpeed, traffic.uploadSpeed);
            });
        }

        private void ContextComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ContextComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem item)
            {
                _currentContext = item.Tag?.ToString() ?? "Home";
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _historyUpdateTimer?.Stop();
                _networkService.MetricsUpdated -= OnMetricsUpdated;
                _networkService.TrafficUpdated -= OnTrafficUpdated;
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
