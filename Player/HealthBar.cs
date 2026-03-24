using Godot;
using System;

public partial class HealthBar : ProgressBar
{
	[Export(PropertyHint.NodeType, "RigidBody3D")]
	PlayerCharacter player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MaxValue = player.MaxHealth;
		Value = player.CurrentHealth;
		player.HealthChanged += HealthChanged;
	}

	private void HealthChanged(float current, float max)
	{
		MaxValue = max;
		Value = current;
	}
}
