public static class  Armory
{
    public static List<Weapon> Weapons { get; } =
    [
        new Weapon { Name = "Blaster", Damage = 3, MinRange = 1, MaxRange = 5, Accuracy = 85 },
        new Weapon { Name = "Combat Shotgun", Damage = 5, MinRange = 1, MaxRange = 2, Accuracy = 95 },
        new Weapon { Name = "Sniper Rifle", Damage = 6, MinRange = 3, MaxRange = 9, Accuracy = 75 },
        new Weapon { Name = "Shock Blade", Damage = 7, MinRange = 1, MaxRange = 1, Accuracy = 100 }
    ];
}