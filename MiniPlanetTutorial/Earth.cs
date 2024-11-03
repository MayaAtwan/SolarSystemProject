using Godot;
using System;

public partial class Earth : Node
{
	public override void _Ready()
	{
		var playerSpawn = GetNode<Marker3D>("PlayerSpawn");

		PackedScene playerScene = (PackedScene)ResourceLoader.Load("res://Player.tscn"); 
		var playerInstance = playerScene.Instantiate<Node3D>();

		if (playerInstance != null && playerSpawn != null)
		{	
			var playerTransform = playerInstance.GlobalTransform;
			playerTransform.Origin = playerSpawn.GlobalTransform.Origin;
			playerInstance.GlobalTransform = playerTransform;

			AddChild(playerInstance);
			GD.Print("Player spawned at Earth landing point.");
		}
		else
		{
			GD.PrintErr("Player or PlayerSpawn not found.");
		}
	}
}
