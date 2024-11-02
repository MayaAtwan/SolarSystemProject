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

	private bool _isInsideSpaceship = true;
	private SpaceShip _spaceship;
	private bool _inMap;

	public override void _Ready()
	{
		GD.Print("Player Ready");
		base._Ready();

		_spaceship = GetParent().GetNode<SpaceShip>("SpaceShip");

		if (_spaceship == null)
		{
			GD.PrintErr("Spaceship node not found. Check if 'SpaceShip' exists in the scene tree.");
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
			FollowSpaceship(delta);
			if (Input.IsActionJustPressed("ui_accept")) // "ui_accept" is usually mapped to the Enter key
			{
				ExitSpaceship();
			}
		}
		else
		{
			ProcessMovementInputs(delta);
		}
	}

	private void FollowSpaceship(double delta)
	{
		if (_spaceship == null) return;
		GlobalPosition = _spaceship.GlobalPosition;
		GlobalRotation = _spaceship.GlobalRotation;
	}

	private void ProcessMovementInputs(double delta)
	{
		var movement = Vector3.Zero;
		var forward = -GlobalTransform.Basis.Z;
		var left = -GlobalTransform.Basis.X;
		var up = GlobalTransform.Basis.Y;

		if (Input.IsActionPressed("Forward")) movement += forward;
		if (Input.IsActionPressed("Backward")) movement -= forward;
		if (Input.IsActionPressed("Left")) movement += left;
		if (Input.IsActionPressed("Right")) movement -= left;
		if (Input.IsActionPressed("Up")) movement += up;
		if (Input.IsActionPressed("Down")) movement -= up;

		ApplyCentralForce(_thrust * movement.Normalized());
	}

	// Called when the player exits the spaceship
	private void ExitSpaceship()
	{
		_isInsideSpaceship = false;

		// Place the player just outside the spaceship
		GlobalPosition = _spaceship.GlobalTransform.Origin + _spaceship.GlobalTransform.Basis.Z * 5f; // Adjust position as needed
		GD.Print("Player has exited the spaceship");
	}

	public override void _Input(InputEvent e)
	{
		base._Input(e);

		if (e is InputEventMouseMotion mouseMotion)
		{
			_mouseDelta += mouseMotion.Relative;
		}
	}

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
