/// <summary>
/// class used by pathfinding algorithm to track the exact steps to a destination.
/// Gcost is how many steps the tile is away, Hcost is the delta between where it is and the target,
/// and Fcost is the final score, which is the sum of Gcost and Hcost.
/// </summary>
public class PathNode (Board board, int tileX, int tileY, int gcost = 0, int hcost = 0, int fcost = 0, PathNode? parent = null)
{
    public Tile Tile { get; set; } = board.GetTile(tileX, tileY);
    public int Gcost { get; set; } = gcost;
    public int Hcost { get; set; } = hcost;
    public int Fcost { get; set; } = fcost;
    public PathNode? Parent { get; set; } = parent;
}

/// <summary>
/// This class holds utility functions that may need to be called for multiple reasons and are not tightly tied to other specific classes.
/// </summary>
public class Pathing
{
    /// <summary>
    /// finds all available tiles within a units alotted movement points.
    /// returns a List<Tile> type.
    /// </summary>
    public Dictionary<Tile, int> BreadthFirstSearch(Unit searcher, Board board)
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
        return searchedTiles;
    }

    /// <summary>
    /// Uses A* algo to find best route to the target tile.
    /// NOTE: This route INCLUDES the target tile, so if you are moving toward another unit, 
    /// the searcher will attempt to move onto the same tile as the target unit. Subtract the final Tile to end next to the target.
    /// </summary>
    public List<Tile> GetAStarPath(Unit searcher, Tile target, Board board)
    {
        PriorityQueue<PathNode, int> openList = new(); //queue that organizes tiles based on their score, sending lower scores (better) out first.
        HashSet<Tile> searchedTiles = []; //List of all tiles already visited to avoid looping over known tiles infintely.
        PathNode startNode = new(board, searcher.X, searcher.Y); //the beginning tile to kick things off.
        openList.Enqueue(startNode, 1);

        while (openList.Count > 0)
        {
            PathNode currentNode = openList.Dequeue(); //pull best scoring node out of the queue
            
            if (currentNode.Tile == target)
            {
                // This means I found my target, and I need to trace the route back to my unit.
                List<Tile> reversedPath = [];
                while (currentNode.Parent != null) //only the first tile will have parent == null, so we trace back until we find that tile.
                {
                    reversedPath.Add(currentNode.Tile); //since we are tracing back, the list we are aing to is in reverse order.
                    currentNode = currentNode.Parent;
                }
                    reversedPath.Reverse(); //flip the list so it is in order for us.
                    List<Tile> truePath = reversedPath;
                    return truePath; //the resulting path to our target.
            }

            (int dx, int dy)[] directions = { (0, -1), (0, 1), (-1, 0), (1, 0) }; //up, down, left, right.

            List<Tile> neighbors = []; // holds all neighboring tiles to loop through later

            foreach (var dir in directions)
            {
                int NextX = currentNode.Tile.X + dir.dx;
                int Nexty = currentNode.Tile.Y + dir.dy;

                if (board.IsInBounds(NextX, Nexty))
                {
                    neighbors.Add(board.GetTile(NextX, Nexty)); //if the neighbors are on the board, add them to the list to check.
                }
            }

            foreach (Tile tile in neighbors)
            { //the gamut. Test each tile if it has been searched before, is walkable, is unoccupied, etc.
                if (!searchedTiles.Contains(tile) && tile.IsWalkable && (!tile.IsOccupied || tile == target))
                {
                    PathNode neighborNode = new(
                        board, 
                        tile.X, 
                        tile.Y, 
                        currentNode.Gcost+1, 
                        Math.Abs(tile.X - target.X) + Math.Abs(tile.Y - target.Y), 
                        currentNode.Gcost+1 + Math.Abs(tile.X - target.X) + Math.Abs(tile.Y - target.Y), 
                        currentNode
                        );

                    //if the tile survives the checks, we put it in our queue to check if it is the target on future loops.
                    //the Fcost is it's score. The lower the score, the more ideal the tile and the sooner it comes out of the queue on new loops.
                    openList.Enqueue(neighborNode, neighborNode.Fcost); 

                }
            }
            //add the tile to searchedTiles so we don't keep checking it needlessly.
            //this tile is already a pathnode in the queue, so no need to do any more with it.
            searchedTiles.Add(currentNode.Tile);
        }
        //fallback. If the function can't find anything, return an empty list.
        return [];
    }
}