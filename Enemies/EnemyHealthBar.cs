using Godot;
using System;
using System.Diagnostics;

public partial class EnemyHealthBar : Sprite3D
{
	[Export(PropertyHint.NodeType, "CharacterBody3D")]
	EnemyBase enemy;

	ProgressBar healthbar;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		healthbar = (ProgressBar)GetNode("SubViewport/ProgressBar");
		healthbar.MaxValue = enemy.MaxHealth;
		healthbar.Value = enemy.CurrentHealth;
	}

	private void HealthChanged(float current, float max)
	{
		if(healthbar == null) return;
		healthbar.MaxValue = max;
		healthbar.Value = current;
	}
}
