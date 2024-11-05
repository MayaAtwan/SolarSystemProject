using Godot;
using System;

public partial class Earth1 : Node3D
{
	[Export] public float surfaceRadius = 50f; // Adjust as needed
	[Export] private float _surfaceGravity = 9.8f;

	public float StandardGravitationalParameter { get; private set; }

	public override void _Ready()
	{
		StandardGravitationalParameter = _surfaceGravity * surfaceRadius * surfaceRadius;
		GD.Print("Earth1 initialized with gravity parameter: " + StandardGravitationalParameter);
	}

	public Vector3 GetAccelerationAtPosition(Vector3 globalPosition)
	{
		var distanceVector = globalPosition - GlobalPosition;
		return -distanceVector.Normalized() * StandardGravitationalParameter / distanceVector.LengthSquared();
	}
}
