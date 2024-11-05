using Godot;
using System;

public partial class SolarSystem : Node3D
{
	private Player _player;
	private SpaceShip _spaceship;
	public static SolarSystem Instance { get; private set; }
	
	// Add a reference to Earth1
	public Earth1 EarthNode { get; private set; }

	public override void _Ready()
	{
		Instance = this;
		GD.Print("SolarSystem.Instance set");

		// Initialize EarthNode
		EarthNode = GetNode<Earth1>("Earth1");

		if (EarthNode == null)
		{
			GD.PrintErr("Earth node not found in SolarSystem.");
		}

		// Find player and spaceship
		_player = GetNode<Player>("Player");
		_spaceship = GetNode<SpaceShip>("SpaceShip");

		if (_player == null || _spaceship == null)
		{
			GD.PrintErr("Player or SpaceShip node not found in SolarSystem.");
		}
		else
		{
			// Pass the EarthNode reference to SpaceShip
			_spaceship.SetEarthNode(EarthNode);
		}
	}
}
