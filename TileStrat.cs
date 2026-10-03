class Program
{
    static void Main()
    {
        Board board = new(10, 10);
        board.BuildMap();
        board.GetTile(3, 2).IsWalkable = false;
        board.GetTile(3, 3).IsWalkable = false;

        Unit hero = new("Player", Team.Player);
        Unit enemy = new("Enemy", Team.Enemy);
        Unit foe = new("Foe", Team.Enemy);

        Weapon blaster = new()
        {
            Name = "Blaster",
            Damage = 3,
            MinRange = 1,
            MaxRange = 5,
        };

        hero.EquippedWeapon = blaster;

        Combat combatSystem = new();

        board.SpawnUnit(hero, 0, 0);
        board.SpawnUnit(enemy, 8, 8);
        board.SpawnUnit(foe, 6, 6);

        Console.Clear();
        board.PrintMap();

        bool isRunning = true;
        while (isRunning)
        {
            Console.WriteLine($"Player position: ({hero.X}, {hero.Y})");
            Console.WriteLine($"Current move points: {hero.CurrentMovePoints}");
            Console.WriteLine($"Enemy Health: {enemy.Health}");
            Console.WriteLine("Enter a direction to move (W/A/S/D or Arrow Keys) or Q to quit:");

            ConsoleKey key = Console.ReadKey(true).Key;

            int targetX = hero.X;
            int targetY = hero.Y;

            if (key == ConsoleKey.W || key == ConsoleKey.UpArrow) targetY--; // Up
            else if (key == ConsoleKey.S || key == ConsoleKey.DownArrow) targetY++; // Down
            else if (key == ConsoleKey.A || key == ConsoleKey.LeftArrow) targetX--; // Left
            else if (key == ConsoleKey.D || key == ConsoleKey.RightArrow) targetX++; // Right
            else if (key == ConsoleKey.Q) isRunning = false; // Quit
            else if (key == ConsoleKey.R) // Reset move points
            {
                hero.CurrentMovePoints = hero.MaxMovePoints;
                Console.WriteLine("Move points reset!");
                continue;
            }
            else if (key == ConsoleKey.F) // Enter targeting/attack mode
            {
                List<Unit> validTargets = combatSystem.GetValidTargets(hero, board);

                if (validTargets.Count == 0)
                {
                    Console.WriteLine("No valid targets in range! Press any key to continue...");
                    Console.ReadKey(true);
                    continue;
                }

                bool isTargeting = true;
                int targetIndex = 0;

                while (isTargeting)
                {
                    Console.Clear();
                    board.PrintMap();

                    Unit currentTarget = validTargets[targetIndex];
                    float hitChance = combatSystem.CalculateHitChance(hero, currentTarget, board);

                    Console.WriteLine("=== TARGETING MODE ===");
                    Console.WriteLine($"Target: {currentTarget.Name} at ({currentTarget.X}, {currentTarget.Y})");
                    Console.WriteLine($"Hit Chance: {hitChance}% | Target Health: {currentTarget.Health}");
                    Console.WriteLine("Use Left/Right Arrows or Tab to cycle targets.");
                    Console.WriteLine("Press Enter or F to Fire. Press Esc or Q to Cancel.");

                    ConsoleKey targetKey = Console.ReadKey(true).Key;

                    if (targetKey == ConsoleKey.RightArrow || targetKey == ConsoleKey.Tab)
                    {
                        targetIndex++;
                        if(targetIndex >= validTargets.Count()) targetIndex = 0;
                    }
                    else if (targetKey == ConsoleKey.LeftArrow)
                    {
                        targetIndex--;
                        if (targetIndex < 0) targetIndex = validTargets.Count() -1;
                        
                    }
                    else if (targetKey == ConsoleKey.F || targetKey == ConsoleKey.Enter)
                    {
                        Console.Clear();
                        combatSystem.Attack(hero, currentTarget, board);
                        Console.WriteLine("press any key to continue...");
                        Console.ReadKey(true);
                        isTargeting = false;   
                    }
                    else if (targetKey == ConsoleKey.Q || targetKey == ConsoleKey.Escape)
                    {
                        isTargeting = false;
                    }
                }
                Console.Clear();
                board.PrintMap();
                continue;
            }
            else
            {
                Console.WriteLine($"Ignored key: {key}. Use WASD or Arrow Keys.");
                continue;
            }

            if (hero.CurrentMovePoints <= 0)
            {
                Console.WriteLine("No more move points available!");
                continue;
            }

            Console.Clear();
            board.MoveUnit(hero, targetX, targetY);
            hero.CurrentMovePoints--;
            board.PrintMap();
        }
    }
}