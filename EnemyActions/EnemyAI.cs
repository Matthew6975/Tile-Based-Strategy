using System.Diagnostics;
using System.Dynamic;
using System.Formats.Tar;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

///
/// <sumarry>
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

public class EnemyAI
{
    readonly Combat combatSystem = new();
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

    ///<summary>
    /// This function scores the enemyAI's movement choices returned from a breadthFirstSearch so it can choose a destination
    /// Accounts for:
    /// 1. Taking cover
    /// 2. Attack chance
    /// 3. Lethality of attack
    /// 4. In range to attack after move
    /// </summary>
    public int ScoreAction(Unit attacker, Tile originTile, Tile targetTile)
    {
        //the score that is eventually returned
        int moveScore = 0;
        //What hit chance would the attacker have if they move there?
        int hitChance = combatSystem.CalculateHitChance(attacker, originTile, targetTile);
        //Will the attacker be in range to attack if they move to this tile?
        int range = combatSystem.CalculateDistance(originTile.X, originTile.Y, targetTile.X, targetTile.Y);
        //How far away is this tile?
        int moveDistance = combatSystem.CalculateDistance(attacker.X, attacker.Y, originTile.X, originTile.Y);
        //What kind of cover will the enemyAI have from returning fire? The arguments are flipped here because this is calculating the attack coming back.
        CoverType cover = combatSystem.GetFacingCover(originTile, targetTile);

        //subtract move distance from score to prioritize efficient movement and help break ties.
        //this number will always be very small. Never larger than unit movement. Should not do much besides help break ties.
        moveScore -= moveDistance;

        //points for attack logic 
        if (targetTile.Occupant != null && attacker.EquippedWeapon != null)
        {
            //would the target be in range?
            if (range <= attacker.EquippedWeapon.MaxRange && range >= attacker.EquippedWeapon.MinRange) 
            {
                moveScore += 100;

                //would there be a hit chance?
                if (hitChance > 0) moveScore += hitChance;

                //would the attack be lethal?
                if (targetTile.Occupant.Health <= attacker.EquippedWeapon.Damage) moveScore += 300;
            }
        }
        else Console.WriteLine($"{attacker.Name} Does not have a weapon equipped!");

        // adds bonuses for better cover taken. Slightly higher than hit chance boosts to avoid ties
        // and this favors cover a bit more than damage now
        if (cover == CoverType.Full) moveScore += 110;
        else if (cover == CoverType.Half) moveScore += 55;

        return moveScore;
    }

    public void ExecuteEnemyTurn (Board board, List<Unit> badGuys, List<Unit> goodGuys, Utilities utilities)
    {
        //update map state
        board.PrintMap();
        Console.WriteLine("Enemy is thinking...");
        System.Threading.Thread.Sleep(1000); // a 1 sec pause so I can see the enemy actions

        //Loop to find and score each of the enemy's potential move options and execute the best move
        foreach (Unit badguy in badGuys)
        {
            List<Tile> moveList = utilities.BreadthFirstSearch(badguy, board);
            Tile bestDest = board.GetTile(badguy.X, badguy.Y);
            Unit? bestTarget = null;
            int highScore = 0;

            foreach (Tile tile in moveList)
            {
                foreach (Unit player in goodGuys)
                {
                    Tile playerTile = board.GetTile(player.X, player.Y);
                    if (badguy.EquippedWeapon != null)
                    {
                        int score = ScoreAction(badguy, tile, playerTile);

                        if (score > highScore)
                        {
                            bestDest = tile;
                            bestTarget = player;
                            highScore = score;
                        }
                    }
                }
            }

            // If there is no scoring move and there are player units on the board, 
            // have the unit move towards the closest player unit
            if (highScore == 0 && goodGuys.Count > 0)
            {
                Unit nearestPlayer = goodGuys[0];
                int shortestDistance = int.MaxValue;

                foreach (Unit unit in goodGuys)
                {
                    //find the closest player unit
                    int distance = combatSystem.CalculateDistance(unit.X, unit.Y, badguy.X, badguy.Y);
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        nearestPlayer = unit;
                    }
                }
                // out of the potential moves from my breadth first search, find the tile closest to closest player unit so we can start moving towards it. 
                // I need this rather than just passing the nearest player tile to A*
                int closestTileDist = int.MaxValue;
                foreach (Tile tile in moveList)
                {
                    int distance = combatSystem.CalculateDistance(tile.X, tile.Y, nearestPlayer.X, nearestPlayer.Y);
                    if (distance < closestTileDist)
                    {
                        closestTileDist = distance;
                        bestDest = tile;
                    }
                }
            }

            //Once the best path/target/destination has been chosen, actually move to execute this course of action.
            List<Tile> path = GetAStarPath(badguy, bestDest, board);
            if (path.Count > 0)
            {                        
                foreach (Tile step in path)
                {
                    if (badguy.CurrentMovePoints <= 0)
                        {
                            Console.WriteLine($"{badguy.Name} is out of movement!");
                            break;
                        }

                    bool moveCheck = board.MoveUnit(badguy, step.X, step.Y);
                    if (moveCheck)
                    {
                        badguy.CurrentMovePoints--;
                        board.PrintMap();
                        System.Threading.Thread.Sleep(500); //short pause between each step enemy takes for my inferior, human brain to understand
                        //Make sure the enemy has movement points
                    }
                    else
                    {
                        // the unit still has movePoints, but the move failed, so the path must be blocked, so the loop is ended
                        Console.WriteLine($"{badguy.Name}'s path was blocked!");
                        break;
                    }
                }
            }

            //if there is a target, the AI has a weapon, and the target is in range, initiate an attack.
            if (bestTarget != null && badguy.EquippedWeapon != null)
            {
                int rangeToTarget = combatSystem.CalculateDistance (badguy.X, badguy.Y, bestTarget.X, bestTarget.Y);
                if (rangeToTarget <= badguy.EquippedWeapon.MaxRange && rangeToTarget >= badguy.EquippedWeapon.MinRange)
                {
                    Tile target = board.GetTile(bestTarget.X, bestTarget.Y);
                    combatSystem.Attack(badguy, target, board);
                    if (!target.IsOccupied && !board.ActiveUnits.Contains(bestTarget)) goodGuys.Remove(bestTarget);
                    System.Threading.Thread.Sleep(1000); //another 1 second pause for my slow brain.
                }
                else
                {
                    Console.WriteLine($"{badguy.Name} is not in range to attack {bestTarget.Name}!");
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }
        board.PrintMap();
    }
}