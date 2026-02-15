using System.Globalization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Connectivity_Tracker.Models;

namespace Connectivity_Tracker.Services
{
    public sealed class TaskbarPingUpdate
    {
        public ImageSource? OverlayImage { get; init; }
        public string TrayTooltip { get; init; } = "Connectivity Tracker";
        public bool UseTrayFallback { get; init; }
    }

    public sealed class TaskbarPingOverlayService
    {
        private readonly TimeSpan _minimumOverlayUpdateInterval;
        private DateTime _lastOverlayUpdateUtc = DateTime.MinValue;
        private string _lastOverlayText = string.Empty;
        private ImageSource? _currentOverlay;

        public TaskbarPingOverlayService(TimeSpan? minimumOverlayUpdateInterval = null)
        {
            _minimumOverlayUpdateInterval = minimumOverlayUpdateInterval ?? TimeSpan.FromSeconds(2);
        }

        public TaskbarPingUpdate BuildUpdate(NetworkMetrics metrics, bool showPingInTaskbar, bool taskbarOverlaySupported)
        {
            if (!showPingInTaskbar)
            {
                ResetOverlayState();
                return new TaskbarPingUpdate
                {
                    TrayTooltip = "Connectivity Tracker",
                    UseTrayFallback = false,
                    OverlayImage = null
                };
            }

            var pingText = TaskbarPingOverlayFormatter.FormatPingText(metrics);
            var trayTooltip = TaskbarPingOverlayFormatter.FormatTrayTooltip(metrics);

            if (!taskbarOverlaySupported)
            {
                return new TaskbarPingUpdate
                {
                    TrayTooltip = trayTooltip,
                    UseTrayFallback = true,
                    OverlayImage = null
                };
            }

            var nowUtc = DateTime.UtcNow;
            if (TaskbarPingOverlayFormatter.ShouldRefreshOverlay(
                    nowUtc,
                    _lastOverlayUpdateUtc,
                    pingText,
                    _lastOverlayText,
                    _minimumOverlayUpdateInterval))
            {
                _currentOverlay = CreateOverlayImage(pingText, metrics.PingSuccess && metrics.PingLatency >= 0);
                _lastOverlayUpdateUtc = nowUtc;
                _lastOverlayText = pingText;
            }

            return new TaskbarPingUpdate
            {
                TrayTooltip = trayTooltip,
                UseTrayFallback = false,
                OverlayImage = _currentOverlay
            };
        }

        private void ResetOverlayState()
        {
            _currentOverlay = null;
            _lastOverlayText = string.Empty;
            _lastOverlayUpdateUtc = DateTime.MinValue;
        }

        private static ImageSource CreateOverlayImage(string pingText, bool pingSuccess)
        {
            var visual = new DrawingVisual();

            using (var drawingContext = visual.RenderOpen())
            {
                var backgroundBrush = pingSuccess
                    ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(56, 142, 60))
                    : new SolidColorBrush(System.Windows.Media.Color.FromRgb(198, 40, 40));

                drawingContext.DrawRoundedRectangle(backgroundBrush, null, new System.Windows.Rect(0, 0, 16, 16), 3, 3);

                var typeface = new Typeface(
                    new System.Windows.Media.FontFamily("Segoe UI"),
                    System.Windows.FontStyles.Normal,
                    System.Windows.FontWeights.Bold,
                    System.Windows.FontStretches.Normal);
                var formattedText = new FormattedText(
                    pingText,
                    CultureInfo.InvariantCulture,
                    System.Windows.FlowDirection.LeftToRight,
                    typeface,
                    8,
                    System.Windows.Media.Brushes.White,
                    1.0);

                var textX = (16 - formattedText.Width) / 2;
                var textY = (16 - formattedText.Height) / 2;
                drawingContext.DrawText(formattedText, new System.Windows.Point(textX, textY));
            }

            var bitmap = new RenderTargetBitmap(16, 16, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            bitmap.Freeze();

            return bitmap;
        }
    }

    public static class TaskbarPingOverlayFormatter
    {
        public static string FormatPingText(NetworkMetrics metrics)
        {
            if (!metrics.PingSuccess || metrics.PingLatency < 0)
            {
                return "X";
            }

            if (metrics.PingLatency > 999)
            {
                return "1K";
            }

            return metrics.PingLatency.ToString(CultureInfo.InvariantCulture);
        }

        public static string FormatTrayTooltip(NetworkMetrics metrics)
        {
            if (!metrics.PingSuccess || metrics.PingLatency < 0)
            {
                return TrimToNotifyIconLimit("Connectivity Tracker - Ping: unavailable");
            }

            return TrimToNotifyIconLimit($"Connectivity Tracker - Ping: {metrics.PingLatency} ms");
        }

        public static bool ShouldRefreshOverlay(
            DateTime nowUtc,
            DateTime lastUpdateUtc,
            string currentPingText,
            string previousPingText,
            TimeSpan minimumOverlayUpdateInterval)
        {
            if (lastUpdateUtc == DateTime.MinValue)
            {
                return true;
            }

            if (string.Equals(currentPingText, previousPingText, StringComparison.Ordinal))
            {
                return false;
            }

            return nowUtc - lastUpdateUtc >= minimumOverlayUpdateInterval;
        }

        private static string TrimToNotifyIconLimit(string text)
        {
            const int maxLength = 63;
            return text.Length <= maxLength ? text : text[..maxLength];
        }
    }
}