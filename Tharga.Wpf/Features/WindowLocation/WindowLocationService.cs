using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Logging;
using Tharga.Wpf.Framework;

namespace Tharga.Wpf.WindowLocation;

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

    public MinitorInfo Monitor(Window window, string name = default)
    {
        name ??= window.Name ?? window.Title?.Replace(" ", "_").NullIfEmpty() ?? window.GetType().Name.Replace(nameof(Window), "").NullIfEmpty() ?? throw new InvalidOperationException("Cannot find a name for the window");
        var monitorEngine = new MonitorEngine(name, window, _logger);
        if (!_monitors.TryAdd(name, monitorEngine)) throw new InvalidOperationException($"Window {name} is already attached to {nameof(WindowLocationService)}.");
        var minitorInfo = new MinitorInfo
        {
            FileLocation = monitorEngine.FileLocation,
            LoadLocation = monitorEngine.LoadLocation,
        };

        monitorEngine.LocationUpdatedEvent += minitorInfo.OnLocationUpdatedEvent;

        return minitorInfo;
    }

    private class MonitorEngine
    {
        private readonly Window _window;
        private readonly ILogger _logger;
        private readonly string _name;
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

            _window.Loaded += OnLoaded;

            if (_loadLocation == null || _loadLocation.Visibility == Visibility.Visible)
            {
                _window.Show();
            }
        }

        public event EventHandler<LocationUpdatedEventArgs> LocationUpdatedEvent;

        public string FileLocation => _fileLocation;
        public Location LoadLocation => _loadLocation;

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_loadLocation == null) return;
            _window.Left = _loadLocation.Left;
            _window.Top = _loadLocation.Top;
            _window.Width = _loadLocation.Width;
            _window.Height = _loadLocation.Height;
            _window.WindowState = _loadLocation.WindowState;    //TODO: Have a configuration start Minimized is allowed

            _window.LocationChanged += OnWindowChanged;
            _window.SizeChanged += OnWindowChanged;
            _window.StateChanged += OnWindowChanged;
            //_window.IsVisibleChanged += OnIsVisibleChanged;
            //_window.Closing += OnClosing;
            //_window.Unloaded += OnUnloaded;
        }

        private Location LoadLastLocation()
        {
            if (!File.Exists(FileLocation)) return null;

            try
            {
                var fileData = File.ReadAllText(FileLocation);
                var data = fileData.Split(";");
                if (data[0] != _name) throw new NotImplementedException("Cannot handle other windows yet.");
                if (data.Length <= 7) return null;

                var location = new Location
                {
                    WindowState = Enum.Parse<WindowState>(data[1], false),
                    Visibility = Enum.Parse<Visibility>(data[2], false),
                    Left = int.Parse(data[3]),
                    Top = int.Parse(data[4]),
                    Width = int.Parse(data[5]),
                    Height = int.Parse(data[6]),
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
            var yourAppDataPath = Path.Combine(appDataPath, _options.ApplicationShortName.Replace(" ", "_"));
            var fileLocation = $"{yourAppDataPath}\\Window_{_name}.txt";
            if (!Directory.Exists(yourAppDataPath)) Directory.CreateDirectory(yourAppDataPath);
            return fileLocation;
        }

        public void SetVisibility(Visibility visibility)
        {
            SetLocation();
        }

        private void OnWindowChanged(object sender, EventArgs e)
        {
            SetLocation();
        }

        private void SetLocation()
        {
            var lastLocation = _lastLocation ?? new Location();
            lastLocation = lastLocation with
            {
                WindowState = _window.WindowState,
                Visibility = _window.Visibility,
                Left = _window.Left >= 0 ? (int)_window.Left : (_lastLocation?.Left ?? 0),
                Top = _window.Top >= 0 ? (int)_window.Top : (_lastLocation?.Top ?? 0),
                Width = _window.Width > 0 ? (int)_window.Width : (_lastLocation?.Width ?? -1),
                Height = _window.Height > 0 ? (int)_window.Height : (_lastLocation?.Height ?? -1)
            };

            Exception e = null;
            try
            {
                if (_lastLocation != lastLocation)
                {
                    _lastLocation = lastLocation;
                    Debug.WriteLine($"Save window location for {_name}. ({_lastLocation.WindowState}, {_lastLocation.Visibility}: {_lastLocation.Left}, {_lastLocation.Top}, {_lastLocation.Width}, {_lastLocation.Height})");
                    var metadata = string.Join("|", _lastLocation.Metadata.Select(x => $"{x.Key}:{x.Value}"));
                    File.WriteAllText(FileLocation, $"{_name};{_lastLocation.WindowState};{_lastLocation.Visibility};{_lastLocation.Left};{_lastLocation.Top};{_lastLocation.Width};{_lastLocation.Height};{metadata}");
                }
            }
            catch (Exception exception)
            {
                e = exception;
                _logger?.LogError(exception, exception.Message);
                Debugger.Break();
            }

            LocationUpdatedEvent?.Invoke(this, new LocationUpdatedEventArgs(lastLocation, e));
        }

        public void AttachProperty(INotifyPropertyChanged container, string propertyName)
        {
            throw new NotImplementedException();
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
                        //_timer.Start();
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

    public void AttachProperty(string name, INotifyPropertyChanged container, string propertyName)
    {
        if (!_monitors.TryGetValue(name, out var monitor)) throw new InvalidOperationException($"Monitor for '{name}' must be created first.");
        monitor.AttachProperty(container, propertyName);
    }

    public void SetVisibility(string name, Visibility visibility)
    {
        if (!_monitors.TryGetValue(name, out var monitor)) throw new InvalidOperationException($"Monitor for '{name}' must be created first.");
        monitor.SetVisibility(visibility);
    }
}