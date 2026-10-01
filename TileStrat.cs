class Program
{
    static void Main()
    {
        Grid grid = new(10, 10);
        grid.BuildMap();

        grid.GetTile(3, 2).IsWalkable = false;
        grid.GetTile(3, 3).IsWalkable = false;
        grid.PrintMap();

        Unit hero = new("Player", Team.Player);
        Unit enemy = new("Enemy", Team.Enemy);
        grid.SpawnUnit(hero, 0, 0);
        grid.SpawnUnit(enemy, 8, 8);

        Console.Clear();
        grid.PrintMap();

        bool isRunning = true;
        while (isRunning)
        {
            Console.WriteLine($"Player position: ({hero.X}, {hero.Y})");
            Console.WriteLine($"Current move points: {hero.CurrentMovePoints}");
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
            grid.MoveUnit(hero, targetX, targetY);
            hero.CurrentMovePoints--;
            grid.PrintMap();
        }
    }
}