# Single instance

By default, Tharga.Wpf allows multiple instances of an application to run side-by-side. Set `AllowMultipleApplications = false` to enforce a single instance:

```csharp
protected override void Options(ThargaWpfOptions options)
{
    options.AllowMultipleApplications = false;
}
```

## What happens on second launch

1. A second instance starts.
2. It detects the running instance via a named-mutex lock.
3. It signals the running instance to show itself — even if hidden in the system tray, the main window is restored and brought to the foreground.
4. The second instance exits without showing UI, and **without constructing its main window**.

The user perceives a "click the shortcut to bring my app back" experience instead of a second window opening.

> **Point 4 changed in 2.4.0.** Before that, a suppressed instance still ran its whole main-window constructor. `Application.Shutdown()` only posts to the dispatcher, so WPF's own startup continued and loaded `StartupUri` anyway — building the window, and any tray icon or service resolution its constructor performs, in a process that was about to exit and whose host had never been started. If you set `StartupUri`, you were affected; the symptom is a brief flash and a duplicate tray icon. Nothing is required of you, but if you added a workaround (an early return or a guard in the window constructor) it can now go.

## Scope of the lock

**The lock is machine-wide.** One instance per machine, across every signed-in Windows session — fast user switching, RDP, or a switched-away session left signed in.

> **This changed in 2.4.0.** The lock was previously scoped to the Windows *session*, not the machine, so two signed-in users each got their own instance despite `AllowMultipleApplications = false`. The mutex name was unprefixed, and an unprefixed kernel object name resolves in the per-session `Local\` namespace rather than the machine-wide `Global\` one. If two instances on one machine were somehow relied upon, that no longer happens.

The show-signal channel is a named pipe, which was already machine-wide and is unchanged.

## Interaction with Velopack updates

The single-instance lock is automatically **released before a Velopack restart**. This avoids a deadlock where the updated process tries to start while the old process still holds the lock. The release happens in `ApplicationUpdateStateServiceBase.BeforeRestartAsync()` — you don't need to do anything special when you opt into single-instance mode plus Velopack updates.

## Choosing the lock name

The lock is keyed off **`ApplicationFullName`**. Two applications with the same full name (e.g. two builds of the same product) will see each other; two applications with different full names will not. Set this in `Options`:

```csharp
protected override void Options(ThargaWpfOptions options)
{
    options.ApplicationFullName = "My Application";
    options.AllowMultipleApplications = false;
}
```

This is deliberately useful: varying `ApplicationFullName` per environment lets a Test and a Production build of the same application run side by side on one machine, each holding its own lock.

> Earlier versions of this page said the lock was keyed off `ApplicationShortName`. That was never true of the code — `ApplicationBase` has always used `ApplicationFullName`. Corrected in 2.4.0. If you set `ApplicationShortName` expecting it to control the lock, set `ApplicationFullName` instead.
