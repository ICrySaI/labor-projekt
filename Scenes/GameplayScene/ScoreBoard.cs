using Godot;
using System;

public partial class ScoreBoard : Label
{
    public int Score { 
        get { return _score; } 
        set {
            _score = value;
            Text = "Score: " + value;
    } }
    private int _score;

    public override void _Ready()
    {
        base._Ready();
    }
}
