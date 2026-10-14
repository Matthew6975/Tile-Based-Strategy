public class PlayerController(Combat combatSystem, Utilities utilities)
{
    public void RunMovementPreview(Unit activeUnit, Board board)
    {
        List<Tile> allowedSpaces = utilities.BreadthFirstSearch(activeUnit, board);

        // location of ghost entitiy for preview
        int targetX = activeUnit.X;
        int targetY = activeUnit.Y;

        bool isPreviewing = true;

        while (isPreviewing)
        {
            //pass in values so it displays the preview unit on the map, but no game logic sees it.
            board.PrintMap(targetX, targetY);
            Console.WriteLine("=== PREVIEW MODE===");
            Console.WriteLine($"Remaining movement: {activeUnit.CurrentMovePoints}");
            Console.WriteLine($"Previewing ({targetX}, {targetY})");
            Console.WriteLine("Press (W/A/S/D) to move, Enter to confirm, or Escape to cancel.");

            int newX = targetX;
            int newY = targetY;

            ConsoleKey moveKey = Console.ReadKey(true).Key;
            
            if (moveKey == ConsoleKey.W || moveKey == ConsoleKey.UpArrow) newY--; // Up
            else if (moveKey == ConsoleKey.S || moveKey == ConsoleKey.DownArrow) newY++; // Down
            else if (moveKey == ConsoleKey.A || moveKey == ConsoleKey.LeftArrow) newX--; // Left
            else if (moveKey == ConsoleKey.D || moveKey == ConsoleKey.RightArrow) newX++; // Right
            else if (moveKey == ConsoleKey.Escape) 
            {
                break;
            }
            // The players hits enter to confirm move decision
            else if (moveKey == ConsoleKey.Enter)
            {
                int cost = combatSystem.CalculateDistance(targetX, targetY, activeUnit.X, activeUnit.Y);
                board.MoveUnit(activeUnit, targetX, targetY);
                activeUnit.CurrentMovePoints -= cost;

                isPreviewing = false;
            }
            if (board.IsInBounds(newX, newY))
            {
                Tile targetTile = board.GetTile(newX, newY);

                if (allowedSpaces.Contains(targetTile))
                {
                    targetX = newX;
                    targetY = newY;
                }
            }
        } 
        board.PrintMap();
    }

    public void RunTargetingMode(Unit activeUnit, Board board)
    {
        if (activeUnit.ActionPoints <= 0)
            {
                Console.WriteLine($"{activeUnit.Name} is out of action points! Press any key...");
                Console.ReadKey(true);
                return;
            }

            List<Unit> validTargets = combatSystem.GetValidTargets(activeUnit, board);

            if (validTargets.Count == 0)
            {
                Console.WriteLine("No valid targets in range! Press any key to continue...");
                Console.ReadKey(true);
                return;
            }

            bool isTargeting = true;
            int targetIndex = 0;

            while (isTargeting)
            {
                //clear screen and start targeting mode UI
                board.PrintMap();

                Unit currentTarget = validTargets[targetIndex];
                Tile targetTile = board.GetTile(currentTarget.X, currentTarget.Y);
                Tile originTile = board.GetTile(activeUnit.X, activeUnit.Y);
                int hitChance = combatSystem.CalculateHitChance(activeUnit, originTile, targetTile);

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
                    combatSystem.Attack(activeUnit, targetTile, board); //This prints "who damaged who" dialogue.
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
            board.PrintMap();
            return;
    }
}