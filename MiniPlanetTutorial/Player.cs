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

	private Earth1 _earthNode;

	public override void _Ready()
	{
		GD.Print("Player Ready");

		var solarSystem = GetParent<SolarSystem>();
		if (solarSystem != null)
		{
			solarSystem.Connect("InitializationComplete", new Callable(this, nameof(OnSolarSystemInitialized)));
		}
		else
		{
			GD.PrintErr("SolarSystem parent not found for Player.");
		}
	}

	private void OnSolarSystemInitialized()
	{
		GD.Print("SolarSystem initialization complete. Setting EarthNode in Player...");
		var solarSystem = GetParent<SolarSystem>();
		_earthNode = solarSystem?.EarthNode;
		GD.Print("Earth node set in Player: ", _earthNode != null);
	}

	public override void _Process(double delta)
	{
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

	private void ProcessEarthMovement(double delta)
	{
		if (_earthNode == null) return;

		Vector3 toEarthCenter = _earthNode.GlobalTransform.Origin - GlobalPosition;
		Vector3 up = toEarthCenter.Normalized();

		LookAtFromPosition(GlobalPosition, GlobalPosition + up, -GlobalTransform.Basis.Z);

		var movement = Vector3.Zero;
		var forward = -GlobalTransform.Basis.Z;
		var left = -GlobalTransform.Basis.X;

		if (Input.IsActionPressed("Forward")) movement += forward;
		if (Input.IsActionPressed("Backward")) movement -= forward;
		if (Input.IsActionPressed("Left")) movement += left;
		if (Input.IsActionPressed("Right")) movement -= left;

		_velocity = movement.Normalized() * _walkSpeed * (float)delta;
		_velocity = _velocity - (_velocity.Dot(up)) * up;
		GlobalPosition += _velocity;

		float targetDistance = _earthNode.surfaceRadius + 1.0f;
		float currentDistance = GlobalPosition.DistanceTo(_earthNode.GlobalTransform.Origin);

		// Correct position only if deviation is significant
		if (Math.Abs(currentDistance - targetDistance) > 0.1f)
		{
			Vector3 correctedPosition = _earthNode.GlobalTransform.Origin + up * targetDistance;
			GlobalPosition = correctedPosition;
		}

		GD.Print("Player Position:", GlobalPosition, "Is on Earth's surface.");
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
		GlobalPosition += _velocity;

		GD.Print("Player Position in space:", GlobalPosition);
	}

	private void FollowSpaceship(double delta)
	{
		if (_spaceship == null) return;
		GlobalPosition = _spaceship.GlobalPosition;
		GlobalRotation = _spaceship.GlobalRotation;
		GD.Print("Player following spaceship at position:", GlobalPosition);
	}

	private void ExitSpaceship()
	{
		if (_spaceship == null)
		{
			GD.PrintErr("Cannot exit spaceship: _spaceship is null.");
			return;
		}

		_isInsideSpaceship = false;
		_onEarth = true;

		GlobalPosition = _spaceship.GlobalTransform.Origin + _spaceship.GlobalTransform.Basis.Z * 5f;
		GD.Print("Player has exited the spaceship at position:", GlobalPosition);

		_camera.Current = true;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_velocity = Vector3.Zero;

		GD.Print("Player is now on Earth and can move freely.");
	}
}
