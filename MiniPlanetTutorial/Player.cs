using Godot;
using System;

public partial class Player : RigidBody3D
{
	[ExportCategory("Nodes")]
	[Export] private Camera3D _camera;
	[Export] private Node3D _cameraPivot;

	[ExportCategory("Settings")]
	[Export] private float _mouseSensitivity = 0.01f;
	private Vector2 _mouseDelta;
	private float _cameraXRotation;

	[Export] private float _thrust = 1f;
	[Export] private float _rotationSpeed = 1f;

	private bool _isInsideSpaceship = true; // Starts inside the spaceship
	private Node3D _spaceship; // Reference to the spaceship
	private bool _inMap;

	public override void _Ready()
	{
		GD.Print("Player Ready");
		base._Ready();
		
		_spaceship = GetNode<Node3D>("/root/SolarSystem/Spaceship");

		if (_spaceship == null)
		{
			GD.PrintErr("Spaceship node not found. Check if 'Sketchfab_Scene' exists.");
		}
		else
		{
			GD.Print("Spaceship found: " + _spaceship.Name);
			GlobalPosition = _spaceship.GlobalTransform.Origin;
		}
		ShowMap(false);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (Input.IsActionJustReleased("Map"))
		{
			ShowMap(!_inMap);
		}
		if (_isInsideSpaceship)
		{
			ProcessSpaceshipMovement(delta); // Move spaceship if inside
		}
		else
		{
			ProcessMovementInputs(delta); // Move player if outside
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		ProcessLookInputs(delta); // Rotate camera for both spaceship and player
	}

	// Handles movement while the player is inside the spaceship
	private void ProcessSpaceshipMovement(double delta)
	{
		if (_spaceship == null) return; // Prevents null reference errors

		var movement = Vector3.Zero;

		var forward = -_spaceship.GlobalTransform.Basis.Z;
		var left = -_spaceship.GlobalTransform.Basis.X;
		var up = _spaceship.GlobalTransform.Basis.Y;

		// Handle spaceship movement
		if (Input.IsActionPressed("Forward")) movement += forward;
		if (Input.IsActionPressed("Backward")) movement -= forward;
		if (Input.IsActionPressed("Left")) movement += left;
		if (Input.IsActionPressed("Right")) movement -= left;
		if (Input.IsActionPressed("Up")) movement += up;
		if (Input.IsActionPressed("Down")) movement -= up;

		// Move the spaceship
		_spaceship.GlobalPosition += movement * _thrust * (float)delta;

		// Allow rotation of the spaceship with the mouse
		var deltaX = _mouseDelta.Y * _rotationSpeed * (float)delta;
		var deltaY = -_mouseDelta.X * _rotationSpeed * (float)delta;
		_spaceship.RotateObjectLocal(Vector3.Up, Mathf.DegToRad(deltaY));

		// Allow exiting the spaceship by pressing 'ui_accept'
		if (Input.IsActionJustPressed("ui_accept")) // Assuming 'ui_accept' is mapped to the Enter or left-click key
		{
			ExitSpaceship();
		}
	}

	// Handles movement when the player is outside the spaceship
	private void ProcessMovementInputs(double delta)
	{
		var movement = Vector3.Zero;

		var forward = -GlobalTransform.Basis.Z;
		var left = -GlobalTransform.Basis.X;
		var up = GlobalTransform.Basis.Y;

		// Allow free player movement
		if (Input.IsActionPressed("Forward")) movement += forward;
		if (Input.IsActionPressed("Backward")) movement -= forward;
		if (Input.IsActionPressed("Left")) movement += left;
		if (Input.IsActionPressed("Right")) movement -= left;
		if (Input.IsActionPressed("Up")) movement += up;
		if (Input.IsActionPressed("Down")) movement -= up;

		ApplyCentralForce(_thrust * movement.Normalized());
	}

	// Handles camera look and rotation
	private void ProcessLookInputs(double delta)
	{
		var deltaX = _mouseDelta.Y * _mouseSensitivity * (float)delta;
		var deltaY = -_mouseDelta.X * _mouseSensitivity * (float)delta;

		// Rotate the object (player or spaceship) on the horizontal axis (yaw)
		RotateObjectLocal(Vector3.Up, Mathf.DegToRad(deltaY));

		// Clamp vertical rotation (pitch)
		if (_cameraXRotation + deltaX > -90 && _cameraXRotation + deltaX < 90)
		{
			_cameraPivot.RotateX(Mathf.DegToRad(-deltaX));
			_cameraXRotation += deltaX;
		}

		_mouseDelta = Vector2.Zero;
	}

	// Called when the player exits the spaceship
	private void ExitSpaceship()
	{
		_isInsideSpaceship = false;

		// Detach player from the spaceship
		// GetParent().RemoveChild(this);
		//GetTree().Root.AddChild(this);

		// Place player just outside the spaceship
		GlobalPosition = _spaceship.GlobalTransform.Origin + _spaceship.GlobalTransform.Basis.Z * 5f; // Adjust position as needed
		GD.Print("Player has exited the spaceship");
	}

	public override void _Input(InputEvent e)
	{
		base._Input(e);

		// Capture mouse movement for rotation
		if (e is InputEventMouseMotion mouseMotion)
		{
			_mouseDelta += mouseMotion.Relative;
		}
	}

	// Toggles the map view mode
	private void ShowMap(bool inMap)
	{
		_inMap = inMap;

		if (_inMap)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
			_camera.Current = false;
		}
		else
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
			_camera.Current = true;
		}
	}
}
