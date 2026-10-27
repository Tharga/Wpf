using System.ComponentModel;

namespace Tharga.Wpf.Framework;

/// <summary>
/// Lets a synchronous <c>Closing</c> handler wait for an asynchronous close check.
/// WPF reads <see cref="CancelEventArgs.Cancel"/> as soon as the handler returns, so a value set after an await is ignored.
/// When the check does not complete synchronously, the close is cancelled right away, the check runs to completion,
/// and when it allows closing the window is closed again. That second pass is let through by <see cref="TryConsumeConfirmed"/>.
/// Pure logic with no UI dependencies, designed for testability.
/// </summary>
internal sealed class AsyncCloseGate
{
    private bool _checking;
    private bool _confirmed;

    /// <summary>
    /// Returns <c>true</c> once, for the close that follows a check that allowed closing.
    /// The caller should then let the close through without checking again.
    /// </summary>
    public bool TryConsumeConfirmed()
    {
        if (!_confirmed) return false;
        _confirmed = false;
        return true;
    }

    /// <summary>
    /// Runs the close check for one <c>Closing</c> pass.
    /// </summary>
    /// <param name="e">The closing event args. <see cref="CancelEventArgs.Cancel"/> is set before this method first yields.</param>
    /// <param name="canCloseAsync">The check. Returns <c>true</c> if the window may close.</param>
    /// <param name="proceed">Called when the check completes synchronously and allows closing.</param>
    /// <param name="closeAgain">Called when the check completes asynchronously and allows closing. It should close the window again.</param>
    public async Task RunAsync(CancelEventArgs e, Func<Task<bool>> canCloseAsync, Action proceed, Action closeAgain)
    {
        if (_checking)
        {
            e.Cancel = true;
            return;
        }

        var check = canCloseAsync();
        if (check.IsCompleted)
        {
            if (check.GetAwaiter().GetResult())
            {
                proceed?.Invoke();
            }
            else
            {
                e.Cancel = true;
            }

            return;
        }

        e.Cancel = true;
        _checking = true;
        bool canClose;
        try
        {
            canClose = await check;
        }
        finally
        {
            _checking = false;
        }

        if (!canClose) return;

        _confirmed = true;
        closeAgain();
    }
}
