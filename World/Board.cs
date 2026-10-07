
/// <summary>
/// This class tracks everything to do with each map. Including what tile is on each space, where units are,
/// spawning units, despawning units, moving units, board creation, etc.
/// </summary>
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

    /// <summary>
    /// returns a tile at a given (X, Y) coordinate.
    /// useful for finding what tile a unit is standing on
    /// </summary>
    public Tile GetTile(int x, int y) => _tile[x, y];

    /// <summary>
    /// Checks if an (X, Y) coordinate is in-bounds of the map.
    /// returns a True/False value.
    /// </summary>
    public bool IsInBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

    /// <summary>
    /// places a unit on the board if the called position is in bounds of the map, unoccupied, and walkable by units.
    /// </summary>
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

    /// <summary>
    /// removes a unit from the board.
    /// </summary>
    public void DespawnUnit(Unit unit)
    {
        if (IsInBounds(unit.X, unit.Y) && _tile[unit.X, unit.Y].Occupant == unit)
        {
            _tile[unit.X, unit.Y].Occupant = null;
            unit.X = -100;
            unit.Y = -100;
            ActiveUnits.Remove(unit);
        }
        else
        {
            Console.WriteLine("Unit is not on the board!");
        }
    }

    /// <summary>
    /// moves a nit to a new location if the move is in bounds and allowed.
    /// returns true if move succeeded. This can be used to track if it succeeded and not waste 
    /// </summary>
    public bool MoveUnit(Unit unit, int newX, int newY)
    {
        if (!IsInBounds(newX, newY))
        {
            Console.WriteLine("Move is out of bounds!");
            return false;
        }
        else if (_tile[newX, newY].Occupant != null)
        {
            Console.WriteLine("Tile is blocked!");
            return false;
        }
        else if (!_tile[newX, newY].IsWalkable)
        {
            Console.WriteLine("Tile is not walkable!");
            return false;
        }
        else 
        {
            _tile[unit.X, unit.Y].Occupant = null;
            _tile[newX, newY].Occupant = unit;
            unit.X = newX;
            unit.Y = newY;
            return true;
        }
    }

    /// <summary>
    /// displays the test map in the terminal
    /// </summary>
    public void PrintMap()
    {
        //clear any old messages on the terminal, then print updated board.
        Console.Clear();
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