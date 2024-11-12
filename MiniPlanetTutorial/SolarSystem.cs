using Godot;
using System;

public partial class SolarSystem : Node3D
{
	[Signal] 
	public delegate void InitializationCompleteEventHandler();

	private Player _player;
	private SpaceShip _spaceship;
	public static SolarSystem Instance { get; private set; }
	public Earth1 EarthNode { get; private set; }

	public override void _Ready()
	{
		GD.Print("SolarSystem is initializing...");
		InitializeSolarSystem(); 
	}

	private void InitializeSolarSystem()
	{
		Instance = this;
		GD.Print("SolarSystem.Instance set");
		EarthNode = GetNodeOrNull<Earth1>("Earth1");
		GD.Print("EarthNode found: ", EarthNode != null);
		_player = GetNodeOrNull<Player>("Player");
		_spaceship = GetNodeOrNull<SpaceShip>("SpaceShip");

		if (_spaceship != null)
		{
			GD.Print("Setting EarthNode in SpaceShip...");
			_spaceship.SetEarthNode(EarthNode);
		}

		if (_player != null)
		{
			GD.Print("Player found: Setting up Player...");
		}

		EmitSignal(nameof(InitializationComplete));
		GD.Print("SolarSystem initialization complete.");
		
		
		GD.Print("Earth1 Position: ", EarthNode.GlobalTransform.Origin);
GD.Print("Player Position: ", _player?.GlobalTransform.Origin);
GD.Print("SpaceShip Position: ", _spaceship?.GlobalTransform.Origin);

	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
	}
}
