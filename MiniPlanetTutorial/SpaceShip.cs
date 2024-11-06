using Godot;
using System;

public partial class SpaceShip : RigidBody3D
{
	[Export] private float thrust = 20f;
	[Export] private float landingDistanceThreshold = 150f;
	[Export] private float landingSpeed = 1f;

	private Earth1 earthNode;
	private bool _controlEnabled = true;
	private bool _isLandedOnEarth = false;
	private Vector3 _targetLandingPosition;

	public override void _Ready()
	{
		GD.Print("Spaceship Ready");
		var solarSystem = GetParent() as SolarSystem;

		if (solarSystem == null)
		{
			GD.PrintErr("SolarSystem parent not found.");
		}
		else
		{
			earthNode = solarSystem.EarthNode;
			if (earthNode == null)
			{
				GD.PrintErr("Earth node not found in SolarSystem.");
			}
		}
	}

	private void ProcessSpaceshipMovement(double delta)
	{
		if (!_controlEnabled || _isLandedOnEarth) return;

		Vector3 movement = Vector3.Zero;
		Vector3 forward = -GlobalTransform.Basis.Z;
		Vector3 left = -GlobalTransform.Basis.X;
		Vector3 up = GlobalTransform.Basis.Y;

		if (Input.IsActionPressed("Forward")) movement += forward;
		if (Input.IsActionPressed("Backward")) movement -= forward;
		if (Input.IsActionPressed("Left")) movement += left;
		if (Input.IsActionPressed("Right")) movement -= left;
		if (Input.IsActionPressed("Up")) movement += up;
		if (Input.IsActionPressed("Down")) movement -= up;

		GlobalPosition += movement * thrust * (float)delta;
	}

	public void SetEarthNode(Earth1 earth)
	{
		earthNode = earth;
	}

	public override void _Process(double delta)
	{
		if (_controlEnabled && !_isLandedOnEarth)
		{
			ProcessSpaceshipMovement(delta);
			CheckProximityToEarth();
		}
		else if (_isLandedOnEarth)
		{
			ApproachLandingPosition((float)delta);
		}
	}

	private void ApproachLandingPosition(float delta)
	{
		GlobalPosition = GlobalPosition.Lerp(_targetLandingPosition, landingSpeed * delta);

		GD.Print("Approaching landing position. Current Position:", GlobalPosition);

		if (GlobalPosition.DistanceTo(_targetLandingPosition) < 0.05f)
		{
			GD.Print("Landed on Earth successfully.");
			GD.Print("Final Landing Position:", GlobalPosition);
			GlobalPosition = _targetLandingPosition;
			SetPhysicsProcess(false);
		}
	}

	private void CheckProximityToEarth()
	{
		if (earthNode == null)
		{
			GD.Print("Earth node is null, cannot check proximity.");
			return;
		}

		float distanceToEarth = GlobalTransform.Origin.DistanceTo(earthNode.GlobalTransform.Origin);
		GD.Print("Distance to Earth:", distanceToEarth);

		if (distanceToEarth <= landingDistanceThreshold)
		{
			GD.Print("Close enough to Earth, ready to land!");
			if (Input.IsActionJustPressed("Land"))
			{
				LandOnEarth();
			}
		}
		else
		{
			GD.Print("Not within landing distance of Earth.");
		}
	}

	private void LandOnEarth()
	{
		GD.Print("Landing on Earth...");

		_controlEnabled = false;
		_isLandedOnEarth = true;

		LinearVelocity = Vector3.Zero;
		AngularVelocity = Vector3.Zero;

		Vector3 directionToEarth = (earthNode.GlobalTransform.Origin - GlobalTransform.Origin).Normalized();
		_targetLandingPosition = earthNode.GlobalTransform.Origin + directionToEarth * (earthNode.surfaceRadius + 1.0f);

		GD.Print("Calculated target landing position:", _targetLandingPosition);
		GD.Print("Earth's position:", earthNode.GlobalTransform.Origin);
		GD.Print("Distance from Earth's surface:", _targetLandingPosition.DistanceTo(earthNode.GlobalTransform.Origin));
	}
}
