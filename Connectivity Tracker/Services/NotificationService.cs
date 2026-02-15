using System.Windows;
using System.Windows.Forms;
using Connectivity_Tracker.Models;

namespace Connectivity_Tracker.Services
{
    public class NotificationService
    {
        private readonly NotifyIcon _notifyIcon;
        private int _alertThreshold;
        private DateTime _lastAlertTime = DateTime.MinValue;
        private readonly TimeSpan _alertCooldown = TimeSpan.FromMinutes(5);
        private bool _isConnectionPoor = false;

        public NotificationService(NotifyIcon notifyIcon, int alertThreshold = 200)
        {
            _notifyIcon = notifyIcon;
            _alertThreshold = alertThreshold;
        }

        public void UpdateThreshold(int alertThreshold)
        {
            _alertThreshold = alertThreshold;
        }

        public void CheckAndNotify(NetworkMetrics metrics)
        {
            bool shouldAlert = !metrics.PingSuccess || metrics.PingLatency > _alertThreshold;

            if (shouldAlert && !_isConnectionPoor)
            {
                if ((DateTime.Now - _lastAlertTime) > _alertCooldown)
                {
                    ShowConnectionPoorNotification(metrics);
                    _lastAlertTime = DateTime.Now;
                }
                _isConnectionPoor = true;
            }
            else if (!shouldAlert && _isConnectionPoor)
            {
                ShowConnectionRestoredNotification(metrics);
                _isConnectionPoor = false;
            }
        }

        private void ShowConnectionPoorNotification(NetworkMetrics metrics)
        {
            string message = metrics.PingSuccess
                ? $"High latency detected: {metrics.PingLatency}ms"
                : "Connection lost!";

            _notifyIcon.ShowBalloonTip(
                5000,
                "Connection Issue",
                message,
                ToolTipIcon.Warning
            );
        }

        private void ShowConnectionRestoredNotification(NetworkMetrics metrics)
        {
            _notifyIcon.ShowBalloonTip(
                3000,
                "Connection Restored",
                $"Connection is back to normal ({metrics.PingLatency}ms)",
                ToolTipIcon.Info
            );
        }

        public void ShowCustomNotification(string title, string message, ToolTipIcon icon = ToolTipIcon.Info)
        {
            _notifyIcon.ShowBalloonTip(3000, title, message, icon);
        }
    }
}
