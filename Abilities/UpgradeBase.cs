using Godot;
using System;
using System.Collections.Generic;

public abstract partial class UpgradeBase : Resource
{
	[Export]
	public Texture2D IconTexture { get; private set; }

	[Export]
	public string UpgradeName { get; private set; }

	[Export(PropertyHint.MultilineText)]
	public string UpgradeDescription { get; private set; }

	// applies it's multiplicative upgrades to the given ability
	public abstract void ApplyMultiplicativeUpgrades(AbilityBase a);
	// applies it's additive upgrades to the given ability
	public abstract void ApplyAdditiveUpgrades(AbilityBase a);
	// called when the ability fires, gets the enemies hit and the player who fired the ability
	public abstract void OnAbilityFired(List<EnemyBase> enemiesHit, PlayerCharacter source);
}
