using Godot;
using System;
using System.ComponentModel;
using System.Diagnostics;

public partial class EnemyHealthBar : Sprite3D
{
	[Export(PropertyHint.NodeType, "CharacterBody3D")]
	private EnemyBase enemy;

	private ProgressBar healthbar;

	private Label level;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		healthbar = GetNode<ProgressBar>("SubViewport/HBoxContainer/ProgressBar");
		healthbar.MaxValue = enemy.MaxHealth;
		healthbar.Value = enemy.CurrentHealth;

		level = GetNode<Label>("SubViewport/HBoxContainer/Level");
		level.Text = Math.Clamp(enemy.Level, 0, 9999).ToString(); // can't display levels higher than 9999
	}

	private void HealthChanged(float current, float max)
	{
		if(IsInstanceValid(healthbar))
		{
			healthbar.MaxValue = max;
			healthbar.Value = current;
		}
	}

	private void LevelChanged(int newLevel)
	{
		if(IsInstanceValid(level))
		{
			level.Text = Math.Clamp(newLevel, 0, 9999).ToString(); // can't display levels higher than 9999
		}
	}
}
