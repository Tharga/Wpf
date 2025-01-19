//namespace Tharga.Wpf;

//public record Inactivity
//{
//    /// <summary>
//    /// Event that triggers on inactivity.
//    /// </summary>
//    public event EventHandler<EventArgs> InactivityEvent;

//    /// <summary>
//    /// Set the time for when inatcivity action should be triggered.
//    /// </summary>
//    public TimeSpan Timeout { get; set; }

//    internal void OnInactivity()
//    {
//        InactivityEvent?.Invoke(this, EventArgs.Empty);
//    }
//}