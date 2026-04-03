using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class AbilityBase : Node
{
	[Export]
	public int CooldownMS {
		get { return _cooldownMS; }
		set {
			_cooldownMS = value;
			EmitSignalCooldownChanged();	
	} }
	private int _cooldownMS = 1000;

	[Signal]
	public delegate void CooldownChangedEventHandler();

	public ulong lastFired { get; private set; } = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// fires the ability at a specific position
	public virtual void Fire(Vector3 at)
	{
		lastFired = Time.GetTicksMsec();
	}
}
