using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using TheXDS.Ganymede.Helpers;
using TheXDS.MCART.Math;
using static TheXDS.MCART.Helpers.DependencyObjectHelpers;

namespace TheXDS.Vivianne.Controls;

/// <summary>
/// Implements a control that displays basic information on a running process.
/// </summary>
public class ProcessMonitor : Control
{
    private static readonly DependencyPropertyKey IsProcessLockedPropertyKey;
    private static readonly DependencyPropertyKey RamUsagePropertyKey;
    private static readonly DependencyPropertyKey CpuUsagePropertyKey;
    private static readonly DependencyPropertyKey ProcessNamePropertyKey;

    /// <summary>
    /// Identifies the <see cref="MonitoredProcess"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MonitoredProcessProperty;

    /// <summary>
    /// Identifies the <see cref="IsProcessLocked"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsProcessLockedProperty;

    /// <summary>
    /// Identifies the <see cref="RamUsage"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RamUsageProperty;

    /// <summary>
    /// Identifies the <see cref="CpuUsage"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CpuUsageProperty;

    /// <summary>
    /// Identifies the <see cref="ProcessName"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ProcessNameProperty;

    /// <summary>
    /// Identifies the <see cref="RefreshInterval"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RefreshIntervalProperty;

    /// <summary>
    /// Identifies the <see cref="NotRespondingContent"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NotRespondingContentProperty;


    /// <summary>
    /// Initializes the <see cref="ProcessMonitor"/> class.
    /// </summary>
    static ProcessMonitor()
    {
        SetControlStyle<ProcessMonitor>(DefaultStyleKeyProperty);
        (IsProcessLockedPropertyKey, IsProcessLockedProperty) = NewDpRo<bool, ProcessMonitor>(nameof(IsProcessLocked));
        (RamUsagePropertyKey, RamUsageProperty) = NewDpRo<long, ProcessMonitor>(nameof(RamUsage));
        (CpuUsagePropertyKey, CpuUsageProperty) = NewDpRo<double, ProcessMonitor>(nameof(CpuUsage));
        (ProcessNamePropertyKey, ProcessNameProperty) = NewDpRo<string?, ProcessMonitor>(nameof(ProcessName));
        MonitoredProcessProperty = NewDp<Process, ProcessMonitor>(nameof(MonitoredProcess), changedValue: OnMonitoredProcessChanged);
        RefreshIntervalProperty = NewDp<int, ProcessMonitor>(nameof(RefreshInterval), 1000, OnIntervalChanged, CoerceInterval);
        NotRespondingContentProperty = NewDp<object?, ProcessMonitor>(nameof(NotRespondingContent), FrameworkPropertyMetadataOptions.AffectsArrange);
    }

    private static object CoerceInterval(DependencyObject d, object baseValue)
    {
        return ((int)baseValue).Clamp(100, int.MaxValue);
    }

    private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ProcessMonitor { _timer: { } timer } || e.NewValue is not int interval) return;
        UiThread.Invoke(() => timer.Change(0, interval));
    }

    private static void OnMonitoredProcessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ProcessMonitor pm) return;
        (e.OldValue as Timer)?.Dispose();
        pm._timer = e.NewValue switch
        {
            Process => UiThread.Invoke(() => CreateTimer(pm)),
            _ => null
        };
    }

    private static Timer? CreateTimer(ProcessMonitor pm)
    {
        pm.SetValue(ProcessNamePropertyKey, pm.MonitoredProcess!.ProcessName);
        pm.prevCpuTime = pm.MonitoredProcess!.TotalProcessorTime;
        pm.prevTime = DateTime.Now;
        return new(_ => UiThread.Invoke(() => pm?.Refresh()), null, 1000, pm.RefreshInterval);
    }

    private TimeSpan prevCpuTime;
    private DateTime prevTime;
    private TimeSpan nextCpuTime;
    private DateTime nextTime;
    private Timer? _timer;

    /// <summary>
    /// Indicates if the process is locked-up; that is, if it's not responding
    /// to messages from the operating system.
    /// </summary>
    public bool IsProcessLocked => UiThread.Invoke(() => (bool)GetValue(IsProcessLockedProperty));

    /// <summary>
    /// Indicates the amount of memory being consumed by the process in bytes.
    /// </summary>
    public long RamUsage => UiThread.Invoke(() => (long)GetValue(RamUsageProperty));

    /// <summary>
    /// Indicates the CPU usage of the process.
    /// </summary>
    public double CpuUsage => UiThread.Invoke(() => (double)GetValue(CpuUsageProperty));

    /// <summary>
    /// Gets the name of the process being monitored.
    /// </summary>
    public string? ProcessName => UiThread.Invoke(() => (string?)GetValue(ProcessNameProperty));

    /// <summary>
    /// Gets or sets a reference to the process being monitored.
    /// </summary>
    public Process? MonitoredProcess
    {
        get => UiThread.Invoke(() => (Process?)GetValue(MonitoredProcessProperty));
        set => UiThread.Invoke(() => SetValue(MonitoredProcessProperty, value));
    }

    /// <summary>
    /// Gets or sets the desired refresh interval for the process telemetry.
    /// </summary>
    public int RefreshInterval
    {
        get => UiThread.Invoke(() => (int)GetValue(RefreshIntervalProperty));
        set => UiThread.Invoke(() => SetValue(RefreshIntervalProperty, value));
    }

    /// <summary>
    /// Gets or sets the content to be displayed if the process stops responding.
    /// </summary>
    public object? NotRespondingContent
    {
        get => UiThread.Invoke(() => GetValue(NotRespondingContentProperty));
        set => UiThread.Invoke(() => SetValue(NotRespondingContentProperty, value));
    }

    private void Refresh()
    {
        (bool locked, long ram, double cpu) = MonitoredProcess is null ? (false, 0L, double.NaN) : (
            !MonitoredProcess.Responding,
            MonitoredProcess.WorkingSet64 + MonitoredProcess.PagedMemorySize64,
            UiThread.Invoke(CalculateCpuUsage));

        SetValue(IsProcessLockedPropertyKey, locked);
        SetValue(RamUsagePropertyKey, ram);
        SetValue(CpuUsagePropertyKey, cpu);
    }

    private double CalculateCpuUsage()
    {
        nextCpuTime = MonitoredProcess!.TotalProcessorTime;
        nextTime = DateTime.Now;
        TimeSpan cpuTimeUsed = nextCpuTime - prevCpuTime;
        TimeSpan elapsedTime = nextTime - prevTime;
        prevCpuTime = nextCpuTime;
        prevTime = nextTime;
        if (elapsedTime == TimeSpan.Zero) return 0;
        return cpuTimeUsed.TotalMilliseconds / (elapsedTime.TotalMilliseconds * Environment.ProcessorCount) * 100;
    }
}
