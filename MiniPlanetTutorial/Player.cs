using Godot;
using System;

public partial class Player : RigidBody3D
{
	[ExportCategory("Nodes")]
	[Export] private Camera3D _camera;
	[Export] private Node3D _cameraPivot;
	[Export] private RayCast3D _groundCast;

	public bool IsGrounded => _groundCast.IsColliding();
	private Planet _ground => _groundCast.GetCollider() as Planet;
	private Vector3 _closestForce;

	[ExportCategory("Settings")]
	[Export] private float _mouseSensitivity = 0.3f;
	private Vector2 _mouseDelta;
	private float _cameraXRotation;

	[Export] private float _thrust = 1f;
	[Export] private float _autoOrientSpeed = 0.5f;

	private bool _inMap;

	public override void _Ready()
	{
		base._Ready();

		ShowMap(false);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (Input.IsActionJustReleased("Map"))
		{
			ShowMap(!_inMap);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		var closestForceMagnitude = 0f;
		_closestForce = Vector3.Zero;
		foreach (var planet in SolarSystem.Instance.planets)
		{
			var force = planet.GetForceAtPosition(GlobalPosition);
			ApplyCentralForce(force);

			// If we're close to a planet we track which one is pulling on us the most to orient our feet towards it
			if (planet.GlobalPosition.DistanceTo(GlobalPosition) < 2f * planet.surfaceRadius)
			{
				var magnitude = force.Length();
				if (magnitude > closestForceMagnitude) 
				{
					_closestForce = force;
				}
			}
		}

		if (!_inMap)
		{
			ProcessMovementInputs(delta);
			ProcessLookInputs(delta);
		}

		ProcessAutoOrientation(delta);

		_mouseDelta = Vector2.Zero;
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

		if (movement == Vector3.Zero)
		{
			PhysicsMaterialOverride.Friction = 5f;
		}
		else
		{
			PhysicsMaterialOverride.Friction = 0f;
			ApplyCentralForce(_thrust * movement.Normalized());
		}


	}

	private void ProcessLookInputs(double delta)
	{
		// Look - mouse y is the rotation around the x axis and vice versa
		var deltaX = _mouseDelta.Y * _mouseSensitivity;
		var deltaY = -_mouseDelta.X * _mouseSensitivity;

		if (Input.IsActionPressed("Rotate"))
		{
			Rotate(_cameraPivot.GlobalTransform.Basis.Z, Mathf.DegToRad(deltaY));
		}
		else
		{
			RotateObjectLocal(Vector3.Up, Mathf.DegToRad(deltaY));
			if (_cameraXRotation + deltaX > -90 && _cameraXRotation + deltaX < 90)
			{
				_cameraPivot.RotateX(Mathf.DegToRad(-deltaX));
				_cameraXRotation += deltaX;
			}
		}
	}

	private void ProcessAutoOrientation(double delta)
	{
		AngularVelocity = Vector3.Zero;


		var inZeroG = _closestForce == Vector3.Zero;
		inZeroG = true;

		if (inZeroG)
		{
			// Couldn't apply auto orientation - We are in zero G
			var dx = Mathf.Lerp(0, -_cameraXRotation, _autoOrientSpeed * (float)delta);
			_cameraXRotation += dx;

			_cameraPivot.RotateX(Mathf.DegToRad(-dx));
			RotateX(Mathf.DegToRad(dx));
		}
		else
		{
			var upDirection = -_closestForce.Normalized();
			var orientationDirection = new Quaternion(GlobalTransform.Basis.Y, upDirection) * GlobalTransform.Basis.GetRotationQuaternion();

			if (IsGrounded)
			{
				GD.Print("Grounded!");
				AngularVelocity = _ground.ConstantAngularVelocity.Project(upDirection);

				GlobalRotation = orientationDirection.Normalized().GetEuler();
			}
			else
			{
				GD.Print("Not grounded!");
				var rotation = GlobalTransform.Basis.GetRotationQuaternion().Slerp(orientationDirection.Normalized(), _autoOrientSpeed * (float)delta);

				GlobalRotation = rotation.GetEuler();
			}
		}


	}

	public override void _Input(InputEvent e)
	{
		base._Input(e);

		if (e is InputEventMouseMotion)
		{
			// Add up mouse motions in the input method and then apply them in physics process
			var mouseMotion = e as InputEventMouseMotion;
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
