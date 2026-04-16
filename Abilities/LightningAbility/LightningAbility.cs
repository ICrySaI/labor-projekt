using Godot;
using System.Collections.Generic;

public partial class LightningAbility : AbilityBase
{
	public int chainAmount = 3;
	public float chainRange = 10;
	public float damage = 50;

	private PackedScene VFXScene = GD.Load<PackedScene>("res://Abilities/LightningAbility/LightningVFX.tscn");
	
	private List<Node3D> enemiesHit = new List<Node3D>();

    public override void Fire(Vector3 at)
    {
		// start chain
		ChainFrom(at, chainAmount);

		// clear target list
		enemiesHit.Clear();

        base.Fire(at);
    }

	private void ChainFrom(Vector3 pos, int amount)
	{
		// stop if we've reached the end of the chain
		if(amount <= 0) return;

		EnemyBase target = null;
		float targetDistance = 0;
		// goes through all the enemies to select the target
		foreach (EnemyBase enemy in GetTree().GetNodesInGroup("Enemies"))
		{
			float distance = pos.DistanceTo(enemy.GlobalPosition);
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
			Vector3 hitPosition = target.GlobalPosition + new Vector3(0, 1, 0);
			LightningVfx lightning = (LightningVfx)VFXScene.Instantiate();
			AddChild(lightning);
			lightning.TriggerEffect(pos, hitPosition); // vfx instances remove themselves from the tree after playing

			// damage enemy
			target.CurrentHealth -= damage;

			// continue chain from enemy with the amount reduced
			ChainFrom(hitPosition, amount - 1);
		}
	}
}
