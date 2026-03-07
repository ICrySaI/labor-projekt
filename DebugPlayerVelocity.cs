using Godot;
using System;

public partial class DebugPlayerVelocity : Label
{
	private RigidBody3D playerCharacter;
	private ulong lastUpdateTick = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerCharacter = (RigidBody3D)GetParent();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Time.GetTicksMsec() - lastUpdateTick > 100)
		{
			Vector3 horizontalVelocity = playerCharacter.LinearVelocity;
			horizontalVelocity.Y = 0;
			Text = "Velocity: " + Math.Round(horizontalVelocity.Length(), 2);
			lastUpdateTick = Time.GetTicksMsec();
		}
	}
}
