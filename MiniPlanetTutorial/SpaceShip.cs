using Godot;
using System;

public partial class SpaceShip : RigidBody3D
{
	[Export] private float baseThrust = 700f; // Base thrust
	[Export] private float fuel = 100000f; // Starting fuel
	[Export] private float fuelConsumptionRate = 0.1f; // Fuel consumed per second of thrust
	[Export] private float maxSpeed = 1000f; // Maximum speed
	[Export] private float rotationSpeed = 1f; // Speed of rotation (yaw, pitch, roll)
	[Export] private float mass = 100f; // Spaceship mass
	[Export] private float gravityScale = 1f; // Adjust gravity effect
	[Export] private float dragCoefficient = 0.01f; // Simulate atmospheric drag

	[Export] private Camera3D camera;
	[Export] private Label hudLabel; // HUD label for displaying fuel and gravity

	private Earth1 earthNode; 
	private Vector3 velocity = Vector3.Zero; 
	private Vector3 totalGravity = Vector3.Zero; // Total gravity acting on the spaceship
	private float currentThrust = 0f; // Dynamic thrust
	private bool _controlEnabled = true;

	public override void _Ready()
	{
		GD.Print("Spaceship Ready");
		CallDeferred(nameof(InitializeSpaceship));

		if (hudLabel == null)
		{
			GD.PrintErr("HUDLabel is not assigned. Assign a Label node in the inspector.");
		}
	}

	private void InitializeSpaceship()
	{
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
			else
			{
				GD.Print("Earth node found in SolarSystem.");
			}
		}

		if (camera == null)
		{
			GD.PrintErr("Camera is not assigned. Attach a Camera3D to the spaceship.");
		}
	}

	public void SetEarthNode(Earth1 earth)
	{
		earthNode = earth;
		GD.Print("Earth node set in SpaceShip.");
	}

	private void ProcessSpaceshipMovement(double delta)
	{
		if (!_controlEnabled)
		{
			GD.Print("Movement is disabled.");
			return;
		}

		// Rotation Controls
		ProcessRotation(delta);

		// Thrust Controls
		Vector3 thrustDirection = Vector3.Zero;

		if (fuel > 0)
		{
			if (Input.IsActionPressed("Forward"))
			{
				GD.Print("Forward input detected!");
				thrustDirection += -GlobalTransform.Basis.Z;
				fuel -= fuelConsumptionRate * baseThrust * (float)delta;
			}

			if (Input.IsActionPressed("Backward"))
			{
				GD.Print("Backward input detected!");
				thrustDirection += GlobalTransform.Basis.Z;
				fuel -= fuelConsumptionRate * baseThrust * (float)delta;
			}
		}
		else
		{
			GD.Print("No fuel! The spaceship cannot move.");
		}

		// Normalize thrust direction and calculate thrust force
		if (thrustDirection != Vector3.Zero)
		{
			thrustDirection = thrustDirection.Normalized();
			currentThrust = baseThrust;
		}
		else
		{
			currentThrust = 0f;
		}

		Vector3 thrustForce = thrustDirection * currentThrust;
		GD.Print($"Thrust Direction: {thrustDirection}, Thrust Force: {thrustForce}");

		// Calculate acceleration
		Vector3 acceleration = thrustForce / mass;

		// Apply gravity and drag
		ApplyGravity(delta);
		ApplyDrag(delta);

		// Update velocity and position
		velocity += acceleration * (float)delta;
		velocity = velocity.LimitLength(maxSpeed);
		GlobalPosition += velocity * (float)delta;

		// Update camera
		UpdateCamera();

		// Update HUD
		UpdateHUD();

		GD.Print($"Velocity: {velocity.Length()} m/s, Current Thrust: {currentThrust}, Fuel: {fuel:F2}");
		if (fuel < 10f && fuel > 0f)
		{
			GD.Print("Warning: Fuel critically low!");
		}
	}

	private void ProcessRotation(double delta)
	{
		if (Input.IsActionPressed("YawLeft"))
			RotateY(-rotationSpeed * (float)delta);
		if (Input.IsActionPressed("YawRight"))
			RotateY(rotationSpeed * (float)delta);
		if (Input.IsActionPressed("PitchUp"))
			RotateX(-rotationSpeed * (float)delta);
		if (Input.IsActionPressed("PitchDown"))
			RotateX(rotationSpeed * (float)delta);
		if (Input.IsActionPressed("RollLeft"))
			RotateZ(-rotationSpeed * (float)delta);
		if (Input.IsActionPressed("RollRight"))
			RotateZ(rotationSpeed * (float)delta);
	}

	private void ApplyGravity(double delta)
	{
		totalGravity = Vector3.Zero;

		// Iterate through all planets in the "planets" group
		foreach (Node node in GetTree().GetNodesInGroup("planets"))
		{
			if (node is Planet planet)
			{
				// Add the gravitational acceleration from this planet
				var gravity = planet.GetAccelerationAtPosition(GlobalPosition);
				totalGravity += gravity;
				GD.Print($"[Gravity Debug] Planet: {planet.Name}, Gravity: {gravity}");
			}
		}

		// Apply the total gravity to the spaceship's velocity
		velocity += totalGravity * (float)delta;
		GD.Print($"[Gravity Debug] Total Gravity: {totalGravity}");
	}

	private void ApplyDrag(double delta)
	{
		// Atmospheric drag simulation
		if (earthNode != null && (GlobalPosition - earthNode.GlobalPosition).Length() < earthNode.surfaceRadius * 2)
		{
			velocity -= velocity * dragCoefficient * (float)delta;
		}
	}

	private void UpdateCamera()
	{
		if (camera == null) return;

		// Smoothly follow the spaceship
		camera.GlobalPosition = GlobalPosition - GlobalTransform.Basis.Z * 10f + GlobalTransform.Basis.Y * 5f;
		camera.LookAt(GlobalPosition, Vector3.Up);
	}

	private void UpdateHUD()
	{
		if (hudLabel != null)
		{
			hudLabel.Text = $"Fuel: {fuel:F2}\nGravity: {totalGravity}";
		}
	}

	public override void _Process(double delta)
	{
		ProcessSpaceshipMovement(delta);
	}
}
