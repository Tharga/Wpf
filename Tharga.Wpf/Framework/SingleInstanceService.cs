using System.IO;
using System.IO.Pipes;

namespace Tharga.Wpf.Framework;

/// <summary>
/// Manages single-instance behavior using a mutex and named pipe for cross-process communication.
/// </summary>
internal class SingleInstanceService : IDisposable
{
    private const string ShowCommand = "SHOW";

    private readonly string _pipeName;
    private Mutex _mutex;
    private CancellationTokenSource _cts;
    private Action _onShowRequested;

    public SingleInstanceService(string applicationName)
    {
        _pipeName = $"Tharga.Wpf.{applicationName}";
    }

    /// <summary>
    /// Tries to acquire the single-instance lock.
    /// Returns true if this is the first instance, false if another instance is already running.
    /// </summary>
    public bool TryAcquire()
    {
        _mutex = new Mutex(true, _pipeName, out var createdNew);
        return createdNew;
    }

    /// <summary>
    /// Sends a "show" signal to the existing instance via named pipe.
    /// </summary>
    public static void SignalExistingInstance(string applicationName)
    {
        var pipeName = $"Tharga.Wpf.{applicationName}";
        try
        {
            using var client = new NamedPipeClientStream(".", pipeName, PipeDirection.Out);
            client.Connect(timeout: 2000);
            using var writer = new StreamWriter(client) { AutoFlush = true };
            writer.WriteLine(ShowCommand);
        }
        catch
        {
            // Existing instance may not be listening — ignore.
        }
    }

    /// <summary>
    /// Starts listening for "show" signals from other instances.
    /// </summary>
    /// <param name="onShowRequested">Called on the caller's context when a show signal is received.</param>
    public void StartListening(Action onShowRequested)
    {
        _onShowRequested = onShowRequested;
        _cts = new CancellationTokenSource();
        Task.Run(() => ListenLoop(_cts.Token));
    }

    /// <summary>
    /// Releases the mutex to allow a new instance (e.g. during update restart).
    /// </summary>
    public void ReleaseMutex()
    {
        try
        {
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
            _mutex = null;
        }
        catch (ApplicationException)
        {
            // Mutex was not owned — ignore.
        }
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        ReleaseMutex();
    }

    private async Task ListenLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                using var server = new NamedPipeServerStream(_pipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                await server.WaitForConnectionAsync(ct);
                using var reader = new StreamReader(server);
                var message = await reader.ReadLineAsync(ct);
                if (message == ShowCommand)
                {
                    _onShowRequested?.Invoke();
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch
            {
                // Pipe error — retry after a short delay.
                try { await Task.Delay(100, ct); } catch { break; }
            }
        }
    }
}
