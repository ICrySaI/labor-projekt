using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.Marshalling;
using Godot;

public abstract partial class AbilityBase : Node3D
{
	[Export]
	public Texture2D IconTexture { get; private set; }

	[Export]
	public string AbilityName { get; private set; }

	[Export(PropertyHint.MultilineText)]
	public string AbilityDescription { get; private set; }

	[Export]
	public int baseCooldownMS { get; private set; } = 1000;
	public int CooldownMS {
		get { return _cooldownMS; }
		set {
			_cooldownMS = value;	
	} }
	private int _cooldownMS;

	public List<UpgradeBase> upgrades = new List<UpgradeBase>();

	public ulong lastFired { get; protected set; } = 0;

	public AbilityBase()
	{
		// constructor sets stats to their base values
		ResetStats();
	}

	// updates the stats to reflect the applied upgrades
	private void UpdateStats()
	{
		// reset stats to their base values
		ResetStats();
		// apply additive upgrades
		foreach (UpgradeBase upgrade in upgrades)
		{
			upgrade.ApplyAdditiveUpgrades(this);
		}
		// apply multiplicative upgrades
		foreach (UpgradeBase upgrade in upgrades)
		{
			upgrade.ApplyMultiplicativeUpgrades(this);
		}
	}

	// resets the stats to their base values
	protected abstract void ResetStats();

	public void AddUpgrade(UpgradeBase upgrade)
	{
		upgrades.Add(upgrade);
		UpdateStats();
	}

	public void RemoveUpgrade(UpgradeBase upgrade)
	{
		upgrades.Remove(upgrade);
		UpdateStats();
	}

	// fires the ability with a given player as source
	public abstract void Fire(PlayerCharacter source);

	protected void OnAbilityFired(List<EnemyBase> enemiesHit, PlayerCharacter source)
	{
		foreach (UpgradeBase upgrade in upgrades)
		{
			upgrade.OnAbilityFired(enemiesHit, source);
		}
	}
}
