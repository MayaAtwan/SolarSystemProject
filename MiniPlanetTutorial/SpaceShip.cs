using Godot;
using System;

public partial class SpaceShip : RigidBody3D
{
	[Export] private float thrust = 10f;
	[Export] private float rotationSpeed = 1f;

	private Vector2 _mouseDelta;

	public override void _Ready()
	{
		GD.Print("Spaceship Ready");
	}

	public override void _Process(double delta)
	{
		ProcessSpaceshipMovement(delta);
	}

	private void ProcessSpaceshipMovement(double delta)
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

		GlobalPosition += movement * thrust * (float)delta;

		if (Input.IsActionPressed("Rotate"))
		{
			var deltaX = _mouseDelta.Y * rotationSpeed * (float)delta;
			var deltaY = -_mouseDelta.X * rotationSpeed * (float)delta;
			RotateObjectLocal(Vector3.Up, Mathf.DegToRad(deltaY));
		}
	}

	public override void _Input(InputEvent e)
	{
		if (e is InputEventMouseMotion mouseMotion)
		{
			_mouseDelta += mouseMotion.Relative;
		}
	}
}
