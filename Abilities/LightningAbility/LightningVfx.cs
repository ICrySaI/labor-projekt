using Godot;
using System;
using System.Collections;

public partial class LightningVfx : Node3D
{
	[Export(PropertyHint.NodeType, "GPUParticles3D")]
	private GpuParticles3D lightningParticles;
	[Export(PropertyHint.NodeType, "AnimationPlayer")]
	private AnimationPlayer animationPlayer;

	Node3D from;
	Node3D to;

	// triggers the effect between two nodes, the effect is automatically deleted after it finishes or if one of the endpoints are removed
	public void TriggerEffect(Node3D from, Node3D to)
	{
		this.from = from;
		this.to = to;
		setPosition();
		animationPlayer.Play("LightningVFX");
		GetTree().CreateTimer(0.4, false).Timeout += () => QueueFree(); // sets a timer to remove the node after the animation plays
	}

    public override void _Process(double delta)
    {
		setPosition();
        base._Process(delta);
    }

	private void setPosition()
	{
		if(from != null && to != null)
		{
			LookAtFromPosition(from.GlobalPosition, to.GlobalPosition, Vector3.Up);
			Scale = new Vector3(1, 1, from.GlobalPosition.DistanceTo(to.GlobalPosition));
		}
		else
		{
			QueueFree();
		}
	}
}
