using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

public partial class RangedEnemy : EnemyBase
{
	[Export(PropertyHint.NodeType, "Node3D")]
	private Node3D head;
	[Export]
	private float attackRange = 20.0f;
	[Export]
	private float preferredDistance = 10.0f;

    protected override void Navigate()
    {
		//only navigate if the map has been initialized
		if(NavigationServer3D.MapGetIterationId(navAgent.GetNavigationMap()) > 0) 
		{
			// if there is no target or it is not visible, tries to find a new one
			if (currentTarget == null || !IsTargetVisible(currentTarget)) currentTarget = FindTarget();
			if(currentTarget == null) return; // returns if no target was found

			// moves towards target if it's not visible or further away than preferred distance.
			float distance = navAgent.DistanceToTarget();
			if(distance > preferredDistance + 1 || !IsTargetVisible(currentTarget)) MoveTowardsTarget();
			else if(distance < preferredDistance - 1) // if target is visible and closer than preferred distance, move directly away from target 
			{
				
				Vector3 direction = GlobalPosition.DirectionTo(currentTarget.GlobalPosition) * -1;
				MoveWithVelocity(direction * movementSpeed);
			}
		}
        
    }

	protected override bool TryAttack()
	{
		return false;
	}

    protected override Node3D FindTarget()
    {
        Node3D closest = null;
		float closestDistance = 0;
		Node3D closestVisible = null;
		float closestVisibleDistance = 0;

		Vector3 currentTargetPos = navAgent.TargetPosition; // stores the current target position so it can be restored at the end

		// finds the closest player prioritizing players that are visible
		foreach (Node p in GetTree().GetNodesInGroup("Players"))
		{
			if(p is Node3D)
			{
				Node3D player = (Node3D)p;
				navAgent.TargetPosition = player.GlobalPosition;
				float distance = navAgent.DistanceToTarget();
				bool visible = IsTargetVisible(player);
				if (visible)
				{
					if (closestVisible == null)
					{
						closestVisible = player;
						closestVisibleDistance = distance;
					}
					else if (distance < closestVisibleDistance)
					{
						closestVisible = player;
						closestVisibleDistance = distance;
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

		if(closestVisible != null) return closestVisible;
		else return closest;
    }

	private bool IsTargetVisible(Node3D target)
	{
		// sets up a raycast from head to target
		Vector3 targetPosition = target.GlobalPosition;
		targetPosition.Y += 1;

		RayCast3D rayCast = new RayCast3D();
		AddChild(rayCast);

		rayCast.Position = head.Position;
		rayCast.ExcludeParent = true;
		rayCast.TargetPosition = rayCast.ToLocal(targetPosition);
		rayCast.ForceRaycastUpdate();

		// gets result of raycast
		bool result = rayCast.GetCollider() == target;

		// removes raycast
		RemoveChild(rayCast);

		//if(result) Debug.Print("visible");
		//else Debug.Print("not visible");

		return result; // if there is nothing in the result there was no collision, target is visible
	}
}
