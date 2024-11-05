using Godot;
using System;

public partial class Earth : Node
{
	public override void _Ready()
	{
		// Spawn the player on Earth
		SpawnPlayer();

		// Spawn the spaceship on Earth
		SpawnSpaceship();
	}

	private void SpawnPlayer()
	{
		var playerSpawn = GetNode<Marker3D>("PlayerSpawn");
		PackedScene playerScene = (PackedScene)ResourceLoader.Load("res://Player.tscn"); 
		var playerInstance = playerScene.Instantiate<Node3D>();

		if (playerInstance != null && playerSpawn != null)
		{
			playerInstance.GlobalTransform = playerSpawn.GlobalTransform;
			AddChild(playerInstance);
			GD.Print("Player spawned at Earth landing point.");
		}
		else
		{
			GD.PrintErr("Player or PlayerSpawn not found.");
		}
	}

	private void SpawnSpaceship()
	{
		var spaceshipLandingPoint = GetNode<Marker3D>("SpaceshipLandingPoint");

		// Load the spaceship scene
		PackedScene spaceshipScene = (PackedScene)ResourceLoader.Load("res://SpaceShip.tscn");
		var spaceshipInstance = spaceshipScene.Instantiate<Node3D>();

		if (spaceshipInstance != null && spaceshipLandingPoint != null)
		{
			spaceshipInstance.GlobalTransform = spaceshipLandingPoint.GlobalTransform;
			AddChild(spaceshipInstance);
			GD.Print("Spaceship spawned at Earth landing point.");
		}
		else
		{
			GD.PrintErr("Spaceship or SpaceshipLandingPoint not found.");
		}
	}
}
