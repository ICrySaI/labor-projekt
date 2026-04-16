using Godot;
using System.Collections.Generic;

public partial class LightningAbility : AbilityBase
{
	[Export]
	public int chainAmount = 3;
	[Export]
	public float chainRange = 10;
	[Export]
	public float damage = 10;

	private PackedScene VFXScene = GD.Load<PackedScene>("res://Abilities/LightningAbility/LightningVFX.tscn");
	
	private List<Node3D> enemiesHit = new List<Node3D>();

    public override void Fire()
    {
		// create chain
		ChainFrom(this, chainAmount);

		// damage all enemies hit
		foreach (EnemyBase enemy in enemiesHit)
		{
			enemy.CurrentHealth -= damage;
		}

		// clear target list
		enemiesHit.Clear();

        base.Fire();
    }

	private void ChainFrom(Node3D source, int amount)
	{
		// stop if we've reached the end of the chain
		if(amount <= 0) return;

		EnemyBase target = null;
		float targetDistance = 0;
		// goes through all the enemies to select the target
		foreach (EnemyBase enemy in GetTree().GetNodesInGroup("Enemies"))
		{
			float distance = source.GlobalPosition.DistanceTo(enemy.CenterOfMass.GlobalPosition);
			// if the enemy is not yet hit and is in range, but it's further than the current target, set it as the target
			if(distance >= targetDistance && distance <= chainRange && !enemiesHit.Contains(enemy))
			{
				target = enemy;
				targetDistance = distance;
			}
		}
		// if we found a target
		if(target != null)
		{
			// add to target list
			enemiesHit.Add(target);

			// create vfx
			LightningVfx lightning = VFXScene.Instantiate<LightningVfx>();
			AddChild(lightning);
			lightning.TriggerEffect(source, target.CenterOfMass); // vfx instances remove themselves from the tree after playing

			// continue chain from enemy with the amount reduced
			ChainFrom(target.CenterOfMass, amount - 1);
		}
	}
}
