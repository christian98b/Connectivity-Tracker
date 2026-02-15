using System.Collections.ObjectModel;
using System.Windows.Input;
using Connectivity_Tracker.Models;
using Connectivity_Tracker.Services;

namespace Connectivity_Tracker.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly SettingsService _settingsService;
        private readonly DatabaseRepository _databaseRepository;

        private int _selectedPingIntervalIndex;
        private string _alertThreshold;
        private bool _startWithWindows;
        private bool _minimizeToTrayOnStartup;

        public ObservableCollection<string> PingIntervalOptions { get; }

        public int SelectedPingIntervalIndex
        {
            get => _selectedPingIntervalIndex;
            set => SetProperty(ref _selectedPingIntervalIndex, value);
        }

        public string AlertThreshold
        {
            get => _alertThreshold;
            set => SetProperty(ref _alertThreshold, value);
        }

        public bool StartWithWindows
        {
            get => _startWithWindows;
            set => SetProperty(ref _startWithWindows, value);
        }

        public bool MinimizeToTrayOnStartup
        {
            get => _minimizeToTrayOnStartup;
            set => SetProperty(ref _minimizeToTrayOnStartup, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand ExportDataCommand { get; }

        public SettingsViewModel(SettingsService settingsService, DatabaseRepository databaseRepository)
        {
            _settingsService = settingsService;
            _databaseRepository = databaseRepository;

            PingIntervalOptions = new ObservableCollection<string>
            {
                "5 seconds",
                "10 seconds",
                "60 seconds"
            };

            SaveCommand = new RelayCommand(SaveSettings);
            ExportDataCommand = new RelayCommand(ExportData);

            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = _settingsService.CurrentSettings;

            _selectedPingIntervalIndex = settings.PingIntervalSeconds switch
            {
                5 => 0,
                10 => 1,
                60 => 2,
                _ => 1
            };

            _alertThreshold = settings.AlertThresholdMs.ToString();
            _startWithWindows = settings.StartWithWindows;
            _minimizeToTrayOnStartup = settings.MinimizeToTrayOnStartup;
        }

        private void SaveSettings()
        {
            if (!int.TryParse(_alertThreshold, out int threshold) || threshold < 0)
            {
                System.Windows.MessageBox.Show(
                    "Please enter a valid number for alert threshold.",
                    "Invalid Input",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Warning
                );
                return;
            }

            int pingInterval = _selectedPingIntervalIndex switch
            {
                0 => 5,
                1 => 10,
                2 => 60,
                _ => 10
            };

            var settings = new AppSettings
            {
                PingIntervalSeconds = pingInterval,
                PingTarget = _settingsService.CurrentSettings.PingTarget,
                AlertThresholdMs = threshold,
                StartWithWindows = _startWithWindows,
                MinimizeToTrayOnStartup = _minimizeToTrayOnStartup
            };

            _settingsService.UpdateSettings(settings);

            System.Windows.MessageBox.Show(
                "Settings saved successfully! Some changes may require restarting the application.",
                "Settings Saved",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information
            );
        }

        private void ExportData()
        {
            try
            {
                var metrics = _databaseRepository.ExportAllMetrics();

                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv",
                    FileName = $"ConnectivityData_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (dialog.ShowDialog() == true)
                {
                    var lines = new List<string>
                    {
                        "Timestamp,PingSuccess,PingLatency,DownloadSpeed,UploadSpeed,Latitude,Longitude,Context"
                    };

                    foreach (var metric in metrics)
                    {
                        lines.Add($"{metric.Timestamp:O},{metric.PingSuccess},{metric.PingLatency}," +
                                 $"{metric.DownloadSpeed},{metric.UploadSpeed}," +
                                 $"{metric.Latitude?.ToString() ?? ""}," +
                                 $"{metric.Longitude?.ToString() ?? ""}," +
                                 $"\"{metric.Context}\"");
                    }

                    System.IO.File.WriteAllLines(dialog.FileName, lines);

                    System.Windows.MessageBox.Show(
                        $"Data exported successfully to:\n{dialog.FileName}",
                        "Export Complete",
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Information
                    );
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Failed to export data: {ex.Message}",
                    "Export Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error
                );
            }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => _execute();
    }
}
