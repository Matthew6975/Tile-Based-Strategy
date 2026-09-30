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
public class Tile(int x, int y, bool isWalkable = true)
{
    public int X { get; } = x;
    public int Y { get; } = y;
    public bool IsWalkable { get; set; } = isWalkable;
    public Unit? Occupant { get; set; } = null;
    public bool IsOccupied => Occupant != null;
}

public class Grid(int width, int height)
{
    public int Width { get; } = width;
    public int Height { get; } = height;

    private readonly Tile[,] _tile = new Tile[width, height];

    public void BuildMap()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                _tile[x, y] = new Tile(x, y);
            }
        }
    }

    public Tile GetTile(int x, int y) => _tile[x, y];

    public bool IsInBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

    public void SpawnUnit(Unit unit, int startX, int startY)
    {
        if (!IsInBounds(startX, startY))
        {
            Console.WriteLine("Spawn position is outside of the board!");
            return;
        }
        else if (_tile[startX, startY].IsOccupied || !_tile[startX, startY].IsWalkable)
        {
            Console.WriteLine("Tile is blocked or not walkable!");
            return;
        }
        else
        {
            _tile[startX, startY].Occupant = unit;
            unit.X = startX;
            unit.Y = startY;
        }
    }

    public void MoveUnit(Unit unit, int newX, int newY)
    {
        if (!IsInBounds(newX, newY))
        {
            Console.WriteLine("Move is out of bounds!");
            return;
        }
        else if (_tile[newX, newY].Occupant != null || !_tile[newX, newY].IsWalkable)
        {
            Console.WriteLine("Tile is blocked or not walkable!");
            return;
        }
       else 
       {
        _tile[unit.X, unit.Y].Occupant = null;
        _tile[newX, newY].Occupant = unit;
        unit.X = newX;
        unit.Y = newY;
       }
    }

    public void PrintMap()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Tile tile = _tile[x, y];
                Console.Write(tile.IsWalkable ? "." : "#");
                if (tile.Occupant == null)
                {
                    Console.Write("_ ");
                }
                else if (tile.Occupant.Team == Team.Player)
                {
                    Console.Write("P ");
                }
                else if (tile.Occupant.Team == Team.Enemy)
                {
                    Console.Write("E ");
                }
                else
                {
                    Console.Write(". ");
                }

            }
            Console.WriteLine();
        }   
        Console.WriteLine();
    }
}

public class Unit(string name, Team team = Team.Player)
{
    public string Name { get; } = name;
    public int X { get; set; } = -1;
    public int Y { get; set; } = -1;
    public Team Team { get; } = team;
    public int Health { get; set; } = 10;
    public int MaxMovePoints {get;} = 5;
    public int CurrentMovePoints {get; set;} = 5;
}

public enum Team
    {
        Player,
        Enemy,
        Neutral
    }