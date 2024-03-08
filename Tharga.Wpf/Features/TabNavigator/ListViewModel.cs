//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Runtime.CompilerServices;
//using System.Windows;
//using Microsoft.Extensions.Logging;
//using Tharga.Florida.Client.Framework.EntityService;

//namespace Tharga.Florida.Client.Framework.TabNavigator;

//public abstract class ListViewModel<T, TSource> : INotifyPropertyChanged
//{
//    private readonly ILogger _logger;
//    private readonly ObservableCollection<T> _items = new();
//    private T _selectedItem;
//    private Visibility _spinnerVisibility = Visibility.Visible;
//    private Visibility _listVisibility = Visibility.Collapsed;

//    protected ListViewModel(IEventHub iEventHub, ILogger logger)
//    {
//        _logger = logger;
//        iEventHub.CacheUpdateStartEvent += (_, e) =>
//        {
//            if (e.DataType == typeof(TSource))
//            {
//                ListVisibility = Visibility.Collapsed;
//                SpinnerVisibility = Visibility.Visible;
//                Application.Current.Dispatcher.Invoke(() => Items.Clear());
//            }
//        };

//        iEventHub.CacheUpdateCompleteEvent += async (_, e) =>
//        {
//            try
//            {
//                if (e.DataType == typeof(TSource))
//                {
//                    await Application.Current.Dispatcher.Invoke(async () => { await InitialLoadAsync(); });
//                }
//            }
//            catch (Exception exception)
//            {
//                _logger.LogError(exception, exception.Message);
//            }
//        };
//    }

//    public event PropertyChangedEventHandler PropertyChanged;

//    public Visibility SpinnerVisibility
//    {
//        get => _spinnerVisibility;
//        set => SetField(ref _spinnerVisibility, value);
//    }

//    public Visibility ListVisibility
//    {
//        get => _listVisibility;
//        set => SetField(ref _listVisibility, value);
//    }

//    public ObservableCollection<T> Items => _items;

//    public T SelectedItem
//    {
//        get => _selectedItem;
//        set
//        {
//            if (Equals(value, _selectedItem)) return;
//            _selectedItem = value;
//            System.Diagnostics.Debug.WriteLine($"Item '{value}' selected.");
//            OnPropertyChanged();
//        }
//    }

//    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
//    {
//        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//    }

//    protected bool SetField<TItem>(ref TItem field, TItem value, [CallerMemberName] string propertyName = null)
//    {
//        if (EqualityComparer<TItem>.Default.Equals(field, value)) return false;
//        field = value;
//        OnPropertyChanged(propertyName);
//        return true;
//    }

//    protected abstract Task<T[]> GetAllItemsAsync();

//    public async Task InitialLoadAsync()
//    {
//        var items = await GetAllItemsAsync();
//        _items.AddRange(items);

//        SpinnerVisibility = Visibility.Collapsed;
//        ListVisibility = Visibility.Visible;
//    }
//}