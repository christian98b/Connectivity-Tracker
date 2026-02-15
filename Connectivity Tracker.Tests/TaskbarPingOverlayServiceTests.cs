using Connectivity_Tracker.Models;
using Connectivity_Tracker.Services;
using Xunit;

namespace Connectivity_Tracker.Tests;

public class TaskbarPingOverlayServiceTests
{
    [Fact]
    public void FormatPingText_ReturnsLatency_WhenPingSuccessful()
    {
        var metrics = new NetworkMetrics { PingSuccess = true, PingLatency = 42 };

        var result = TaskbarPingOverlayFormatter.FormatPingText(metrics);

        Assert.Equal("42", result);
    }

    [Fact]
    public void FormatPingText_ReturnsX_WhenPingFailed()
    {
        var metrics = new NetworkMetrics { PingSuccess = false, PingLatency = -1 };

        var result = TaskbarPingOverlayFormatter.FormatPingText(metrics);

        Assert.Equal("X", result);
    }

    [Fact]
    public void ShouldRefreshOverlay_ReturnsFalse_WhenTextIsUnchanged()
    {
        var now = DateTime.UtcNow;
        var last = now.AddSeconds(-10);

        var shouldRefresh = TaskbarPingOverlayFormatter.ShouldRefreshOverlay(
            now,
            last,
            "42",
            "42",
            TimeSpan.FromSeconds(2));

        Assert.False(shouldRefresh);
    }

    [Fact]
    public void ShouldRefreshOverlay_ReturnsFalse_WhenChangedButInsideThrottleWindow()
    {
        var now = DateTime.UtcNow;
        var last = now.AddMilliseconds(-500);

        var shouldRefresh = TaskbarPingOverlayFormatter.ShouldRefreshOverlay(
            now,
            last,
            "43",
            "42",
            TimeSpan.FromSeconds(2));

        Assert.False(shouldRefresh);
    }

    [Fact]
    public void BuildUpdate_UsesTrayFallback_WhenTaskbarOverlayUnsupported()
    {
        var service = new TaskbarPingOverlayService(TimeSpan.FromSeconds(2));
        var metrics = new NetworkMetrics { PingSuccess = true, PingLatency = 25 };

        var update = service.BuildUpdate(metrics, true, false);

        Assert.True(update.UseTrayFallback);
        Assert.Null(update.OverlayImage);
        Assert.Contains("25 ms", update.TrayTooltip, StringComparison.Ordinal);
    }

    [Fact]
    public void BuildUpdate_ReturnsBaseTooltip_WhenFeatureDisabled()
    {
        var service = new TaskbarPingOverlayService(TimeSpan.FromSeconds(2));
        var metrics = new NetworkMetrics { PingSuccess = true, PingLatency = 25 };

        var update = service.BuildUpdate(metrics, false, true);

        Assert.False(update.UseTrayFallback);
        Assert.Null(update.OverlayImage);
        Assert.Equal("Connectivity Tracker", update.TrayTooltip);
    }
}