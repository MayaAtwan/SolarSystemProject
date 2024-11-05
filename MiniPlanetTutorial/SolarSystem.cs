using Godot;
using System.Linq;

public partial class SolarSystem : Node3D
{
	private Player _player;
	private SpaceShip _spaceship;
	public static SolarSystem Instance { get; private set; }

	public Planet[] Planets { get; private set; }
	public Earth1 EarthNode { get; private set; }

	public override void _Ready()
	{
		Instance = this;
		GD.Print("SolarSystem.Instance set");

		Planets = this.GetChildren().Where(x => x is Planet).Cast<Planet>().ToArray();

		foreach (var planet in Planets)
		{
			planet.Init();
		}

		EarthNode = GetNode<Earth1>("Earth1"); 
		_spaceship = GetNode<SpaceShip>("SpaceShip");
		_player = GetNode<Player>("Player");

		if (_spaceship == null || EarthNode == null)
		{
			GD.PrintErr("SpaceShip or Earth1 node not found in SolarSystem.");
		}
	}
}
