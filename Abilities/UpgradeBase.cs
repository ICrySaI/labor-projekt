using Godot;
using System;
using System.Collections.Generic;

public abstract partial class UpgradeBase : Resource, IInventoryItem
{
	[Export]
	public Texture2D ItemIcon { get; set; }

	[Export]
	public string ItemName { get; set; }

	[Export(PropertyHint.MultilineText)]
	public string ItemDescription { get; set; }

	// applies it's multiplicative upgrades to the given ability
	public abstract void ApplyMultiplicativeUpgrades(AbilityBase a);
	// applies it's additive upgrades to the given ability
	public abstract void ApplyAdditiveUpgrades(AbilityBase a);
	// called when the ability fires, gets the enemies hit and the player who fired the ability
	public abstract void OnAbilityFired(List<EnemyBase> enemiesHit, PlayerCharacter source);
}
