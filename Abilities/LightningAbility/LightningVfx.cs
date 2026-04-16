using Godot;
using System;

public partial class LightningVfx : Node3D
{
	[Export(PropertyHint.NodeType, "GPUParticles3D")]
	private GpuParticles3D lightningParticles;
	[Export(PropertyHint.NodeType, "AnimationPlayer")]
	private AnimationPlayer animationPlayer;

	public void TriggerEffect(Vector3 from, Vector3 to)
	{
		LookAtFromPosition(from, to, Vector3.Up);
		ScaleObjectLocal(new Vector3(1, 1, from.DistanceTo(to)));
		animationPlayer.Play("LightningVFX");
		GetTree().CreateTimer(0.4, false).Timeout += () => QueueFree(); // sets a timer to remove the node after the animation plays
	}
}
