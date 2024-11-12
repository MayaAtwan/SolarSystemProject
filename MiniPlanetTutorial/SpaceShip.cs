using Godot;
using System;

public partial class SpaceShip : RigidBody3D
{
	[Export] private float thrust = 20f;
	[Export] private float landingDistanceThreshold = 150f;
	[Export] private float landingSpeed = 1f;

	private Earth1 earthNode;
	private bool _controlEnabled = true;
	private bool _isLanding = false;
	private bool _isLandingCompleted = false;
	private Vector3 _targetLandingPosition;

	public override void _Ready()
	{
		GD.Print("SpaceShip Ready");

		var solarSystem = GetParent<SolarSystem>();
		if (solarSystem != null)
		{
			solarSystem.Connect("InitializationComplete", new Callable(this, nameof(OnSolarSystemInitialized)));
		}
		else
		{
			GD.PrintErr("SolarSystem parent not found for SpaceShip.");
		}
	}

	private void OnSolarSystemInitialized()
	{
		GD.Print("SolarSystem initialization complete. Setting EarthNode in SpaceShip...");
		var solarSystem = GetParent<SolarSystem>();
		earthNode = solarSystem?.EarthNode;
		GD.Print("Earth node set in SpaceShip: ", earthNode != null);
	}

	private void ProcessSpaceshipMovement(double delta)
	{
		if (!_controlEnabled || _isLanding || _isLandingCompleted) return;

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
		GD.Print("Earth node set in SpaceShip.");
	}

	public override void _Process(double delta)
	{
		if (_controlEnabled && !_isLanding && !_isLandingCompleted)
		{
			ProcessSpaceshipMovement(delta);
			CheckProximityToEarth();
		}
		else if (_isLanding && !_isLandingCompleted)
		{
			ApproachLandingPosition((float)delta);
		}
	}

	private void ApproachLandingPosition(float delta)
	{
		if (_isLandingCompleted) return;

		GlobalPosition = GlobalPosition.Lerp(_targetLandingPosition, landingSpeed * delta);
		GD.Print("Approaching landing position. Current Position:", GlobalPosition);

		if (GlobalPosition.DistanceTo(_targetLandingPosition) < 0.05f && LinearVelocity.Length() < 0.01f)
		{
			GD.Print("Landed on Earth successfully at Final Position:", GlobalPosition);
			GlobalPosition = _targetLandingPosition;
			LinearVelocity = Vector3.Zero;
			AngularVelocity = Vector3.Zero;
			_isLandingCompleted = true;
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
		_isLanding = true;

		LinearVelocity = Vector3.Zero;
		AngularVelocity = Vector3.Zero;

		Vector3 directionToEarth = (earthNode.GlobalTransform.Origin - GlobalTransform.Origin).Normalized();
		_targetLandingPosition = earthNode.GlobalTransform.Origin + directionToEarth * (earthNode.surfaceRadius + 1.0f);

		GD.Print("Calculated target landing position:", _targetLandingPosition);
		GD.Print("Earth's position:", earthNode.GlobalTransform.Origin);
		GD.Print("Distance from Earth's surface:", _targetLandingPosition.DistanceTo(earthNode.GlobalTransform.Origin));
	}

	public void ResetSpaceShip()
	{
		GD.Print("Resetting SpaceShip...");
		_controlEnabled = true;
		_isLanding = false;
		_isLandingCompleted = false;

		GlobalPosition = Vector3.Zero;
		LinearVelocity = Vector3.Zero;
		AngularVelocity = Vector3.Zero;

		GD.Print("SpaceShip reset completed.");
	}
}
