//The actual game logic that runs
class Program
{
    static void Main()
    {
        //Sapwn a board, modify some specific tiles
        Board board = new(10, 10);
        board.BuildMap();
        board.GetTile(3, 2).IsWalkable = false;
        board.GetTile(3, 3).IsWalkable = false;

        //create some unit instances
        Unit hero = new("Player", Team.Player);
        Unit sidekick = new("Sidekick", Team.Player);
        Unit enemy = new("Enemy", Team.Enemy);
        Unit foe = new("Foe", Team.Enemy);

        //Spawn each of the units
        board.SpawnUnit(hero, 0, 0);
        board.SpawnUnit(sidekick, 0, 2);
        board.SpawnUnit(enemy, 4, 2);
        board.SpawnUnit(foe, 6, 6);

        //Initialize a basic weapon
        Weapon blaster = new()
        {
            Name = "Basic Blaster",
            Damage = 3,
            MinRange = 1,
            MaxRange = 5,
        };

        //equip the basic blaster to each of the unit instances
        sidekick.EquippedWeapon = blaster;
        hero.EquippedWeapon = blaster;
        foe.EquippedWeapon = blaster;
        enemy.EquippedWeapon = blaster;

        BattleManager battle = new();
        battle.RunBattle(board);
    }
}