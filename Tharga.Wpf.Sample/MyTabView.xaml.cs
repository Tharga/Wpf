﻿namespace Tharga.Wpf.Sample;

public partial class MyTabView
{
    public MyTabView()
    {
        CanClose = false;

        var timer = new System.Timers.Timer();
        timer.Interval = 3000;
        timer.Elapsed += (_, _) =>
        {
            CanClose = true;
            Title = "Yeee";
        };
        timer.Start();

        InitializeComponent();
    }

    public override string DefaultTitle => "Some title";
    public override bool AllowMultiple => true;
    public override bool AllowClose => true;
}