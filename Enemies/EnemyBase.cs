using Godot;
using System;
using Chickensoft.UMLGenerator;

/*
Base class for enemy units. Inherit enemy scripts from this class

Implement attack behavior by overriding the TryAttack() function.
This will be called every frame when the attack is off cooldown,
so it should also include any checks for attack condition (like if the enemy is in range to attack).

Implement navigation different from the base class by overriding the Navigate() function.
This is called every physics frame, and should include finding a target (FindTarget() function, can also be overridden)
and calling MoveTowardsTarget when necessary (this will move the enemy towards the target along a navigation path automatically)

By default an enemy has no attack and always moves towards the nearest reachable player, or the nearest if none are reachable

If you override _Process or _PhysicsProcess always call the base function as well to make sure navigation and attacking stay functional
(or implement navigation and attacking yourself)
*/
[ClassDiagram(UseVSCodePaths = true)]
public partial class EnemyBase : CharacterBody3D
{
	// base stats
	[ExportGroup("Base Stats")]
	[Export]
	protected float baseDamage = 10;
	[Export]
	protected float baseHealth = 100;
	[Export(PropertyHint.Range, "1, 10, or_greater")]
	public int Level {
		get { return _level; }
		set {
			_level = Math.Max(1, value); // minimum level is 1
			MaxHealth = baseHealth + ((_level - 1) * 0.1f * baseHealth);
			AttackDamage = baseDamage + ((_level - 1) * 0.1f * baseDamage);
			EmitSignalLevelChanged(_level);
		} }
	private int _level = 1;

	[Export(PropertyHint.Range, "5, 20")]
    protected float movementSpeed = 8.0f;

	// current stats (scaled with level)
	public float AttackDamage { get; private set; }
	public float MaxHealth{
        get { return _maxHealth; }
        set {
            float missingHealth = _maxHealth - CurrentHealth;
            _maxHealth = Math.Max(1, value); // max health is at least 1
            CurrentHealth = _maxHealth - missingHealth;
    } }
    private float _maxHealth = 100;

    public float CurrentHealth {
        get { return _currentHealth; }
        protected set {
            _currentHealth = Math.Clamp(value, 0, MaxHealth); // health is clamped between 0 and max health
            EmitSignalHealthChanged(_currentHealth, MaxHealth);
    } }
    private float _currentHealth = 100;

    [Signal]
    public delegate void HealthChangedEventHandler(float currentHealth, float maxHealth);
	[Signal]
	public delegate void LevelChangedEventHandler(int level);

	// other variables
	[ExportGroup("Navigation Agent")]
	[Export(PropertyHint.NodeType, "NavigationAgent3D")]
	protected NavigationAgent3D navAgent;
	[Export]
	protected ulong attackCooldownMS = 2000;
	protected Node3D currentTarget = null;
	protected ulong lastAttackTimeMS = 0;

	[ExportGroup("Utility")]
	[Export(PropertyHint.NodeType, "Node3D")]
	public Node3D CenterOfMass { get; private set; }

    public override void _Ready()
    {
		navAgent.VelocityComputed += MoveWithVelocity;

        base._Ready();
    }

    public override void _Process(double delta)
    {
		// tries to attack when the attack is off cooldown, if it's successful save the attack time
		if(Time.GetTicksMsec() - lastAttackTimeMS > attackCooldownMS)
		{
			bool successful = TryAttack();
			if(successful) lastAttackTimeMS = Time.GetTicksMsec();
		}
    }

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// sets new target position if the target has moved
		if (currentTarget != null && !navAgent.TargetPosition.IsEqualApprox(currentTarget.GlobalPosition))
		{
			navAgent.TargetPosition = currentTarget.GlobalPosition;
		}

		Navigate();
		
		// Applies gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}
		Velocity = velocity;
		MoveAndSlide();
	}

	public void Hit(float damage, PlayerCharacter source)
	{
		CurrentHealth -= damage;
		if( CurrentHealth <= 0)
		{
			SignalBus.Instance.EmitSignal("EnemyKilled", this, source);
		}
	}

	// handles navigation every physics frame, called once in _PhysicsProcess
	protected virtual void Navigate()
	{
		//only navigate if the map has been initialized
		if(NavigationServer3D.MapGetIterationId(navAgent.GetNavigationMap()) > 0) 
		{
			// if there is no target or it is not reachable, tries to find a new one
			if (currentTarget == null || !navAgent.IsTargetReachable()) currentTarget = FindTarget();
			// moves towards the target
			MoveTowardsTarget();
		}
	}

	// moves towards target, only call in _PhysicsProcess or method called from _PhysicsProcess (like Navigate)
	protected void MoveTowardsTarget()
	{
		// return if there is no target
		if (currentTarget == null) return;
		
		// moves towards target position if it's not reached
		if (!navAgent.IsTargetReached())
		{
			Vector3 nextPathPosition = navAgent.GetNextPathPosition();
			Vector3 newVelocity = GlobalPosition.DirectionTo(nextPathPosition) * movementSpeed;
			if (navAgent.AvoidanceEnabled) navAgent.Velocity = newVelocity;
			else MoveWithVelocity(newVelocity);
		}
	}

	// moves with the given velocity, only call in _PhysicsProcess or method called from _PhysicsProcess
	protected void MoveWithVelocity(Vector3 safeVelocity)
	{
		Velocity = safeVelocity;
		MoveAndSlide();
	}

	// targets a player, returns null if no target was found
	protected virtual Node3D FindTarget()
	{
		Node3D closest = null;
		float closestDistance = 0;
		Node3D closestReachable = null;
		float closestReachableDistance = 0;

		Vector3 currentTargetPos = navAgent.TargetPosition; // stores the current target position so it can be restored at the end

		// finds the closest player prioritizing players that are reachable
		foreach (Node p in GetTree().GetNodesInGroup("Players"))
		{
			if(p is Node3D)
			{
				Node3D player = (Node3D)p;
				navAgent.TargetPosition = player.GlobalPosition;
				float distance = navAgent.DistanceToTarget();
				bool reachable = navAgent.IsTargetReachable();
				if (reachable)
				{
					if (closestReachable == null)
					{
						closestReachable = player;
						closestReachableDistance = distance;
					}
					else if (distance < closestReachableDistance)
					{
						closestReachable = player;
						closestReachableDistance = distance;
					}
				}
				else
				{
					if(closest == null)
					{
						closest = player;
						closestDistance = distance;
					}
					else if (distance < closestDistance)
					{
						closest = player;
						closestDistance = distance;
					}
				}
			}
		}

		if(!navAgent.TargetPosition.IsEqualApprox(currentTargetPos)) navAgent.TargetPosition = currentTargetPos; // restores the target position so this function doesn't mess with active targeting

		if(closestReachable != null) return closestReachable;
		else return closest;
	}

	// tries to attack, returns true if attack successfully fired, false if not
	protected virtual bool TryAttack()
	{
		return false;
	}
}
