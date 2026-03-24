using Godot;

public partial class RangedAttackBullet : MeshInstance3D
{
	Node source;
	private SphereMesh mesh;
	private SphereShape3D hitShape;
	private ShapeCast3D hitDetector;
	private float speed;
	private Vector3 direction;
	private float damage;
	private float range;
	private Vector3 startPostition;

	public RangedAttackBullet(Node source, float radius, float speed, Vector3 direction, float damage, float range) : base()
	{
		this.source = source;
		mesh = new SphereMesh{ Radius = radius, Height = radius * 2 };
		hitShape = new SphereShape3D{ Radius = radius };
		hitDetector = new ShapeCast3D{ Shape = hitShape, TargetPosition = Vector3.Zero };
		hitDetector.AddException((CollisionObject3D)source);
		this.speed = speed;
		this.direction = direction.IsNormalized() ? direction : direction.Normalized();
		this.damage = damage;
		this.range = range;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Mesh = mesh;
		MaterialOverride = GD.Load<Material>("res://Enemies/AttackActiveMaterial.tres");
		startPostition = GlobalPosition;
        AddChild(hitDetector);
	}

	public override void _PhysicsProcess(double delta)
	{
		// checks if we hit something
		for(int i = 0; i < hitDetector.GetCollisionCount(); i++)
		{
			GodotObject collider = hitDetector.GetCollider(i);
			if(collider is PlayerCharacter && ((PlayerCharacter)collider).IsInGroup("Players"))
			{
				((PlayerCharacter)collider).CurrentHealth -= damage;
			}
			QueueFree(); // if we hit something, queue the bullet to be removed
		}

		// moves the bullet
		GlobalPosition += direction * speed * (float)delta;

		// if we're over the maximum range, queue the bullet to be removed
		if(startPostition.DistanceTo(GlobalPosition) > range) QueueFree();
	}
}
