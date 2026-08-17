namespace Tharga.Wpf.ApplicationUpdate;

internal static class VelopackUpdateMessage
{
    public static string Build(string targetVersion, int deltaCount, long deltaSize, long fullSize, int maximumDeltasBeforeFallback)
    {
        var usesDeltas = deltaCount > 0 && deltaCount <= maximumDeltasBeforeFallback && deltaSize <= fullSize;
        if (!usesDeltas) return $"version {targetVersion} (full)";

        return deltaCount == 1
            ? $"version {targetVersion} (delta)"
            : $"version {targetVersion} ({deltaCount} deltas)";
    }
}
