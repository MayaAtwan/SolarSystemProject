using Godot;
using System;

public partial class SpaceShip : RigidBody3D
{
	[Export] private float thrust = 10f;
	[Export] private float rotationSpeed = 1f;
	[Export] private StaticBody3D earthNode; 
	[Export] private float landingDistanceThreshold = 100f; 
	private bool _controlEnabled = true;
	private Vector2 _mouseDelta;

	public override void _Ready()
	{
		GD.Print("Spaceship Ready");
	}

	public override void _Process(double delta)
	{
		ProcessSpaceshipMovement(delta);
		CheckProximityToEarth();
	}

	public void SetControlEnabled(bool enabled)
	{
		_controlEnabled = enabled;
	}

	private void ProcessSpaceshipMovement(double delta)
	{
		if (!_controlEnabled) return;

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

	private void CheckProximityToEarth()
{
	if (earthNode == null) return;

	float distanceToEarth = GlobalTransform.Origin.DistanceTo(earthNode.GlobalTransform.Origin);

	if (distanceToEarth <= landingDistanceThreshold)
	{
		GD.Print("Close enough to Earth, ready to land!");

		// If the player presses the E - Land button switch to Earth scene
		if (Input.IsActionJustPressed("Land"))
		{
			LandOnEarth();
		}
	}
}


	private void LandOnEarth()
	{
		GD.Print("Attempting to land on Earth...");
		 var animationPlayer = GetNode<AnimationPlayer>($"../CanvasLayer/AnimationPlayer");
		GD.Print(animationPlayer);

	// Play the flash animation before fade-out
	animationPlayer.Play("Flash");
	GD.Print("Flash");

	// Delay fade-out slightly after flash effect
	GetTree().CreateTimer(0.2).Timeout += () =>
	{
		animationPlayer.Play("FadeOut");
		GetTree().CreateTimer(3.0).Timeout += () => TransitionToEarthScene();
	};
		// string earthScenePath ="res://Earth.tscn";

		//if (ResourceLoader.Exists(earthScenePath))
		//{
			//GetTree().ChangeSceneToFile(earthScenePath);
			//GD.Print("Transitioning to Earth scene for landing.");
		//}
		//else
		//{
			//GD.PrintErr("Earth scene not found at path: " + earthScenePath);
		//}
	}
	
	private void TransitionToEarthScene()
{
	string earthScenePath ="res://Earth.tscn"; 

	if (ResourceLoader.Exists(earthScenePath))
	{
		GetTree().ChangeSceneToFile(earthScenePath);
		GD.Print("Transitioning to Earth scene for landing.");
		// Play fade-in animation in the Earth scene
		var fadeAnimationPlayer = GetNode<AnimationPlayer>($"../CanvasLayer/AnimationPlayer");
		fadeAnimationPlayer.Play("FadeIn");
		GetTree().CreateTimer(3.0).Timeout += () =>
		{
			GD.Print("Fade-in complete.");
		};
	}
	else
	{
		GD.PrintErr("Earth scene not found at path: " + earthScenePath);
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
