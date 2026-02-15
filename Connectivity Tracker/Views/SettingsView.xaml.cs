using Connectivity_Tracker.Services;
using Connectivity_Tracker.ViewModels;

namespace Connectivity_Tracker.Views
{
    public partial class SettingsView : System.Windows.Controls.UserControl
    {
        public SettingsView(SettingsService settingsService, DatabaseRepository databaseRepository)
        {
            InitializeComponent();
            DataContext = new SettingsViewModel(settingsService, databaseRepository);
        }
    }
}
