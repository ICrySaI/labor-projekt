using Godot;
using System;
using System.ComponentModel;
using System.Diagnostics;

public partial class MeleeEnemyAttack : MeshInstance3D
{
	private Timer timer = new Timer();
	private Node source;
	private float range;
	private SphereMesh mesh = new SphereMesh();
	private bool timerComplete = false;

	public MeleeEnemyAttack(Node source, float range) : base()
	{
		this.source = source;
        this.range = range;

		timer.OneShot = true;
		AddChild(timer);

		mesh.Radius = 0;
		mesh.Height = 0;
    }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer.Connect("timeout", Callable.From(CompleteAttack));
		Mesh = mesh;
		MaterialOverride = GD.Load<Material>("res://Enemies/MeleeEnemy/MeleeAttackProgressMaterial.tres");
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
		if(timerComplete) GetParent().RemoveChild(this);
		else
		{
			timerComplete = true;
			MaterialOverride = GD.Load<Material>("res://Enemies/MeleeEnemy/MeleeAttackActiveMaterial.tres");
			timer.Start(0.1);

			//---------- checks hits on players ----------

			// creates shape cast
			SphereShape3D hitShape = new SphereShape3D();
			hitShape.Radius = range;
			ShapeCast3D hitDetector = new ShapeCast3D();
			hitDetector.Shape = hitShape;
			hitDetector.TargetPosition = Vector3.Zero;
			AddChild(hitDetector);
			hitDetector.ForceShapecastUpdate();
			// look for players within shape
			for(int i = 0; i < hitDetector.GetCollisionCount(); i++)
			{
				GodotObject collider = hitDetector.GetCollider(i);
				if(collider is Node3D && ((Node3D)collider).IsInGroup("Players"))
				{
					Debug.Print("player hit!");
				}
			}

		}
	}
}
