using Godot;
using System;

public partial class SolarSystem : Node3D
{
	private Player _player;
	private SpaceShip _spaceship;
	public static SolarSystem Instance { get; private set; }
	public Earth1 EarthNode { get; private set; }

public override void _Ready()
{
	CallDeferred(nameof(InitializeSolarSystem)); // Use CallDeferred to delay initialization
}

private void InitializeSolarSystem()
{
	Instance = this;
	GD.Print("SolarSystem.Instance set");

	EarthNode = GetNodeOrNull<Earth1>("Earth1");

	if (EarthNode == null)
	{
		GD.PrintErr("Earth node not found in SolarSystem.solar system");
	}
	else
	{
		GD.Print("Earth node found in SolarSystem.");
	}

	_player = GetNodeOrNull<Player>("Player");
	_spaceship = GetNodeOrNull<SpaceShip>("SpaceShip");

	if (_player == null)
	{
		GD.PrintErr("Player node not found in SolarSystem.");
	}
	if (_spaceship == null)
	{
		GD.PrintErr("SpaceShip node not found in SolarSystem.");
	}
	else
	{
		_spaceship.SetEarthNode(EarthNode);
	}
}


	private T FindChild<T>() where T : Node
	{
		foreach (Node child in GetChildren())
		{
			if (child is T node)
				return node;
		}
		return null;
	}
}
