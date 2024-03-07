using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows;
using Microsoft.Extensions.Logging;
using Tharga.Wpf.Framework;
using Timer = System.Timers.Timer;

namespace Tharga.Wpf.Features.WindowLocation;

internal class WindowLocationService : IWindowLocationService
{
    private static ThargaWpfOptions _options;
    private readonly ILogger<WindowLocationService> _logger;
    private readonly ConcurrentDictionary<string, MonitorEngine> _monitors = new();

    public WindowLocationService(ThargaWpfOptions options, ILogger<WindowLocationService> logger)
    {
        _options = options;
        _logger = logger;
    }

    public void Monitor(Window window, string name = default)
    {
        name ??= window.Name ?? window.Title?.Replace(" ", "_").NullIfEmpty() ?? window.GetType().Name.Replace(nameof(Window), "").NullIfEmpty() ?? throw new InvalidOperationException("Cannot find a name for the window");
        if (!_monitors.TryAdd(name, new MonitorEngine(name, window, _logger))) throw new InvalidOperationException($"Window {name} is already attached to {nameof(WindowLocationService)}.");
    }

    private class MonitorEngine
    {
        private readonly Window _window;
        private readonly ILogger _logger;
        private readonly string _name;
        private readonly Timer _timer;
        private readonly string _fileLocation;
        private readonly Location _loadLocation;

        private Location _lastLocation;

        public MonitorEngine(string name, Window window, ILogger logger)
        {
            _name = name;
            _window = window;
            _logger = logger;

            _fileLocation = GetFileLocation();
            _loadLocation = LoadLastLocation();

            _window.LocationChanged += OnWindowChanged;
            _window.SizeChanged += OnWindowChanged;
            _window.StateChanged += OnWindowChanged;
            _window.Loaded += OnLoaded;

            _timer = new Timer(TimeSpan.FromSeconds(1)) { AutoReset = false };
            _timer.Elapsed += SaveLocation;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_loadLocation == null) return;
            _window.Left = _loadLocation.Left;
            _window.Top = _loadLocation.Top;
            _window.Width = _loadLocation.Width;
            _window.Height = _loadLocation.Height;
            _window.WindowState = _loadLocation.WindowState;

            _lastLocation = _loadLocation;
        }

        private void SaveLocation(object sender, ElapsedEventArgs e)
        {
            if (_lastLocation == null) return;

            try
            {
                Debug.WriteLine($"Save window location for {_name}. ({_lastLocation.WindowState}: {_lastLocation.Left}, {_lastLocation.Top}, {_lastLocation.Width}, {_lastLocation.Height})");
                var metadata = string.Join("|", _lastLocation.Metadata.Select(x => $"{x.Key}:{x.Value}"));
                File.WriteAllText(_fileLocation, $"{_name};{_lastLocation.WindowState};{_lastLocation.Left};{_lastLocation.Top};{_lastLocation.Width};{_lastLocation.Height};{metadata}");
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, exception.Message);
                Debugger.Break();
            }
        }

        private Location LoadLastLocation()
        {
            if (!File.Exists(_fileLocation)) return null;

            try
            {
                var fileData = File.ReadAllText(_fileLocation);
                var data = fileData.Split(";");
                if (data[0] != _name) throw new NotImplementedException("Cannot handle other windows yet.");

                var location = new Location
                {
                    WindowState = Enum.Parse<WindowState>(data[1], false),
                    Left = int.Parse(data[2]),
                    Top = int.Parse(data[3]),
                    Width = int.Parse(data[4]),
                    Height = int.Parse(data[5]),
                };

                if (data.Length > 6)
                {
                    var meta = data[6];
                    var pairs = meta.Split('|');
                    foreach (var pair in pairs)
                    {
                        var vals = pair.Split(":");
                        if (vals.Length > 1)
                        {
                            location.Metadata.TryAdd(vals[0], vals[1]);
                        }
                    }
                }

                return location;
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, exception.Message);
                Debugger.Break();
                return null;
            }
        }

        private string GetFileLocation()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var yourAppDataPath = Path.Combine(appDataPath, _options.ApplicationShortName);
            var fileLocation = $"{yourAppDataPath}\\Window_{_name}.txt";
            if (!Directory.Exists(yourAppDataPath)) Directory.CreateDirectory(yourAppDataPath);
            return fileLocation;
        }

        private void OnWindowChanged(object sender, EventArgs e)
        {
            if (_window.WindowState == WindowState.Minimized) return;
            if (_window.Left < 0 || _window.Top < 0 || _window.Width < 0 || _window.Height < 0) return;

            _timer.Stop();

            _lastLocation ??= new Location();
            _lastLocation = _lastLocation with
            {
                WindowState = _window.WindowState,
                Left = (int)_window.Left,
                Top = (int)_window.Top,
                Width = (int)_window.Width,
                Height = (int)_window.Height
            };

            _timer.Start();
        }

        public void AttachProperty(INotifyPropertyChanged container, string propertyName)
        {
            container.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName != propertyName) return;

                try
                {
                    var prop = container.GetType().GetProperty(propertyName) ?? throw new NullReferenceException($"Cannot find property named '{propertyName}' in '{container.GetType().Name}'.");
                    var val = prop.GetValue(container);

                    if (val != default)
                    {
                        if (!_lastLocation.Metadata.TryAdd(propertyName, val.ToString()))
                        {
                            _lastLocation.Metadata[propertyName] = val.ToString();
                        }
                        _timer.Start();
                    }
                }
                catch (Exception exception)
                {
                    _logger?.LogError(exception, exception.Message);
                    Debugger.Break();
                }
            };

            try
            {
                //var val = _lastLocation.GetMetadata(propertyName);
                if (_lastLocation.Metadata.TryGetValue(propertyName, out var val))
                {
                    var prop = container.GetType().GetProperty(propertyName) ?? throw new NullReferenceException($"Cannot find property named '{propertyName}' in '{container.GetType().Name}'.");

                    var typeConverter = TypeDescriptor.GetConverter(prop.PropertyType);
                    if (val != default)
                    {
                        var v = typeConverter.ConvertFromString(val);
                        prop.SetValue(container, v);
                    }
                }
                else
                {
                }
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, exception.Message);
                Debugger.Break();
            }
        }
    }

    private record Location
    {
        public WindowState WindowState { get; init; }
        public int Left { get; init; }
        public int Top { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
        public readonly Dictionary<string, string> Metadata = new();

        //public string GetMetadata(string propertyName)
        //{
        //    Metadata.TryGetValue(propertyName, out var val);
        //    return val;
        //}

        //public void SetMetadata(string propertyName, string val)
        //{
        //    Metadata.TryAdd(propertyName, val);
        //}
    }

    public void AttachProperty(string name, INotifyPropertyChanged container, string propertyName)
    {
        if (!_monitors.TryGetValue(name, out var monitor)) throw new InvalidOperationException($"Monitor for '{name}' must be created first.");
        monitor.AttachProperty(container, propertyName);
    }
}