using Godot;
using System;

public partial class Earth1 : Node3D
{
	[Export] public float surfaceRadius = 50f; // Earth's radius
	[Export] private float _surfaceGravity = 9.8f; // Gravity at Earth's surface
	[Export] private float orbitalSpeed = 1f; // Speed in degrees per second
	[Export] private float orbitalRadius = 500f; // Radius of Earth's orbit
	[Export] private Vector3 orbitalAxis = new Vector3(0, 1, 0); // Orbital plane axis

	private float angle = 0f;

	public float StandardGravitationalParameter { get; private set; }

	public override void _Ready()
	{
		StandardGravitationalParameter = _surfaceGravity * surfaceRadius * surfaceRadius;
		//GD.Print("Earth1 initialized with gravity parameter: " + StandardGravitationalParameter);
	}

	public Vector3 GetAccelerationAtPosition(Vector3 globalPosition)
	{
		var distanceVector = globalPosition - GlobalPosition;
		if (distanceVector.LengthSquared() == 0) return Vector3.Zero; // Avoid division by zero

		return -distanceVector.Normalized() * StandardGravitationalParameter / distanceVector.LengthSquared();
	}

	public override void _Process(double delta)
	{
		angle += orbitalSpeed * (float)delta;
		angle %= 360f;

		float radians = Mathf.DegToRad(angle);
		Vector3 orbitPlane = orbitalAxis.Rotated(Vector3.Right, radians).Normalized();
		GlobalPosition = orbitPlane * orbitalRadius;

		// GD.Print($"Earth Position: {GlobalPosition}");
	}
}
