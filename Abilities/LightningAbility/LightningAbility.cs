using Godot;
using System.Collections.Generic;
using Chickensoft.UMLGenerator;

[ClassDiagram(UseVSCodePaths = true)]
public partial class LightningAbility : AbilityBase
{
	[Export]
	public int baseChainAmount { get; private set; } = 3;
	public int ChainAmount {
		get { return _chainAmount; }
		set { _chainAmount = value; }
	}
	private int _chainAmount;

	[Export]
	public float baseChainRange { get; private set; } = 10;
	public float ChainRange {
		get { return _chainRange; }
		set { _chainRange = value; }
	}
	private float _chainRange;

	[Export]
	public float baseDamage { get; private set; } = 10;
	public float Damage {
		get { return _damage; }
		set { _damage = value; }
	}
	private float _damage;

	private PackedScene VFXScene = GD.Load<PackedScene>("res://Abilities/LightningAbility/LightningVFX.tscn");
	
	private List<EnemyBase> enemiesHit = new List<EnemyBase>();

    public override void Fire(PlayerCharacter source)
    {
		// create chain
		ChainFrom(this, ChainAmount);

		// damage all enemies hit
		foreach (EnemyBase enemy in enemiesHit)
		{
			enemy.Hit(Damage, source);
		}

		OnAbilityFired(enemiesHit, source);

		// clear target list
		enemiesHit.Clear();

        lastFired = Time.GetTicksMsec();
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
			if(distance >= targetDistance && distance <= ChainRange && !enemiesHit.Contains(enemy))
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

    protected override void ResetStats()
    {
        CooldownMS = baseCooldownMS;
		ChainAmount = baseChainAmount;
		ChainRange = baseChainRange;
		Damage = baseDamage;
    }
}