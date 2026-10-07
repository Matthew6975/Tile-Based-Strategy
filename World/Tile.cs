/// <summary>
/// This class tracks the terrain for locations on the board, as well as what kind of cover the space provides
/// </summary>
public class Tile(int x, int y, bool isWalkable = true)
{
    public int X { get; } = x;
    public int Y { get; } = y;
    public bool IsWalkable { get; set; } = isWalkable;
    public Unit? Occupant { get; set; } = null;
    public bool IsOccupied => Occupant != null;


    public CoverType NorthCover { get; set; } = CoverType.None;
    public CoverType SouthCover { get; set; } = CoverType.None;
    public CoverType EastCover { get; set; } = CoverType.None;
    public CoverType WestCover { get; set; } = CoverType.None;
}
/// <summary>
/// what kinds of cover A tile can provide.
/// </summary>
public enum CoverType
{
    None,
    Half,
    Full
}