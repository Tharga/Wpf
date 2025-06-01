using System.Windows.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Tharga.Wpf.TabNavigator;

public abstract class TabView : UserControl
{
    private bool _canClose = true;
    private string _title;

    public event EventHandler<EventArgs> CanCloseChangedEvent;
    public event EventHandler<EventArgs> TitleChangedEvent;

    public virtual string DefaultTitle => GetType().Name;
    public virtual bool AllowMultiple => true;
    public virtual bool AllowClose => true;
    public virtual Task LoadActionAsync(object parameter) => Task.CompletedTask;

    public string Title
    {
        get => _title ?? DefaultTitle;
        set
        {
            if (_title == value) return;
            _title = value;
            TitleChangedEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool CanClose
    {
        get => _canClose && AllowClose;
        protected set
        {
            if (_canClose == value) return;
            _canClose = value;
            CanCloseChangedEvent?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Select()
    {
        var p = (TabItem)Parent;
        p.IsSelected = true;
    }

    public virtual Task<bool> OnCloseAsync()
    {
        return Task.FromResult(CanClose);
    }
}