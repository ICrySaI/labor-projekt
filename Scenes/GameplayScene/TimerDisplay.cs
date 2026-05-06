using Godot;
using System;

public partial class TimerDisplay : Label
{
    [Export(PropertyHint.NodeType, "Timer")]
    private GameTimer timer;

    public override void _Ready()
    {
        timer.Timeout += TimePassed;
        base._Ready();
    }

    public void TimePassed()
    {
        if(timer.Hours == 0) Text = "Time: " + timer.Minutes + ":" + timer.Seconds;
        else Text = "Time: " + timer.Hours + ":" + timer.Minutes + ":" + timer.Seconds;
    }
}
