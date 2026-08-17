using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
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

    public IWindowMonitor Monitor(Window window, string name = default, string environment = default, bool isMainWindow = false)
    {
        name ??= window.Name ?? window.Title?.Replace(" ", "_").NullIfEmpty() ?? window.GetType().Name.Replace(nameof(Window), "").NullIfEmpty() ?? throw new InvalidOperationException("Cannot find a name for the window");
        var monitorEngine = new MonitorEngine(name, environment, window, _logger, _options, isMainWindow);
        if (!_monitors.TryAdd(name, monitorEngine)) throw new InvalidOperationException($"Window {name} is already attached to {nameof(WindowLocationService)}.");

        window.Closed += (_, _) => _monitors.TryRemove(name, out _);

#pragma warning disable CS0618 // MinitorInfo is obsolete — used internally
        var monitor = new MinitorInfo
        {
            FileLocation = monitorEngine.FileLocation,
            LoadLocation = monitorEngine.LoadLocation,
        };
        monitorEngine.LocationUpdatedEvent += monitor.OnLocationUpdatedEvent;
#pragma warning restore CS0618

        return monitor;
    }

    private class MonitorEngine
    {
        private readonly string _name;
        private readonly string _environment;
        private readonly Window _window;
        private readonly ILogger _logger;
        private readonly ThargaWpfOptions _options;
        private readonly string _fileLocation;
        private readonly Location _loadLocation;

        private static readonly TimeSpan DisplayChangeDebounce = TimeSpan.FromMilliseconds(750);

        private readonly bool _isMainWindow;
        private Location _lastLocation;
        private Debouncer _displayChangeDebouncer;

        public MonitorEngine(string name, string environment, Window window, ILogger logger, ThargaWpfOptions options, bool isMainWindow = false)
        {
            _name = name;
            _environment = environment;
            _window = window;
            _logger = logger;
            _options = options;
            _isMainWindow = isMainWindow;

            _fileLocation = GetFileLocation();
            _loadLocation = LoadLastLocation();

            _window.Loaded += OnLoaded;

            _window.Closing += (_, _) =>
            {
                _window.LocationChanged -= OnWindowChanged;
                _window.SizeChanged -= OnWindowChanged;
                _window.StateChanged -= OnWindowChanged;
                SetLocation();
            };

            _window.Closed += (_, _) =>
            {
                SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
                _displayChangeDebouncer?.Dispose();
            };
        }

        public event EventHandler<LocationUpdatedEventArgs> LocationUpdatedEvent;

        public string FileLocation => _fileLocation;
        public Location LoadLocation => _loadLocation;

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_loadLocation != null)
            {
                var screens = GetScreenBounds();
                var validated = LocationValidator.Validate(_loadLocation, screens,
                    defaultWidth: (int)_window.Width, defaultHeight: (int)_window.Height);

                _window.Left = validated.Left;
                _window.Top = validated.Top;
                _window.Width = validated.Width;
                _window.Height = validated.Height;

                if (_isMainWindow)
                {
                    var startupState = _options.StartupWindowState;
                    switch (startupState)
                    {
                        case StartupWindowState.Last:
                            _window.WindowState = validated.WindowState;
                            break;
                        case StartupWindowState.Normal:
                            _window.WindowState = WindowState.Normal;
                            break;
                        case StartupWindowState.Maximized:
                            _window.WindowState = WindowState.Maximized;
                            break;
                        case StartupWindowState.Minimized:
                            _window.WindowState = WindowState.Minimized;
                            break;
                        case StartupWindowState.Hidden:
                            break;
                    }
                }
                else
                {
                    _window.WindowState = validated.WindowState;
                }
            }

            _window.LocationChanged += OnWindowChanged;
            _window.SizeChanged += OnWindowChanged;
            _window.StateChanged += OnWindowChanged;

            _displayChangeDebouncer = new Debouncer(DisplayChangeDebounce, RevalidateLocation);
            SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
        }

        private void OnDisplaySettingsChanged(object sender, EventArgs e)
        {
            _displayChangeDebouncer?.Trigger();
        }

        private void RevalidateLocation()
        {
            try
            {
                _window.Dispatcher.Invoke(() =>
                {
                    // Windows re-places maximized and minimized windows itself.
                    if (_window.WindowState != WindowState.Normal) return;

                    var width = (int)(double.IsNaN(_window.Width) ? _window.ActualWidth : _window.Width);
                    var height = (int)(double.IsNaN(_window.Height) ? _window.ActualHeight : _window.Height);
                    var location = new Location
                    {
                        WindowState = _window.WindowState,
                        Visibility = _window.Visibility,
                        Left = (int)_window.Left,
                        Top = (int)_window.Top,
                        Width = width,
                        Height = height
                    };

                    var validated = LocationValidator.Validate(location, GetScreenBounds(), defaultWidth: width, defaultHeight: height);
                    if (ReferenceEquals(validated, location)) return;

                    _logger?.LogInformation("Display setup changed. Window {Name} moved from ({Left}, {Top}, {Width}, {Height}) to ({NewLeft}, {NewTop}, {NewWidth}, {NewHeight}).", _name, location.Left, location.Top, location.Width, location.Height, validated.Left, validated.Top, validated.Width, validated.Height);

                    _window.Left = validated.Left;
                    _window.Top = validated.Top;
                    _window.Width = validated.Width;
                    _window.Height = validated.Height;
                });
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, exception.Message);
            }
        }

        private static IReadOnlyList<ScreenBounds> GetScreenBounds()
        {
            return System.Windows.Forms.Screen.AllScreens
                .Select(s => new ScreenBounds(
                    s.WorkingArea.Left,
                    s.WorkingArea.Top,
                    s.WorkingArea.Width,
                    s.WorkingArea.Height,
                    s.Primary))
                .ToList();
        }

        private Location LoadLastLocation()
        {
            if (!File.Exists(FileLocation)) return null;

            try
            {
                var fileData = File.ReadAllText(FileLocation);
                return LocationFormatter.Parse(_name, fileData);
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
            var yourAppDataPath = Path.Combine(appDataPath, _options.CompanyName ?? string.Empty, _options.ApplicationShortName.Replace(" ", "_"), _environment?.Replace("Production", string.Empty) ?? string.Empty);
            if (!Directory.Exists(yourAppDataPath)) Directory.CreateDirectory(yourAppDataPath);
            var fileLocation = $"{yourAppDataPath}\\Window_{_name}.txt";
            return fileLocation;
        }

        public void SetVisibility(Visibility visibility)
        {
            SetLocation(visibility);
        }

        private void OnWindowChanged(object sender, EventArgs e)
        {
            SetLocation();
        }

        private void SetLocation(Visibility? visibilityOverride = null)
        {
            var baseLocation = _lastLocation ?? _loadLocation ?? new Location();
            var lastLocation = baseLocation with
            {
                WindowState = _window.WindowState,
                Visibility = visibilityOverride ?? _window.Visibility
            };

            // Normal state only: minimized parks at -32000 and maximized must not overwrite the restore bounds; legitimately negative coordinates are saved as-is and validated on load.
            if (_window.WindowState == WindowState.Normal)
            {
                lastLocation = lastLocation with
                {
                    Left = double.IsNaN(_window.Left) ? baseLocation.Left : (int)_window.Left,
                    Top = double.IsNaN(_window.Top) ? baseLocation.Top : (int)_window.Top,
                    Width = _window.Width > 0 ? (int)_window.Width : baseLocation.Width,
                    Height = _window.Height > 0 ? (int)_window.Height : baseLocation.Height
                };
            }

            Exception e = null;
            try
            {
                if (_lastLocation != lastLocation)
                {
                    _lastLocation = lastLocation;
                    Debug.WriteLine($"Save window location for {_name}. ({_lastLocation.WindowState}, {_lastLocation.Visibility}: {_lastLocation.Left}, {_lastLocation.Top}, {_lastLocation.Width}, {_lastLocation.Height})");
                    File.WriteAllText(FileLocation, LocationFormatter.Serialize(_name, _lastLocation));
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

        //public void AttachProperty(INotifyPropertyChanged container, string propertyName)
        //{
        //    throw new NotImplementedException();
        //    container.PropertyChanged += (s, e) =>
        //    {
        //        if (e.PropertyName != propertyName) return;

        //        try
        //        {
        //            var prop = container.GetType().GetProperty(propertyName) ?? throw new NullReferenceException($"Cannot find property named '{propertyName}' in '{container.GetType().Name}'.");
        //            var val = prop.GetValue(container);

        //            if (val != default)
        //            {
        //                if (!_lastLocation.Metadata.TryAdd(propertyName, val.ToString()))
        //                {
        //                    _lastLocation.Metadata[propertyName] = val.ToString();
        //                }
        //                //_timer.Start();
        //            }
        //        }
        //        catch (Exception exception)
        //        {
        //            _logger?.LogError(exception, exception.Message);
        //            Debugger.Break();
        //        }
        //    };

        //    try
        //    {
        //        //var val = _lastLocation.GetMetadata(propertyName);
        //        if (_lastLocation.Metadata.TryGetValue(propertyName, out var val))
        //        {
        //            var prop = container.GetType().GetProperty(propertyName) ?? throw new NullReferenceException($"Cannot find property named '{propertyName}' in '{container.GetType().Name}'.");

        //            var typeConverter = TypeDescriptor.GetConverter(prop.PropertyType);
        //            if (val != default)
        //            {
        //                var v = typeConverter.ConvertFromString(val);
        //                prop.SetValue(container, v);
        //            }
        //        }
        //        else
        //        {
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        _logger?.LogError(exception, exception.Message);
        //        Debugger.Break();
        //    }
        //}
    }

    //public void AttachProperty(string name, INotifyPropertyChanged container, string propertyName)
    //{
    //    if (!_monitors.TryGetValue(name, out var monitor)) throw new InvalidOperationException($"Monitor for '{name}' must be created first.");
    //    monitor.AttachProperty(container, propertyName);
    //}

    public void SetVisibility(string name, Visibility visibility)
    {
        if (!_monitors.TryGetValue(name, out var monitor)) throw new InvalidOperationException($"Monitor for '{name}' must be created first.");
        monitor.SetVisibility(visibility);
    }

    internal bool ShouldShowOnStartup(string name)
    {
        if (!_monitors.TryGetValue(name, out var monitor)) return true;

        return _options.StartupWindowState switch
        {
            StartupWindowState.Hidden => false,
            StartupWindowState.Last => monitor.LoadLocation?.Visibility != Visibility.Hidden,
            _ => true
        };
    }

    public string GetFolder(string environment)
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var yourAppDataPath = Path.Combine(appDataPath, _options.CompanyName ?? string.Empty, _options.ApplicationShortName.Replace(" ", "_"), environment?.Replace("Production", string.Empty) ?? string.Empty);
        if (!Directory.Exists(yourAppDataPath)) Directory.CreateDirectory(yourAppDataPath);
        return yourAppDataPath;
    }
}