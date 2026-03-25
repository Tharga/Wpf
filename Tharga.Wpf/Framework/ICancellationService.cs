namespace Tharga.Wpf.Framework;

/// <summary>
/// Provides a cancellation token that can be used to signal application shutdown or task cancellation.
/// </summary>
public interface ICancellationService
{
    /// <summary>
    /// Gets the cancellation token.
    /// </summary>
    CancellationToken Token { get; }
}