using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[ExportCategory("Nodes")]
	[Export] private Camera3D _camera;
	[Export] private Node3D _cameraPivot;

	[ExportCategory("Settings")]
	[Export] private float _mouseSensitivity = 0.01f;
	private Vector2 _mouseDelta;
	private float _cameraXRotation;

	[Export] private float _walkSpeed = 5f;
	[Export] private float _spaceThrust = 1f;
	[Export] private float _rotationSpeed = 1f;

	private bool _isInsideSpaceship = true;
	private bool _onEarth = false;
	private SpaceShip _spaceship;
	private Vector3 _velocity = Vector3.Zero;

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
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (_isInsideSpaceship)
		{
			FollowSpaceship(delta);
			if (Input.IsActionJustPressed("ui_accept"))
			{
				ExitSpaceship();
			}
		}
		else
		{
			if (_onEarth)
			{
				ProcessEarthMovement(delta);
			}
			else
			{
				ProcessSpaceMovement(delta);
			}
		}
	}

	private void FollowSpaceship(double delta)
	{
		if (_spaceship == null) return;
		GlobalPosition = _spaceship.GlobalPosition;
		GlobalRotation = _spaceship.GlobalRotation;
	}

	private void ProcessSpaceMovement(double delta)
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

		_velocity = movement.Normalized() * _spaceThrust * (float)delta;

		GlobalPosition += _velocity; // Directly update position in space
	}

	private void ProcessEarthMovement(double delta)
	{
		var movement = Vector3.Zero;
		var forward = -GlobalTransform.Basis.Z;
		var left = -GlobalTransform.Basis.X;

		if (Input.IsActionPressed("Forward")) movement += forward;
		if (Input.IsActionPressed("Backward")) movement -= forward;
		if (Input.IsActionPressed("Left")) movement += left;
		if (Input.IsActionPressed("Right")) movement -= left;

		_velocity = movement.Normalized() * _walkSpeed * (float)delta;
		
		// Keep the player grounded by ignoring Y-axis changes
		_velocity.Y = 0;
		
		GlobalPosition += _velocity; // Directly update position on Earth
	}

	private void ExitSpaceship()
	{
		_isInsideSpaceship = false;
		_onEarth = true;

		GlobalPosition = _spaceship.GlobalTransform.Origin + _spaceship.GlobalTransform.Basis.Z * 5f;
		GD.Print("Player has exited the spaceship");

		//_spaceship.SetControlEnabled(false);

		_camera.Current = true;
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public void EnterSpaceMode()
	{
		_onEarth = false;
		_isInsideSpaceship = false;
		_velocity = Vector3.Zero;
	}

	public override void _Input(InputEvent e)
	{
		base._Input(e);

		if (e is InputEventMouseMotion mouseMotion)
		{
			_mouseDelta += mouseMotion.Relative;
		}
	}
}
