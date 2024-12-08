using Godot;
using System;

public partial class Planet : StaticBody3D // StaticBody3D for planets without landing features
{
	[ExportCategory("Orbit")]
	[Export] private Planet _orbitalParent; // The parent object around which this planet orbits.

	[Export] private float _orbitalRadius; // Distance from the orbital parent.
	[Export] private float _orbitalAngle; // Angle around the orbital parent.

	[ExportCategory("Gravity")]
	[Export] public float surfaceRadius = 6.371f; // Scaled radius of the planet in kilometers.
	[Export] private float _surfaceGravity = 0.0098f; // Scaled gravity at the planet's surface.

	[ExportCategory("Rotation")]
	[Export] private float _dayLength = 24f; // Duration of a full rotation (in hours).
	[Export] private bool _tidallyLocked; // If true, the planet always shows the same face to its parent.

	// Gravitational parameter used in orbital and gravity calculations.
	public float StandardGravitationalParameter { get; private set; }
	// Linear velocity of the planet's orbit.
	public Vector3 ConstantLinearVelocity { get; private set; }
	// Angular velocity of the planet's rotation.
	public Vector3 ConstantAngularVelocity { get; private set; }

	public override void _Ready()
	{
		// Calculate the standard gravitational parameter: GM = g * r^2.
		StandardGravitationalParameter = _surfaceGravity * surfaceRadius * surfaceRadius;
	}

	// Initialize the planet's position and velocities based on orbital parent.
	public void Init()
	{
		if (_orbitalParent != null)
		{
			// Set the global position based on the orbital radius and angle.
			GlobalPosition = _orbitalParent.GlobalPosition + 
				(Vector3.Forward * _orbitalRadius).Rotated(Vector3.Up, _orbitalAngle);

			// Calculate the linear velocity by combining the parent's velocity and orbital velocity.
			ConstantLinearVelocity = _orbitalParent.ConstantLinearVelocity + 
				_orbitalParent.GetOrbitalVelocity(GlobalPosition);
		}

		// If tidally locked, the day length matches the orbital period.
		if (_tidallyLocked)
		{
			_dayLength = GetOrbitalPeriod();
		}

		// Calculate the angular velocity for rotation.
		ConstantAngularVelocity = -(_dayLength == 0 ? 0 : 2f * Mathf.Pi / _dayLength) * Vector3.Up;
		GD.Print($"Initialized {Name}");
	}

	// Calculate the gravitational acceleration at a given position.
	public Vector3 GetAccelerationAtPosition(Vector3 globalPosition)
	{
		var distanceVector = globalPosition - GlobalPosition; // Vector from the planet to the target position.

		// Debug logs to monitor calculations.
		GD.Print($"[Debug] {Name} - Position: {GlobalPosition}, Target: {globalPosition}");
		GD.Print($"[Debug] {Name} - Distance Vector: {distanceVector}, Length: {distanceVector.Length()}");

		// Prevent division by zero for objects at the planet's exact position.
		if (distanceVector.LengthSquared() == 0)
		{
			return Vector3.Zero;
		}

		// Calculate gravitational force: F = -GM/r^2 * direction.
		var gravity = -distanceVector.Normalized() * StandardGravitationalParameter / distanceVector.LengthSquared();
		GD.Print($"[Debug] {Name} - Calculated Gravity: {gravity}");
		return gravity;
	}

	// Calculate the orbital velocity at a given position.
	public Vector3 GetOrbitalVelocity(Vector3 globalPosition)
	{
		var distanceVector = globalPosition - GlobalPosition; // Vector from the planet to the target position.
		var speed = Mathf.Sqrt(StandardGravitationalParameter / distanceVector.Length()); // Orbital speed using v = sqrt(GM/r).
		var direction = distanceVector.Normalized().Cross(Vector3.Up); // Perpendicular direction for circular orbit.
		return direction * speed;
	}

	// Calculate the orbital period using Kepler's Third Law.
	public float GetOrbitalPeriod()
	{
		return _orbitalParent == null ? 0 : 
			2f * Mathf.Pi * Mathf.Sqrt(Mathf.Pow(_orbitalRadius, 3) / _orbitalParent.StandardGravitationalParameter);
	}

	// Update the planet's position and rotation during each physics frame.
	public override void _PhysicsProcess(double delta)
	{
		if (_orbitalParent != null)
		{
			// Apply gravitational pull from the orbital parent.
			var gravity = _orbitalParent.GetAccelerationAtPosition(GlobalPosition);
			ConstantLinearVelocity += gravity * (float)delta; // Update linear velocity.
			GlobalPosition += ConstantLinearVelocity * (float)delta; // Update position.
		}

		// Apply rotational velocity to simulate day-night cycles.
		GlobalRotation += ConstantAngularVelocity * (float)delta;
	}
}
