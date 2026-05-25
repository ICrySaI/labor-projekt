using Godot;
using Chickensoft.UMLGenerator;

[ClassDiagram(UseVSCodePaths = true)]
public partial class LightningVfx : Node3D
{
	[Export(PropertyHint.NodeType, "GPUParticles3D")]
	private GpuParticles3D lightningParticles;
	[Export(PropertyHint.NodeType, "AnimationPlayer")]
	private AnimationPlayer animationPlayer;

	Node3D from;
	Vector3 startPoint;
	Node3D to;
	Vector3 endPoint;

	// triggers the effect between two nodes, the effect is automatically deleted after it finishes
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

	// sets the start and endpoints of the effect to the postitions of the from and to nodes if they're still valid
	private void setPosition()
	{
		if(IsInstanceValid(from))
		{
			startPoint = from.GlobalPosition;
		}
		if (IsInstanceValid(to))
		{
			endPoint = to.GlobalPosition;
		}
		LookAtFromPosition(startPoint, endPoint, Vector3.Up);
		Scale = new Vector3(1, 1, startPoint.DistanceTo(endPoint));
	}
}
