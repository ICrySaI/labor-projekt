using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class AbilityBase : Node3D
{
	[Export]
	public Texture2D IconTexture { get; private set; }
	[Export]
	public string AbilityName { get; private set; }
	[Export(PropertyHint.MultilineText)]
	public string AbilityDescription { get; private set; }
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

	// fires the ability at a specific position
	public virtual void Fire()
	{
		lastFired = Time.GetTicksMsec();
	}
}
