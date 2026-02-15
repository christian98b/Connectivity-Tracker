using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Connectivity_Tracker.Views;
using Connectivity_Tracker.Services;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Shell;

namespace Connectivity_Tracker
{
    public partial class MainWindow : Window
    {
        private NotifyIcon? _notifyIcon;
        private readonly NetworkMonitorService _networkService;
        private NotificationService _notificationService;
        private readonly DatabaseRepository _databaseRepository;
        private readonly SettingsService _settingsService;
        private readonly TaskbarPingOverlayService _taskbarPingOverlayService;
        private bool _showPingInTaskbar;
        private bool _showPingInTray;
        private bool _taskbarOverlaySupported = true;
        private DateTime _lastTrayIconUpdate = DateTime.MinValue;
        private readonly TimeSpan _trayIconUpdateThrottle = TimeSpan.FromSeconds(2);

        // Cached views to prevent memory leaks from event handler subscriptions
        private DashboardView? _dashboardView;
        private HistoryView? _historyView;
        private SettingsView? _settingsView;

        public MainWindow()
        {
            InitializeComponent();
            InitializeSystemTray();

            _settingsService = new SettingsService();
            _databaseRepository = new DatabaseRepository();
            _taskbarPingOverlayService = new TaskbarPingOverlayService(TimeSpan.FromSeconds(2));

            var settings = _settingsService.CurrentSettings;
            _showPingInTaskbar = settings.ShowPingInTaskbar;
            _showPingInTray = settings.ShowPingInTray;
            _notificationService = new NotificationService(_notifyIcon!, settings.AlertThresholdMs);
            _networkService = new NetworkMonitorService(settings.PingTarget, settings.PingIntervalSeconds);
            _networkService.MetricsUpdated += OnMetricsUpdated;

            _settingsService.SettingsChanged += OnSettingsChanged;

            InitializeServicesAsync();

            NavigateToDashboard(null, null);
        }

        private void OnSettingsChanged(object? sender, EventArgs e)
        {
            var settings = _settingsService.CurrentSettings;
            _notificationService.UpdateThreshold(settings.AlertThresholdMs);
            _networkService.UpdateInterval(settings.PingIntervalSeconds);
            _networkService.UpdatePingTarget(settings.PingTarget);
            _showPingInTaskbar = settings.ShowPingInTaskbar;
            _showPingInTray = settings.ShowPingInTray;

            if (!_showPingInTaskbar)
            {
                TaskbarItemInfo ??= new TaskbarItemInfo();
                TaskbarItemInfo.Overlay = null;

                if (_notifyIcon is not null)
                {
                    _notifyIcon.Text = "Connectivity Tracker";
                }
            }

            // Reset tray icon to default if ShowPingInTray is disabled
            if (!_showPingInTray && _notifyIcon is not null)
            {
                _notifyIcon.Icon = SystemIcons.Application;
            }
        }

        private async void InitializeServicesAsync()
        {
            await _networkService.InitializeAsync();
            _networkService.Start();
        }

        private async void OnMetricsUpdated(object? sender, Models.NetworkMetrics metrics)
        {
            await Dispatcher.InvokeAsync(() => UpdateTaskbarPing(metrics));
            _notificationService.CheckAndNotify(metrics);
            await _databaseRepository.SaveMetricsAsync(metrics);
        }

        private void UpdateTaskbarPing(Models.NetworkMetrics metrics)
        {
            if (_notifyIcon is null)
            {
                return;
            }

            var update = _taskbarPingOverlayService.BuildUpdate(metrics, _showPingInTaskbar, _taskbarOverlaySupported);
            _notifyIcon.Text = update.TrayTooltip;

            // Update tray icon with ping value if enabled and throttling allows
            if (_showPingInTray)
            {
                var now = DateTime.UtcNow;
                if ((now - _lastTrayIconUpdate) >= _trayIconUpdateThrottle)
                {
                    try
                    {
                        var pingMs = metrics.PingSuccess ? (int?)metrics.PingLatency : null;
                        var newIcon = TrayIconGenerator.GeneratePingIcon(pingMs);

                        // Dispose old icon before setting new one to prevent memory leaks
                        var oldIcon = _notifyIcon.Icon;
                        _notifyIcon.Icon = newIcon;

                        if (oldIcon != SystemIcons.Application)
                        {
                            oldIcon?.Dispose();
                        }

                        _lastTrayIconUpdate = now;
                    }
                    catch
                    {
                        // If icon generation fails, fall back to default icon
                        _notifyIcon.Icon = SystemIcons.Application;
                    }
                }
            }

            TaskbarItemInfo ??= new TaskbarItemInfo();

            if (!_showPingInTaskbar || update.UseTrayFallback)
            {
                TaskbarItemInfo.Overlay = null;
                return;
            }

            if (update.OverlayImage is not null)
            {
                try
                {
                    TaskbarItemInfo.Overlay = update.OverlayImage;
                }
                catch
                {
                    _taskbarOverlaySupported = false;
                    TaskbarItemInfo.Overlay = null;

                    var fallbackUpdate = _taskbarPingOverlayService.BuildUpdate(metrics, _showPingInTaskbar, false);
                    _notifyIcon.Text = fallbackUpdate.TrayTooltip;
                }
            }
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

            // Dispose cached views to prevent memory leaks from event handler subscriptions
            _dashboardView?.Dispose();

            _notifyIcon?.Dispose();
            base.OnClosed(e);
        }

        private void NavigateToDashboard(object? sender, RoutedEventArgs? e)
        {
            _dashboardView ??= new DashboardView(_networkService, _databaseRepository);
            MainContentArea.Content = _dashboardView;
            UpdateNavigationButtonState(DashboardButton);
        }

        private void NavigateToHistory(object? sender, RoutedEventArgs? e)
        {
            _historyView ??= new HistoryView(_databaseRepository);
            _historyView.RefreshData();
            MainContentArea.Content = _historyView;
            UpdateNavigationButtonState(HistoryButton);
        }

        private void NavigateToSettings(object? sender, RoutedEventArgs? e)
        {
            _settingsView ??= new SettingsView(_settingsService, _databaseRepository);
            MainContentArea.Content = _settingsView;
            UpdateNavigationButtonState(SettingsButton);
        }

        private void UpdateNavigationButtonState(System.Windows.Controls.Button activeButton)
        {
            var inactiveColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#757575");
            var activeColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2196F3");

            // Reset all buttons to inactive state
            DashboardButton.SetValue(System.Windows.Controls.Button.BackgroundProperty, new SolidColorBrush(inactiveColor));
            HistoryButton.SetValue(System.Windows.Controls.Button.BackgroundProperty, new SolidColorBrush(inactiveColor));
            SettingsButton.SetValue(System.Windows.Controls.Button.BackgroundProperty, new SolidColorBrush(inactiveColor));

            // Set active button
            activeButton.SetValue(System.Windows.Controls.Button.BackgroundProperty, new SolidColorBrush(activeColor));
        }
    }
}