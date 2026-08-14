using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Tharga.Wpf.Framework;

/// <summary>
/// Manages single-instance behavior using a mutex and named pipe for cross-process communication.
/// </summary>
internal class SingleInstanceService : IDisposable
{
    private const string ShowCommand = "SHOW";
    private const string NamePrefix = "Tharga.Wpf.";

    private readonly string _pipeName;
    private readonly string _mutexName;
    private Mutex _mutex;
    private CancellationTokenSource _cts;
    private Action _onShowRequested;

    public SingleInstanceService(string applicationName)
    {
        _pipeName = BuildPipeName(applicationName);
        _mutexName = BuildMutexName(applicationName);
    }

    /// <summary>
    /// The machine-wide name of the single-instance mutex. Exposed for testing.
    /// </summary>
    internal string MutexName => _mutexName;

    /// <summary>
    /// The name of the show-signal pipe. Exposed for testing.
    /// </summary>
    internal string PipeName => _pipeName;

    /// <summary>
    /// Builds the show-signal pipe name. Named pipes are already machine-wide, so this
    /// deliberately carries no namespace prefix.
    /// </summary>
    internal static string BuildPipeName(string applicationName) => $"{NamePrefix}{applicationName}";

    /// <summary>
    /// Builds the mutex name in the <c>Global\</c> kernel namespace, so the lock is scoped to
    /// the machine rather than the Windows session. An unprefixed name resolves in
    /// <c>Local\</c>, which is per-session, so two signed-in users would each get an instance.
    /// </summary>
    internal static string BuildMutexName(string applicationName) => $@"Global\{NamePrefix}{applicationName}";

    /// <summary>
    /// Tries to acquire the single-instance lock.
    /// Returns true if this is the first instance, false if another instance is already running.
    /// </summary>
    public bool TryAcquire()
    {
        _mutex = CreateMutex(_mutexName, out var createdNew);
        return createdNew;
    }

    /// <summary>
    /// Creates the machine-wide mutex with an ACL that lets a process in another Windows session —
    /// potentially running as a different user — open it. Without the ACL that process gets
    /// <see cref="UnauthorizedAccessException"/> instead of being told the lock is already held,
    /// which would report "first instance" and defeat the guard.
    /// </summary>
    private static Mutex CreateMutex(string name, out bool createdNew)
    {
        var security = new MutexSecurity();
        security.AddAccessRule(new MutexAccessRule(
            new SecurityIdentifier(WellKnownSidType.WorldSid, null),
            MutexRights.Synchronize | MutexRights.Modify,
            AccessControlType.Allow));

        try
        {
            return MutexAcl.Create(true, name, out createdNew, security);
        }
        catch (UnauthorizedAccessException)
        {
            // The mutex exists but this process may not open it for full control. Being able to
            // open it at all proves another instance holds it.
            createdNew = false;
            return null;
        }
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
            // Assigned after the using declaration rather than in an object
            // initializer, so the writer is tracked for disposal from the moment
            // it is constructed.
            using var writer = new StreamWriter(client);
            writer.AutoFlush = true;
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
