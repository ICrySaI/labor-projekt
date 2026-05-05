using Godot;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

public partial class Globals : Node
{
    public static Globals Instance { get; private set; }

    // the list of enemies that can be instanced
    public static Godot.Collections.Array<PackedScene> EnemyRepository = new Godot.Collections.Array<PackedScene>(){
        GD.Load<PackedScene>("res://Enemies/MeleeEnemy/MeleeEnemy.tscn"),
        GD.Load<PackedScene>("res://Enemies/RangedEnemy/RangedEnemy.tscn")
    };

    // the list of abilities that can be instanced
    public static Godot.Collections.Array<PackedScene> AbilityRepository = new Godot.Collections.Array<PackedScene>(){
        GD.Load<PackedScene>("res://Abilities/LightningAbility/LightningAbility.tscn")
    };

    public override void _Ready()
    {
        Instance = this;
        base._Ready();
    }
}
