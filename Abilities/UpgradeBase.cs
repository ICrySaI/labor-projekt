using Godot;
using System.Collections.Generic;
using Chickensoft.UMLGenerator;

[ClassDiagram(UseVSCodePaths = true)]
public abstract partial class UpgradeBase : Resource, IInventoryItem
{
	[Export]
	public string ItemID { get; private set; }

	[Export]
	public Texture2D ItemIcon { get; set; }

	[Export]
	public string ItemName { get; set; }

	[Export(PropertyHint.MultilineText)]
	public string ItemDescription { get; set; }

	[Export]
	public string AbilityID { get; private set; }

	// applies it's multiplicative upgrades to the given ability
	public abstract void ApplyMultiplicativeUpgrades(AbilityBase a);
	// applies it's additive upgrades to the given ability
	public abstract void ApplyAdditiveUpgrades(AbilityBase a);
	// called when the ability fires, gets the enemies hit and the player who fired the ability
	public abstract void OnAbilityFired(List<EnemyBase> enemiesHit, PlayerCharacter source);
}
