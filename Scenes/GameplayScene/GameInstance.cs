using Godot;
using System;
using System.ComponentModel;

public partial class GameInstance : Node3D
{
	[Export(PropertyHint.NodeType, "Label")]
	private ScoreBoard scoreBoard;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Globals.CaptureMouse();
		SignalBus.Instance.EnemyKilled += EnemyKilled;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void EnemyKilled(EnemyBase enemy, PlayerCharacter killedBy)
	{
		scoreBoard.Score += enemy.Level * 10;
		enemy.QueueFree();
	}
}
