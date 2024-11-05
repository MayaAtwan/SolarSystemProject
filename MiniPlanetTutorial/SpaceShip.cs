using Godot;
using System;

public partial class SpaceShip : RigidBody3D
{
	[Export] private float thrust = 7f;
	[Export] private StaticBody3D earthNode;
	[Export] private float landingDistanceThreshold = 150f;
	[Export] private PackedScene earthScene; 

	private bool _controlEnabled = true;
	private bool _isLandedOnEarth = false;

	public override void _Ready()
	{
		GD.Print("Spaceship Ready");
	}

	public override void _Process(double delta)
	{
		if (_controlEnabled && !_isLandedOnEarth) 
		{
			ProcessSpaceshipMovement(delta);
			CheckProximityToEarth();
		}
	}

	public void SetControlEnabled(bool enabled)
	{
		_controlEnabled = enabled;
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

	private void CheckProximityToEarth()
	{
		if (earthNode == null) return;

		float distanceToEarth = GlobalTransform.Origin.DistanceTo(earthNode.GlobalTransform.Origin);

		if (distanceToEarth <= landingDistanceThreshold)
		{
			GD.Print("Close enough to Earth, ready to land!");

			if (Input.IsActionJustPressed("Land"))
			{
				LandOnEarth();
			}
		}
	}

	private void LandOnEarth()
	{
		GD.Print("Attempting to land on Earth...");

		_controlEnabled = false; // Disable further control
		_isLandedOnEarth = true; // Mark as landed
		SetPhysicsProcess(false); // Disable physics processing to prevent any unintended movement

		GetTree().CreateTimer(1.0).Timeout += () => TransitionToEarthScene();
	}

	private void TransitionToEarthScene()
	{
		if (earthScene != null)
		{
			// Load and instance Earth scene
			var earthInstance = earthScene.Instantiate();
			GetParent().AddChild(earthInstance);

			// Move the spaceship to Earth spawn point
			var spaceshipLandingPoint = earthInstance.GetNodeOrNull<Marker3D>("SpaceshipLandingPoint");
			if (spaceshipLandingPoint != null)
			{
				GlobalPosition = spaceshipLandingPoint.GlobalTransform.Origin;
				GlobalRotation = spaceshipLandingPoint.GlobalTransform.Basis.GetEuler();
				GD.Print("Transitioned to Earth scene for landing.");
			}
			else
			{
				GD.PrintErr("SpaceshipLandingPoint node not found in Earth scene.");
			}

			// Completely disable movement
			_controlEnabled = false;
			SetPhysicsProcess(false); 
		}
		else
		{
			GD.PrintErr("Earth scene not loaded. Please set the Earth scene reference.");
		}
	}

	// Method to return to space
	public void ReturnToSpace()
	{
		_controlEnabled = true;
		_isLandedOnEarth = false;
		SetPhysicsProcess(true); 

		string solarSystemScenePath = "res://SolarSystem.tscn";

		if (ResourceLoader.Exists(solarSystemScenePath))
		{
			GetTree().ChangeSceneToFile(solarSystemScenePath);
			GD.Print("Returning to space...");
		}
		else
		{
			GD.PrintErr("Solar System scene not found.");
		}
	}

	public override void _Input(InputEvent e)
	{
		if (e.IsActionPressed("ReturnToSpace") && _isLandedOnEarth)
		{
			ReturnToSpace();
		}
	}
}
