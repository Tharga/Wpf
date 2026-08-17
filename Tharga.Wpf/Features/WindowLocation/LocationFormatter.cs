using System.Windows;

namespace Tharga.Wpf.WindowLocation;

internal static class LocationFormatter
{
    public static string Serialize(string name, Location location)
    {
        var metadata = string.Join("|", location.Metadata.Select(x => $"{x.Key}:{x.Value}"));
        return $"{name};{location.WindowState};{location.Visibility};{location.Left};{location.Top};{location.Width};{location.Height};{metadata}";
    }

    public static Location Parse(string name, string data)
    {
        var parts = data.Split(";");
        if (parts[0] != name) throw new NotImplementedException("Cannot handle other windows yet.");
        if (parts.Length <= 7) return null;

        var location = new Location
        {
            WindowState = Enum.Parse<WindowState>(parts[1], false),
            Visibility = Enum.Parse<Visibility>(parts[2], false),
            Left = int.Parse(parts[3]),
            Top = int.Parse(parts[4]),
            Width = int.Parse(parts[5]),
            Height = int.Parse(parts[6])
        };

        foreach (var pair in parts[7].Split('|'))
        {
            var values = pair.Split(":");
            if (values.Length > 1)
            {
                location.Metadata.TryAdd(values[0], values[1]);
            }
        }

        return location;
    }
}
