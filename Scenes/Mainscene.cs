using Godot;
using System;

public partial class Mainscene : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
    {
        // capture mouse when game window is clicked, release with escape
        if(@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left) Input.MouseMode = Input.MouseModeEnum.Captured;
        if(@event.IsActionPressed("ui_cancel")) Input.MouseMode = Input.MouseModeEnum.Visible;


        base._Input(@event);
    }

    public override void _Notification(int what)
    {
		// release mouse on focus loss
		if(what == NotificationApplicationFocusOut)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
    }

}
