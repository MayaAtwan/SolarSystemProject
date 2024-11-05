using Godot;
using System;

public partial class Planet : StaticBody3D // StaticBody3D for planets without landing features
{
	[ExportCategory("Orbit")]
	[Export] private Planet _orbitalParent;

	[Export] private float _orbitalRadius;
	[Export] private float _orbitalAngle;

	[ExportCategory("Gravity")]
	[Export] public float surfaceRadius = 50f; // Adjust for each planet's approximate radius
	[Export] private float _surfaceGravity = 9.8f;

	[ExportCategory("Rotation")]
	[Export] private float _dayLength = 24f;
	[Export] private bool _tidallyLocked;

	public float StandardGravitationalParameter { get; private set; }
	public Vector3 ConstantLinearVelocity { get; private set; }
	public Vector3 ConstantAngularVelocity { get; private set; }

	public override void _Ready()
	{
		StandardGravitationalParameter = _surfaceGravity * surfaceRadius * surfaceRadius;
	}

	public void Init()
	{
		if (_orbitalParent != null)
		{
			GlobalPosition = _orbitalParent.GlobalPosition + 
				(Vector3.Forward * _orbitalRadius).Rotated(Vector3.Up, _orbitalAngle);
			ConstantLinearVelocity = _orbitalParent.ConstantLinearVelocity + 
				_orbitalParent.GetOrbitalVelocity(GlobalPosition);
		}

		if (_tidallyLocked)
		{
			_dayLength = GetOrbitalPeriod();
		}

		ConstantAngularVelocity = -(_dayLength == 0 ? 0 : 2f * Mathf.Pi / _dayLength) * Vector3.Up;
		GD.Print($"Initialized {Name}");
	}

	public Vector3 GetAccelerationAtPosition(Vector3 globalPosition)
	{
		var distanceVector = globalPosition - GlobalPosition;
		return -distanceVector.Normalized() * StandardGravitationalParameter / distanceVector.LengthSquared();
	}

	public Vector3 GetOrbitalVelocity(Vector3 globalPosition)
	{
		var distanceVector = globalPosition - GlobalPosition;
		var speed = Mathf.Sqrt(StandardGravitationalParameter / distanceVector.Length());
		var direction = distanceVector.Normalized().Cross(Vector3.Up);
		return direction * speed;
	}

	public float GetOrbitalPeriod()
	{
		return _orbitalParent == null ? 0 : 
			2f * Mathf.Pi * Mathf.Sqrt(Mathf.Pow(_orbitalRadius, 3) / _orbitalParent.StandardGravitationalParameter);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_orbitalParent != null)
		{
			var gravity = _orbitalParent.GetAccelerationAtPosition(GlobalPosition);
			ConstantLinearVelocity += gravity * (float)delta;
			GlobalPosition += ConstantLinearVelocity * (float)delta;
		}

		GlobalRotation += ConstantAngularVelocity * (float)delta;
	}
}
