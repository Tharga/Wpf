using System.Windows.Controls;

namespace Tharga.Wpf.Features.TabNavigator;

public abstract class TabView : UserControl
{
    private bool _canClose = true;

    public event EventHandler<EventArgs> CanCloseChangedEvent;

    public virtual string Title => GetType().Name;
    public virtual bool AllowMultiple => true;
    public virtual bool AllowClose => true;

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

    public virtual Task<bool> OnCloseAsync()
    {
        return Task.FromResult(CanClose);
    }
}