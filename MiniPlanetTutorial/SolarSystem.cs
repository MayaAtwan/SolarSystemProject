using Godot;
using System.Linq;

public partial class SolarSystem : Node3D
{
	private Player _player;
	private SpaceShip _spaceship;
	public static SolarSystem Instance { get; private set; }

	public Planet[] Planets { get; private set; }

	public override void _Ready()
	{
		Instance = this;
		GD.Print("SolarSystem.Instance set");
		Planets = this.GetChildren().Where(x => x is Planet).Cast<Planet>().ToArray();

		var orderedPlanets = Planets.OrderBy(planet => planet.HowManyParents());
		foreach (var planet in orderedPlanets)
		{
			planet.Init();
		} 
		_player = GetNode<Player>("Player");
		_spaceship = GetNode<SpaceShip>("SpaceShip");

		if (_player == null || _spaceship == null)
		{
			GD.PrintErr("Player or SpaceShip node not found in SolarSystem.");
		}
	}
}
