using System.Windows;
using System.Windows.Forms;
using Connectivity_Tracker.Models;

namespace Connectivity_Tracker.Services
{
    public class NotificationService
    {
        private readonly NotifyIcon _notifyIcon;
        private int _alertThreshold;
        private double _packetLossThreshold;
        private DateTime _lastAlertTime = DateTime.MinValue;
        private DateTime _lastPacketLossAlertTime = DateTime.MinValue;
        private readonly TimeSpan _alertCooldown = TimeSpan.FromMinutes(5);
        private bool _isConnectionPoor = false;
        private bool _isPacketLossHigh = false;

        public NotificationService(NotifyIcon notifyIcon, int alertThreshold = 200, double packetLossThreshold = 10.0)
        {
            _notifyIcon = notifyIcon;
            _alertThreshold = alertThreshold;
            _packetLossThreshold = packetLossThreshold;
        }

        public void UpdateThreshold(int alertThreshold)
        {
            _alertThreshold = alertThreshold;
        }

        public void UpdatePacketLossThreshold(double packetLossThreshold)
        {
            _packetLossThreshold = packetLossThreshold;
        }

        public void CheckAndNotify(NetworkMetrics metrics)
        {
            // Check ping/latency alerts
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

            // Check packet loss alerts
            bool shouldAlertPacketLoss = metrics.PacketLossPercentage > _packetLossThreshold;

            if (shouldAlertPacketLoss && !_isPacketLossHigh)
            {
                if ((DateTime.Now - _lastPacketLossAlertTime) > _alertCooldown)
                {
                    ShowPacketLossHighNotification(metrics);
                    _lastPacketLossAlertTime = DateTime.Now;
                }
                _isPacketLossHigh = true;
            }
            else if (!shouldAlertPacketLoss && _isPacketLossHigh)
            {
                ShowPacketLossNormalNotification(metrics);
                _isPacketLossHigh = false;
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

        private void ShowPacketLossHighNotification(NetworkMetrics metrics)
        {
            _notifyIcon.ShowBalloonTip(
                5000,
                "High Packet Loss Detected",
                $"Packet loss is {metrics.PacketLossPercentage:F1}% (Threshold: {_packetLossThreshold}%)",
                ToolTipIcon.Warning
            );
        }

        private void ShowPacketLossNormalNotification(NetworkMetrics metrics)
        {
            _notifyIcon.ShowBalloonTip(
                3000,
                "Packet Loss Normal",
                $"Packet loss is back to normal ({metrics.PacketLossPercentage:F1}%)",
                ToolTipIcon.Info
            );
        }

        public void ShowCustomNotification(string title, string message, ToolTipIcon icon = ToolTipIcon.Info)
        {
            _notifyIcon.ShowBalloonTip(3000, title, message, icon);
        }
    }
}
