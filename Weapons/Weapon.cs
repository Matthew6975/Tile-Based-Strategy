public class Weapon
{
    public string ?Name { get; set; } // what the weapon is called (blaster, pistol, sniper rifle, etc.)
    public int Damage { get; set; }  // how much health is removed from the target upon hit.
    public int MinRange { get; set; } // Minimum range would be used if a weapon cannot be used in close combat, like an rpg one tile away.
    public int MaxRange { get; set; } // The typicaL definition of range. How far the weapon can hit.
    public int Accuracy { get; set; } = 100; // Default accuracy is 100%
}