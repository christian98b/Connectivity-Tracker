using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Connectivity_Tracker.Views;
using Connectivity_Tracker.Services;
using System.Drawing;
using System.Windows.Forms;

namespace Connectivity_Tracker
{
    public partial class MainWindow : Window
    {
        private NotifyIcon? _notifyIcon;
        private readonly NetworkMonitorService _networkService;
        private readonly NotificationService _notificationService;
        private readonly DatabaseRepository _databaseRepository;

        public MainWindow()
        {
            InitializeComponent();
            InitializeSystemTray();

            _databaseRepository = new DatabaseRepository();
            _notificationService = new NotificationService(_notifyIcon!, 200);
            _networkService = new NetworkMonitorService();
            _networkService.MetricsUpdated += OnMetricsUpdated;
            _networkService.Start();

            NavigateToDashboard(null, null);
        }

        private async void OnMetricsUpdated(object? sender, Models.NetworkMetrics metrics)
        {
            _notificationService.CheckAndNotify(metrics);
            await _databaseRepository.SaveMetricsAsync(metrics);
        }

        private void InitializeSystemTray()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Text = "Connectivity Tracker",
                Visible = true
            };

            _notifyIcon.DoubleClick += (sender, args) =>
            {
                Show();
                WindowState = WindowState.Normal;
                Activate();
            };

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Show", null, (sender, args) =>
            {
                Show();
                WindowState = WindowState.Normal;
                Activate();
            });
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Exit", null, (sender, args) =>
            {
                _notifyIcon.Visible = false;
                System.Windows.Application.Current.Shutdown();
            });

            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            _networkService?.Stop();
            _notifyIcon?.Dispose();
            base.OnClosed(e);
        }

        private void NavigateToDashboard(object? sender, RoutedEventArgs? e)
        {
            MainContentArea.Content = new DashboardView(_networkService);
            UpdateNavigationButtonState(DashboardButton);
        }

        private void NavigateToHistory(object? sender, RoutedEventArgs? e)
        {
            MainContentArea.Content = new HistoryView();
            UpdateNavigationButtonState(HistoryButton);
        }

        private void NavigateToSettings(object? sender, RoutedEventArgs? e)
        {
            MainContentArea.Content = new SettingsView();
            UpdateNavigationButtonState(SettingsButton);
        }

        private void UpdateNavigationButtonState(System.Windows.Controls.Button activeButton)
        {
            DashboardButton.Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#757575"));
            HistoryButton.Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#757575"));
            SettingsButton.Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#757575"));

            activeButton.Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2196F3"));
        }
    }
}