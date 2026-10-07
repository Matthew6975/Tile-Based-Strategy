using System;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;

//The actual game logic that runs
class Program
{
    static void Main()
    {
        //Sapwn a board, modify some specific tiles
        Board board = new(10, 10);
        board.BuildMap();
        board.GetTile(3, 2).IsWalkable = false;
        board.GetTile(3, 3).IsWalkable = false;

        //create some unit instances
        Unit hero = new("Player", Team.Player);
        Unit enemy = new("Enemy", Team.Enemy);
        Unit foe = new("Foe", Team.Enemy);

        //Spawn each of the units
        board.SpawnUnit(hero, 0, 0);
        board.SpawnUnit(enemy, 4, 2);
        board.SpawnUnit(foe, 6, 6);

        //Initialize a basic weapon
        Weapon blaster = new()
        {
            Name = "Basic Blaster",
            Damage = 3,
            MinRange = 1,
            MaxRange = 5,
        };

        //equip the basic blaster to each of the unit instances
        hero.EquippedWeapon = blaster;
        foe.EquippedWeapon = blaster;
        enemy.EquippedWeapon = blaster;

        //a list of all units on the board
        List<Unit> allUnits = board.ActiveUnits;

        //initialize an instance of each class for function calls
        TurnManager turnManager = new();
        Combat combatSystem = new();
        EnemyAI enemyAI = new();
        Utilities utilities = new();

        //clear any old messages in the terminal and print the current state of the board.
        Console.Clear();
        board.PrintMap();

        //Start the game loop
        bool isRunning = true;
        while (isRunning)
        {
            // Initialize lists you want in the loop here
            List<Unit> badGuys = [];
            List<Unit> goodGuys = [];

            //Load the lists
            foreach (Unit unit in allUnits)
                {
                    if (unit.Team == Team.Enemy)
                    {
                        badGuys.Add(unit);
                    }
                    else if (unit.Team == Team.Player)
                    {
                        goodGuys.Add(unit);
                    }
                    else
                    {
                        Console.WriteLine($"Unit {unit.Name} is not on a team!");
                    }
                }

            // PLAYER TURN ---------------------|| targetKey == ConsoleKey.Tab)----------------------------------------------------------------------------------------------------------
            if (turnManager.CurrentTurn == Team.Player)
            {
                //print basic stats all the time
                Console.WriteLine($"Player position: ({hero.X}, {hero.Y})");
                Console.WriteLine($"Current move points: {hero.CurrentMovePoints}");
                Console.WriteLine($"Current player Health: {hero.Health}");
                Console.WriteLine("Enter a direction to move (W/A/S/D or Arrow Keys), F to target, E to pass turn, or Q to quit:");

                ConsoleKey key = Console.ReadKey(true).Key;

                int targetX = hero.X;
                int targetY = hero.Y;

                if (key == ConsoleKey.W || key == ConsoleKey.UpArrow) targetY--; // Up
                else if (key == ConsoleKey.S || key == ConsoleKey.DownArrow) targetY++; // Down
                else if (key == ConsoleKey.A || key == ConsoleKey.LeftArrow) targetX--; // Left
                else if (key == ConsoleKey.D || key == ConsoleKey.RightArrow) targetX++; // Right
                else if (key == ConsoleKey.Q) isRunning = false; // Quit
                else if (key == ConsoleKey.R) // Reset move points for "unlimited" movement. This will be removed after testing
                {
                    hero.CurrentMovePoints = hero.MaxMovePoints;
                    Console.WriteLine("Move p|| targetKey == ConsoleKey.Tab)oints reset!");
                    continue;
                }
                else if (key == ConsoleKey.F) // Enter targeting/attack mode
                {
                    List<Unit> validTargets = combatSystem.GetValidTargets(hero, board);

                    if (validTargets.Count == 0)
                    {
                        Console.WriteLine("No valid targets in range! Press any key to continue...");
                        Console.ReadKey(true);
                        continue;
                    }

                    bool isTargeting = true;
                    int targetIndex = 0;

                    while (isTargeting)
                    {
                        //clear screen and start targeting mode UI
                        Console.Clear();
                        board.PrintMap();

                        Unit currentTarget = validTargets[targetIndex];
                        Tile targetTile = board.GetTile(currentTarget.X, currentTarget.Y);
                        Tile originTile = board.GetTile(hero.X, hero.Y);
                        int hitChance = combatSystem.CalculateHitChance(hero, originTile, targetTile);

                        Console.WriteLine("=== TARGETING MODE ===");
                        Console.WriteLine($"Target: {currentTarget.Name} at ({currentTarget.X}, {currentTarget.Y})");
                        Console.WriteLine($"Hit Chance: {hitChance}% | Target Health: {currentTarget.Health}");
                        Console.WriteLine("Use Left/Right Arrows or Tab to cycle targets.");
                        Console.WriteLine("Press Enter or F to Fire. Press Esc or Q to Cancel.");

                        ConsoleKey targetKey = Console.ReadKey(true).Key;

                        //Cycle through available targets. Allows looping.
                        if (targetKey == ConsoleKey.RightArrow)
                        {
                            targetIndex++;
                            if(targetIndex >= validTargets.Count) targetIndex = 0;
                        }
                        else if (targetKey == ConsoleKey.LeftArrow)
                        {
                            targetIndex--;
                            if (targetIndex < 0) targetIndex = validTargets.Count -1;
                            
                        }
                        //launch the attack against the selected target.
                        else if (targetKey == ConsoleKey.F || targetKey == ConsoleKey.Enter)
                        {
                            Console.Clear();
                            combatSystem.Attack(hero, targetTile, board); //This prints "who damaged who" dialogue.
                            Console.WriteLine("press any key to continue...");
                            Console.ReadKey(true);
                            isTargeting = false; //end targeting UI   
                        }
                        //end targeting UI
                        else if (targetKey == ConsoleKey.Q || targetKey == ConsoleKey.Escape)
                        {
                            isTargeting = false;
                        }
                    }
                    //print a new, clean board and repeat loop
                    Console.Clear();
                    board.PrintMap();
                    continue;
                }
                //pass turn to the enemy AI
                else if (key == ConsoleKey.E)
                {
                    turnManager.EndCurrentTurn(board);
                    continue;
                }
                //If any other key is pressed, hadle the exception and re-loop
                else
                {
                    Console.WriteLine($"Ignored key: {key}. Use WASD or Arrow Keys.");
                    continue;
                }
                
                //Check if the player unit is out of movement for this turn
                if (hero.CurrentMovePoints <= 0)
                {
                    Console.Clear();
                    board.PrintMap();
                    Console.WriteLine("No more move points available!");
                    continue;
                }

                //if unit still has movement, move the unit and deduct the movement points
                Console.Clear();
                board.MoveUnit(hero, targetX, targetY);
                hero.CurrentMovePoints--;
                board.PrintMap();
            }


            //ENEMY TURN -----------------------------------------------------------------------------------------------------------------------
            else if (turnManager.CurrentTurn == Team.Enemy)
            {
                //print fresh map state
                board.PrintMap();
                Console.WriteLine("Enemy is thinking...");
                System.Threading.Thread.Sleep(1000); // a 1 sec pause so I can see the enemy actions

                // Giant loop to aggregate, score, and execute each of the enemy's potential move options.
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
                                int score = enemyAI.ScoreAction(badguy, tile, playerTile, board);

                                if (score > highScore)
                                {
                                    bestDest = tile;
                                    bestTarget = player;
                                    highScore = score;


                                    // need to fix the fact that if the enemy cannot reach the player this turn with their attack, they will default to not moving
                                    //this is beacause bestDest demands a non-null value, so I defaulted it to it's own tile. Need to default to an enemy unit.
                                    
                                    // if (highScore == 0 && goodGuys.Count > 0)
                                    // {
                                    //     Unit nearestPlayer = goodGuys[0];
                                    //     int shortestDistance = int.MaxValue;

                                    //     foreach (Unit unit in goodGuys)
                                    //     {
                                    //         int distance = combatSystem.CalculateDistance(unit.X, unit.Y, badguy.X, badguy.Y);
                                    //         if (distance < shortestDistance)
                                    //         {
                                    //             shortestDistance = distance;
                                    //             bestDest = board.GetTile(player.X, player.Y);
                                    //         }
                                    //     }
                                    // }
                                }
                            }
                        }
                    }

                    //Once the best path/target/destination has been chosen, actually move to execute this course of action.
                    List<Tile> path = enemyAI.GetAStarPath(badguy, bestDest, board);
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

                        //if there is a target and the AI has a weapon, initiate an attack.
                        if(bestTarget != null && badguy.EquippedWeapon != null)
                        {
                            Tile target = board.GetTile(bestTarget.X, bestTarget.Y);
                            combatSystem.Attack(badguy, target, board);
                            System.Threading.Thread.Sleep(1000); //another pause for my slow brain.
                        }
                    }
                }
                board.PrintMap();
                turnManager.EndCurrentTurn(board);
            }
        }
    }
}