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

    public static bool mouseCaptured { get; private set; } = false;

    public static void CaptureMouse()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        mouseCaptured = true;
    }

    public static void ReleaseMouse()
    {
        Input.MouseMode = Input.MouseModeEnum.Visible;
        mouseCaptured = false;
    }

    public override void _Ready()
    {
        Instance = this;
        base._Ready();
    }

    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseButton mouseEvent)
		{
            // when the window is clicked, if the game is in mouse capture mode it captures the mouse
			if(mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left && mouseCaptured)
			{
				Input.MouseMode = Input.MouseModeEnum.Captured;
			}
		}

        base._Input(@event);
    }

    public override void _Notification(int what)
    {
		// focus loss always releases the mouse, but doesn't changed the captured mode
		if(what == NotificationApplicationFocusOut)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
    }
}
