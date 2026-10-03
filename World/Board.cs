public class Board(int width, int height)
{
    public int Width { get; } = width;
    public int Height { get; } = height;
    private readonly Tile[,] _tile = new Tile[width, height];
    public List<Unit> ActiveUnits { get; } = [];

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
            ActiveUnits.Add(unit);
        }
    }

    public void DespawnUnit(Unit unit)
    {
        if (IsInBounds(unit.X, unit.Y) && _tile[unit.X, unit.Y].Occupant == unit)
        {
            _tile[unit.X, unit.Y].Occupant = null;
            unit.X = -1;
            unit.Y = -1;
            ActiveUnits.Remove(unit);
        }
        else
        {
            Console.WriteLine("Unit is not on the board!");
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