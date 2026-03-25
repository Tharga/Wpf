using System.Windows.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Tharga.Wpf.TabNavigator;

/// <summary>
/// Abstract base class for tab views displayed in the <see cref="ITabNavigationStateService"/>.
/// Override virtual properties to control title, close behaviour, and multiple-instance rules.
/// </summary>
public abstract class TabView : UserControl
{
    private bool _canClose = true;
    private string _title;

    /// <summary>Raised when the <see cref="CanClose"/> property changes.</summary>
    public event EventHandler<EventArgs> CanCloseChangedEvent;

    /// <summary>Raised when the <see cref="Title"/> property changes.</summary>
    public event EventHandler<EventArgs> TitleChangedEvent;

    /// <summary>Gets the default title for this tab. Defaults to the type name.</summary>
    public virtual string DefaultTitle => GetType().Name;

    /// <summary>Gets a value indicating whether multiple instances of this tab type are allowed.</summary>
    public virtual bool AllowMultiple => true;

    /// <summary>Gets a value indicating whether this tab can be closed by the user.</summary>
    public virtual bool AllowClose => true;

    /// <summary>
    /// Called when the tab is opened, allowing async initialization with an optional parameter.
    /// </summary>
    /// <param name="parameter">An optional parameter passed when opening the tab.</param>
    public virtual Task LoadActionAsync(object parameter) => Task.CompletedTask;

    /// <summary>Gets or sets the tab title. Defaults to <see cref="DefaultTitle"/> when not set.</summary>
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

    /// <summary>Gets or sets a value indicating whether this tab can currently be closed.</summary>
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

    /// <summary>Selects this tab, making it the active tab in the navigator.</summary>
    public void Select()
    {
        var p = (TabItem)Parent;
        p.IsSelected = true;
    }

    /// <summary>
    /// Called when the tab is being closed. Override to add confirmation logic.
    /// </summary>
    /// <returns><c>true</c> if the tab can be closed; otherwise, <c>false</c>.</returns>
    public virtual Task<bool> OnCloseAsync()
    {
        return Task.FromResult(CanClose);
    }
}