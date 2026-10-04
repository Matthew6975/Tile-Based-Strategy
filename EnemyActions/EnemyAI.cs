using System.Diagnostics;
using System.Dynamic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

public class PathNode (Board board, int tileX, int tileY, int gcost = 0, int hcost = 0, int fcost = 0, PathNode? parent = null)
{
    public Tile Tile { get; set; } = board.GetTile(tileX, tileY);
    public int Gcost { get; set; } = gcost;
    public int Hcost { get; set; } = hcost;
    public int Fcost { get; set; } = fcost;
    public PathNode? Parent { get; set; } = parent;
}

public class EnemyAI
{
    public List<Tile> GetAStarPath(Unit searcher, Tile target, Board board)
    {
        PriorityQueue<PathNode, int> openList = new();
        HashSet<Tile> searchedTiles = [];
        PathNode startNode = new(board, searcher.X, searcher.Y);
        openList.Enqueue(startNode, 1);

        while (openList.Count > 0)
        {
            PathNode currentNode = openList.Dequeue();
            
            if (currentNode.Tile == target)
            {
                // This means I found my target, and I need to trace the route back to my unit.

                // NOTE: This route INCLUDES the target tile, so if you are moving toward another unit, 
                // the searcher will attempt to move onto the same tile as the target unit.
                List<Tile> reversedPath = [];
                while (currentNode.Parent != null)
                {
                    reversedPath.Add(currentNode.Tile);
                    currentNode = currentNode.Parent;
                }
                    reversedPath.Reverse();
                    List<Tile> truePath = reversedPath;
                    return truePath;
            }

            (int dx, int dy)[] directions = { (0, -1), (0, 1), (-1, 0), (1, 0) };

            List<Tile> neighbors = []; // holds all neighboring tiles to loop through later

            foreach (var dir in directions)
            {
                int NextX = currentNode.Tile.X + dir.dx;
                int Nexty = currentNode.Tile.Y + dir.dy;

                if (board.IsInBounds(NextX, Nexty))
                {
                    neighbors.Add(board.GetTile(NextX, Nexty));
                }
            }

    // this block is unneeded per the loop above this. Just keeping this here for now because I made it and am attached.
            // if (board.IsInBounds(currentNode.Tile.X, currentNode.Tile.Y - 1))
            // {
            //     Tile up = board.GetTile(currentNode.Tile.X, currentNode.Tile.Y - 1);
            //     neighbors.Add(up);
            // }
            // if (board.IsInBounds(currentNode.Tile.X, currentNode.Tile.Y + 1))
            // {
            //     Tile down = board.GetTile(currentNode.Tile.X, currentNode.Tile.Y + 1);
            //     neighbors.Add(down);
            // }
            // if (board.IsInBounds(currentNode.Tile.X-1, currentNode.Tile.Y))
            // {
            //     Tile left = board.GetTile(currentNode.Tile.X-1, currentNode.Tile.Y);
            //     neighbors.Add(left);
            // }
            // if (board.IsInBounds(currentNode.Tile.X+1, currentNode.Tile.Y))
            // {
            //     Tile right = board.GetTile(currentNode.Tile.X+1, currentNode.Tile.Y);
            //     neighbors.Add(right);
            // }

            foreach (Tile tile in neighbors)
            {
                if (!searchedTiles.Contains(tile) && tile.IsWalkable && (!tile.IsOccupied || tile == target))
                {
                    PathNode neighborNode = new(
                        board, 
                        tile.X, 
                        tile.Y, 
                        currentNode.Gcost+1, 
                        Math.Abs(tile.X - target.X) + Math.Abs(tile.Y - target.Y), 
                        (currentNode.Gcost+1 + Math.Abs(tile.X - target.X) + Math.Abs(tile.Y - target.Y)), 
                        currentNode
                        );

                    openList.Enqueue(neighborNode, neighborNode.Fcost);
                }

            }
            searchedTiles.Add(currentNode.Tile);
        }
        return [];
    }
}