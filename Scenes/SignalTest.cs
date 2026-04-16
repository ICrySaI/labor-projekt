using Godot;
using System;
using System.Diagnostics;

public partial class SignalTest : Node
{
	public void HealthChanged(float current, float max)
	{
		Debug.Print("signal received");
	}
}
