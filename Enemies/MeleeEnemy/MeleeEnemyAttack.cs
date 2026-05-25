using Godot;
using Chickensoft.UMLGenerator;

[ClassDiagram(UseVSCodePaths = true)]
public partial class MeleeEnemyAttack : MeshInstance3D
{
	// attack stats
	private Node source;
	private float range;
	private float damage;

	private Timer timer;
	private SphereMesh mesh;
	private bool timerComplete = false;

	public MeleeEnemyAttack(Node source, float range, float damage) : base()
	{
		this.source = source;
        this.range = range;
		this.damage = damage;

        timer = new Timer{ OneShot = true };
        AddChild(timer);

        mesh = new SphereMesh{ Radius = 0, Height = 0 };
    }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer.Timeout += CompleteAttack;
		Mesh = mesh;
		MaterialOverride = GD.Load<Material>("res://Enemies/AttackWarningMaterial.tres");
		timer.Start(0.5);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!timerComplete)
		{
			// interpolate from zero to the radius over the duration of the timer
			float t = (float)(1 - (timer.TimeLeft / timer.WaitTime));
			mesh.Radius = t * range;
			mesh.Height = mesh.Radius * 2;	
		}
		
	}

	public void CompleteAttack()
	{
		// removes itself from scene tree when done
		if(timerComplete) QueueFree();
		else
		{
			timerComplete = true;
			MaterialOverride = GD.Load<Material>("res://Enemies/AttackActiveMaterial.tres");
			timer.Start(0.1);

            //---------- checks hits on players ----------

            // creates shape cast
            SphereShape3D hitShape = new SphereShape3D{ Radius = range };
            ShapeCast3D hitDetector = new ShapeCast3D{ Shape = hitShape, TargetPosition = Vector3.Zero };
            AddChild(hitDetector);
			hitDetector.ForceShapecastUpdate();
			// look for players within shape
			for(int i = 0; i < hitDetector.GetCollisionCount(); i++)
			{
				GodotObject collider = hitDetector.GetCollider(i);
				if(collider is PlayerCharacter && ((PlayerCharacter)collider).IsInGroup("Players"))
				{
					((PlayerCharacter)collider).CurrentHealth -= damage;
				}
			}

		}
	}
}
