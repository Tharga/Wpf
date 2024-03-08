using System.Windows.Controls;

namespace Tharga.Wpf.Features.TabNavigator;

public abstract class TabView : UserControl
{
    public virtual string Title => GetType().Name;
    public virtual bool AllowMultiple => true;
    public virtual bool CanClose => true;

    public virtual Task<bool> OnCloseAsync()
    {
        return Task.FromResult(true);
    }
}

//public abstract class TabView<TListViewModel, TModel, TSource> : TabView
//    where TListViewModel : ListViewModel<TModel, TSource>
//{
//    protected TabView()
//    {
//        DataContextChanged += OnDataContextChanged;
//    }

//    protected virtual async void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs ev)
//    {
//        var dataContext = (TListViewModel)DataContext;
//        await dataContext.InitialLoadAsync();
//    }
//}