using Connectivity_Tracker.ViewModels;
using Connectivity_Tracker.Services;
using System;

namespace Connectivity_Tracker.Views
{
    public partial class DashboardView : System.Windows.Controls.UserControl, IDisposable
    {
        private readonly DashboardViewModel _viewModel;
        private readonly NetworkMonitorService _networkService;
        private string _currentContext = "Home";
        private bool _disposed = false;

        public DashboardView(NetworkMonitorService networkService)
        {
            InitializeComponent();

            _networkService = networkService;
            _viewModel = new DashboardViewModel();
            DataContext = _viewModel;

            _networkService.MetricsUpdated += OnMetricsUpdated;
            _networkService.TrafficUpdated += OnTrafficUpdated;
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
                _networkService.MetricsUpdated -= OnMetricsUpdated;
                _networkService.TrafficUpdated -= OnTrafficUpdated;
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
