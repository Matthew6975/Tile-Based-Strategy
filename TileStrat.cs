class Program
{
    static void Main()
    {
        Grid grid = new(10, 10);
        grid.BuildMap();

        grid.GetTile(3, 2).IsWalkable = false;
        grid.GetTile(3, 3).IsWalkable = false;
        grid.PrintMap();

        Unit hero = new("Player",0, 0);
        grid.SpawnUnit(hero);

        Console.Clear();
        grid.PrintMap();

        bool isRunning = true;
        while (isRunning)
        {
            Console.WriteLine($"Player position: ({hero.X}, {hero.Y}) \nEnter a direction to move (W/A/S/D) or Q to quit:");

            ConsoleKey key = Console.ReadKey(true).Key;

            int targetX = hero.X;
            int targetY = hero.Y;

            if (key == ConsoleKey.W || key == ConsoleKey.UpArrow) targetY--; // Up
            else if (key == ConsoleKey.S || key == ConsoleKey.DownArrow) targetY++; // Down
            else if (key == ConsoleKey.A || key == ConsoleKey.LeftArrow) targetX--; // Left
            else if (key == ConsoleKey.D || key == ConsoleKey.RightArrow) targetX++; // Right
            else if (key == ConsoleKey.Q) isRunning = false; // Quit
            else
            {
                Console.WriteLine($"Ignored key: {key}. Use WASD or Arrow Keys.");
                continue;
            }

            Console.Clear();
            grid.MoveUnit(hero, targetX, targetY);
            grid.PrintMap();
        }
    }
}
public class Tile(int x, int y, bool isWalkable = true, bool isOccupied = false)
{
    public int X { get; } = x;
    public int Y { get; } = y;
    public bool IsWalkable { get; set; } = isWalkable;
    public bool IsOccupied { get; set; } = isOccupied;
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

    public void SpawnUnit(Unit unit)
    {
        if (!IsInBounds(unit.X, unit.Y))
        {
            Console.WriteLine("Spawn position is outside of the board!");
            return;
        }
        else
        {
            _tile[unit.X, unit.Y].IsOccupied = true;
        }
    }

    public void MoveUnit(Unit unit, int newX, int newY)
    {
        if (!IsInBounds(newX, newY))
        {
            Console.WriteLine("Move is out of bounds!");
            return;
        }
        else if (_tile[newX, newY].IsOccupied || !_tile[newX, newY].IsWalkable)
        {
            Console.WriteLine("Tile is blocked or not walkable!");
            return;
        }
       else 
       {
        _tile[unit.X, unit.Y].IsOccupied = false;
        _tile[newX, newY].IsOccupied = true;
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
                Console.Write(tile.IsOccupied ? "! " : "_ ");
            }
            Console.WriteLine();
        }   
        Console.WriteLine();
    }
}

public class Unit(string name, int startX, int startY)
{
    public string Name { get; } = name;
    public int X { get; set; } = startX;
    public int Y { get; set; } = startY;
}