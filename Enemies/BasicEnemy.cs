using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class BasicEnemy : CharacterBody3D
{
	[Export]
    public float MovementSpeed { get; set; } = 4.0f;

	private Node3D currentTarget = null;

	private NavigationAgent3D navAgent;

    public override void _Ready()
    {
		navAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
		navAgent.VelocityComputed += MoveWithVelocity;

        base._Ready();
    }


	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		//calculate navigation
		if(NavigationServer3D.MapGetIterationId(navAgent.GetNavigationMap()) > 0) //only navigate if the map has been initialized
		{
			// if there is no target, tries to find one
			if (currentTarget == null) currentTarget = FindTarget();

			// if a target was found moves towards it
			if (currentTarget != null)
			{
				// sets new target position if the target has moved, and tries to find a new target if it is no longer reachable
				if (!navAgent.TargetPosition.IsEqualApprox(currentTarget.GlobalPosition))
				{
					navAgent.TargetPosition = currentTarget.GlobalPosition;
					if (!navAgent.IsTargetReachable())
					{
						currentTarget = FindTarget();
						if (currentTarget != null) navAgent.TargetPosition = currentTarget.GlobalPosition;
					}
				}
				// moves towards target position if it's not reached
				if (!navAgent.IsTargetReached())
				{
					Vector3 nextPathPosition = navAgent.GetNextPathPosition();
					Vector3 newVelocity = GlobalPosition.DirectionTo(nextPathPosition) * MovementSpeed;
					if (navAgent.AvoidanceEnabled) navAgent.Velocity = newVelocity;
					else MoveWithVelocity(newVelocity);
				}
			}
		}

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}
		Velocity = velocity;
		MoveAndSlide();
	}

	private void MoveWithVelocity(Vector3 safeVelocity)
	{
		Velocity = safeVelocity;
		MoveAndSlide();
	}

	// targets a player, returns null if no target was found
	private Node3D FindTarget()
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
				Debug.Print("playerfound");
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
}
