using Godot;
using System;

public partial class GameTimer : Timer
{
    public uint Seconds {
        get {
            return _seconds;
        }
        private set {
            if(value >= 60)
            {
                Minutes += value / 60;
                _seconds = value % 60;
            }
            else _seconds = value;
    } }
    private uint _seconds = 0;

    public uint Minutes {
        get {
            return _minutes;
        }
        private set {
            if (value >= 60)
            {
                Hours += value / 60;
                _minutes = value % 60;
            }
            else _minutes = value;
    } }
    private uint _minutes = 0;

    public uint Hours {
        get {
            return _hours;
        }
        private set {
            _hours = value;
    } }
    private uint _hours = 0;

    public uint TotalSeconds { get; private set; }

    public override void _Ready()
    {
        Timeout += TimePassed;
        base._Ready();
    }

    private void TimePassed()
    {
        Seconds++;
        TotalSeconds++;
    }
}
