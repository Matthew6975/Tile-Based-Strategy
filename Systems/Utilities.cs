using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Net.NetworkInformation;

public class Utilities
{
    /// <summary>
    /// finds all available tiles within a units alotted movement points
    /// </summary>
    public List<Tile> BreadthFirstSearch(Unit searcher, Board board)
    {
        Tile start = board.GetTile(searcher.X, searcher.Y);
        Dictionary<Tile, int> searchedTiles = []; // A collection of all searched tiles and ther distance from start.
        Queue<Tile> searchQueue = []; // A queue of what tiles to search next.

        searchQueue.Enqueue(start);
        searchedTiles.Add(start, 0);

        while (searchQueue.Count > 0)
        {
            Tile currentTile = searchQueue.Dequeue();
            
            (int dx, int dy)[] directions = { (0, -1), (0, 1), (-1, 0), (1, 0) };
            List<Tile> neighbors = [];
            
            foreach (var (dx, dy) in directions)
            {
                int NextX = currentTile.X + dx;
                int NextY = currentTile.Y + dy;
                if (board.IsInBounds(NextX, NextY))
                {
                    neighbors.Add(board.GetTile(NextX, NextY));
                }
            }
            foreach (Tile tile in neighbors)
            {
                if (!searchedTiles.ContainsKey(tile) && tile.IsWalkable && !tile.IsOccupied)
                {
                    int newCost = searchedTiles[currentTile] + 1;
                    if (newCost <= searcher.CurrentMovePoints)
                    {
                        searchedTiles.Add(tile, newCost);
                        searchQueue.Enqueue(tile);
                    }
                }
            }
        }
        return searchedTiles.Keys.ToList<Tile>();
    }
}