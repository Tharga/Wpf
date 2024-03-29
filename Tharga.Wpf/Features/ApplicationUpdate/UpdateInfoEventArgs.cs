﻿namespace Tharga.Wpf.ApplicationUpdate;

public class UpdateInfoEventArgs : EventArgs
{
    public UpdateInfoEventArgs(string message)
    {
        Message = message;
    }

    public string Message { get; }
}