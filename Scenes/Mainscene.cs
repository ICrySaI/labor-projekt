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
        // capture mouse when game window is clicked and the game is not paused
        if(@event is InputEventMouseButton mouseEvent)
		{
			if(mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left && !GetTree().Paused)
			{
				Input.MouseMode = Input.MouseModeEnum.Captured;
			}
		}

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
