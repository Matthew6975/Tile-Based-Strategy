using System.Diagnostics;
using System.Dynamic;
using System.Formats.Tar;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

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
    Combat combatSystem = new();
    Utilities utilities = new();
    /// <summary>
    /// Uses A* algo to find best route to the target tile.
    /// NOTE: This route INCLUDES the target tile, so if you are moving toward another unit, 
    /// the searcher will attempt to move onto the same tile as the target unit. Subtract the final Tile to end naxt to the target.
    /// </summary>
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
    public int ScoreAction(Unit attacker, Tile originTile, Tile targetTile, Board board)
    {
        int moveScore = 0;
        int hitChance = combatSystem.CalculateHitChance(attacker, originTile, targetTile);
        int range = combatSystem.CalculateDistance(originTile.X, originTile.Y, targetTile.X, targetTile.Y);
        int moveDistance = combatSystem.CalculateDistance(attacker.X, attacker.Y, originTile.X, originTile.Y);
        CoverType cover = combatSystem.GetFacingCover(originTile, targetTile); //the arguments are flipped here because this is calculating the attack coming back.

        //subtract move distance from score to prioritize efficient movement and help break ties.
        //this number should be very small. Never larger than unit movement.
        moveScore -= moveDistance;

        //points for attack logic 
        if (targetTile.Occupant != null && attacker.EquippedWeapon != null)
        {
            //is the target in range?
            if (range <= attacker.EquippedWeapon.MaxRange && range >= attacker.EquippedWeapon.MinRange)
            {
                moveScore += 100;
            }

            //is there a hit chance?
            if (hitChance > 0)
            {
                moveScore += hitChance;
            }

            //is the attack lethal?
            if (targetTile.Occupant.Health <= attacker.EquippedWeapon.Damage)
            {
                moveScore += 300;
            }
        }

        // adds bonuses for better cover taken. Slightly higher than hit chance boosts to avoid ties
        // and this favors cover a bit more than damage now
        if (cover == CoverType.Full)
        {
            moveScore += 110;
        }
        else if (cover == CoverType.Half)
        {
            moveScore += 55;
        }
        return moveScore;
    }
}