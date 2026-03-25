namespace Tharga.Wpf.TabNavigator;

/// <summary>
/// Indicates the result of an <see cref="ITabNavigationStateService.OpenTabAsync{TTabView}"/> call.
/// </summary>
public enum TabAction
{
    /// <summary>A new tab was created.</summary>
    Created,

    /// <summary>An existing tab was focused.</summary>
    Focused
}