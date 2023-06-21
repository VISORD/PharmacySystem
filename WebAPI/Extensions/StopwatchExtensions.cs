using System.Diagnostics;

namespace PharmacySystem.WebAPI.Extensions;

public static class StopwatchExtensions
{
    public static string FormatElapsed(this Stopwatch stopwatch)
    {
        var time = stopwatch.Elapsed;
        var parts = new List<string>();

        if (time.Days > 0)
        {
            parts.Add($"{time.Days}d");
        }

        if (time.Hours > 0)
        {
            parts.Add($"{time.Hours}h");
        }

        if (time.Minutes > 0)
        {
            parts.Add($"{time.Minutes}m");
        }

        if (time.Seconds > 0)
        {
            parts.Add($"{time.Seconds}s");
        }

        if (time.Milliseconds > 0)
        {
            parts.Add($"{time.Milliseconds}ms");
        }

        if (time.TotalMilliseconds < 1)
        {
            parts.Add("0ms");
        }

        return string.Join(" ", parts);
    }
}