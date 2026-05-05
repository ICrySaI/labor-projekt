using Godot;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Inventory : Node3D
{
	[Export(PropertyHint.NodeType, "RigidBody3D")]
	PlayerCharacter player; // the player the inventory belongs to

	[Export(PropertyHint.NodeType, "GridContainer")]
	private GridContainer grid;

	private List<(AbilityBase ability, AbilityIconUI icon)> abilities = new List<(AbilityBase, AbilityIconUI)>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PackedScene lightningAbilityScene = GD.Load<PackedScene>("res://Abilities/LightningAbility/LightningAbility.tscn");
		GD.Load<LightningDamageUpgrade>("res://Abilities/LightningAbility/Upgrades/LightningDamageUpgrade.tres");
		// ------testing abilities------ //
		LightningAbility test1 = lightningAbilityScene.Instantiate<LightningAbility>();
		test1.AddUpgrade(GD.Load<LightningDamageUpgrade>("res://Abilities/LightningAbility/Upgrades/LightningDamageUpgrade.tres"));
		addAbility(test1);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		ulong currentTime = Time.GetTicksMsec();
		// fires every ability that's ready, and updates ability icons
		foreach (var a in abilities)
		{
			int timeSinceFired = (int)(currentTime - a.ability.lastFired);
			a.icon.MaxValue = a.ability.CooldownMS;
			a.icon.Value = timeSinceFired;

			if(timeSinceFired > a.ability.CooldownMS)
			{
				a.ability.Fire(player); // fires the ability
			}
		}
	}

	// adds an ability to the inventory, this places it in the scene tree and creates an icon for it as well
	public void addAbility(AbilityBase ability)
	{
		// if this ability is already in the list, we do not add it
		if(abilities.Where(a => a.ability == ability).Count() > 0) return;

		AbilityIconUI newIcon = new AbilityIconUI(ability); // we create a new icon for the ability
		abilities.Add((ability, newIcon)); // add the ability to the list

		// add the ability as a child node, and the icon as a child of the icon grid
		AddChild(ability);
		grid.AddChild(newIcon);
	}

	// remove the ability from the inventory, this removes in from the scene tree along with it's corresponding icon
	public void removeAbility(AbilityBase ability)
	{
		//finds the corresponding entry in the list
		var a = abilities.SingleOrDefault(a => a.ability == ability);
		if(a == default) return; // returns if there's no such ability in the list

		// removes the ability from the list, and from the scene tree (along with the icon)
		abilities.Remove(a);
		a.ability.QueueFree();
		a.icon.QueueFree();
		
	}

	// return a list of all items in the inventory, this includes abilities and upgrades on those abilities
	public List<IInventoryItem> GetItems()
	{
		List<IInventoryItem> list = new List<IInventoryItem>();
		foreach (var a in abilities)
		{
			AbilityBase ability = a.ability;
			list.Add(ability);
			foreach (UpgradeBase upgrade in ability.upgrades)
			{
				list.Add(upgrade);
			}
		}
		return list;
	}
}
