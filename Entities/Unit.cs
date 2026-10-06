public class Unit(string name, Team team, int actionPoints = 1)
{
    public string Name { get; } = name;
    public int X { get; set; } = -1;
    public int Y { get; set; } = -1;
    public Team Team { get; } = team;
    public int Health { get; set; } = 10;
    public int MaxMovePoints {get;} = 5;
    public int CurrentMovePoints {get; set;} = 5;
    public Weapon? EquippedWeapon { get; set; } = null;
    public int ActionPoints { get; set; } = actionPoints;
}