public class BattleManager
{
    private readonly TurnManager _turnManager = new();
    private readonly Combat _combatSystem = new();
    private readonly EnemyAI _enemyAI = new();
    private readonly Utilities _utilities = new();
    private readonly PlayerController _playerController;
    public BattleManager()
    {
        _playerController = new PlayerController(_combatSystem, _utilities);
    }

    public void RunBattle(Board board)
    {
        board.PrintMap();
        int selectedUnitIndex = 0;
        bool isRunning = true;
        while (isRunning)
        {
            // Initialize lists you want in the loop here
            List<Unit> badGuys = [];
            List<Unit> goodGuys = [];

            //Load the lists
            foreach (Unit unit in board.ActiveUnits)
                {
                    if (unit.Team == Team.Enemy) badGuys.Add(unit);
                    else if (unit.Team == Team.Player) goodGuys.Add(unit);
                    else Console.WriteLine($"Unit {unit.Name} is not on a team!");
                }
                // Win / Loss Checks
            if (goodGuys.Count == 0)
            {
                Console.WriteLine("All player units defeated! Game Over.");
                break;
            }
            if (badGuys.Count == 0)
            {
                Console.WriteLine("All enemies defeated! Victory!");
                break;
            }

            // PLAYER TURN \\-----------------------------------------------------------------------------------------------------------------
            if (_turnManager.CurrentTurn == Team.Player)
            {
                // Safety check: if all player units were defeated, end the game
                if (goodGuys.Count == 0)
                {
                    Console.WriteLine("All player units defeated! Game Over.");
                    isRunning = false;
                    continue;
                }

                // Clamp index in case a unit died last turn and shrunk the list
                if (selectedUnitIndex >= goodGuys.Count)
                {
                    selectedUnitIndex = 0;
                }

                Unit activeUnit = goodGuys[selectedUnitIndex];


                // Print stats for whichever unit is currently selected
                Console.WriteLine($"=== Selected Unit: {activeUnit.Name} ({selectedUnitIndex + 1}/{goodGuys.Count}) ===");
                Console.WriteLine($"Position: ({activeUnit.X}, {activeUnit.Y}) | Health: {activeUnit.Health}");
                Console.WriteLine($"Move Points: {activeUnit.CurrentMovePoints} | Action Points: {activeUnit.ActionPoints}");
                Console.WriteLine("Press [Tab] to switch unit, [M] to move, [F] to target, [E] to pass turn, or [Q] to quit:");

                ConsoleKey key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Q) isRunning = false; // Quit
                else if (key == ConsoleKey.R) // Reset move points for "unlimited" movement. This will be removed after testing
                {
                    activeUnit.CurrentMovePoints = activeUnit.MaxMovePoints;
                    Console.WriteLine("Move points reset!");
                    continue;
                }

                else if (key == ConsoleKey.Tab)
                {
                    selectedUnitIndex++;
                    if (selectedUnitIndex >= goodGuys.Count)
                    {
                        selectedUnitIndex = 0; // Wrap back to the first unit
                    }
                    board.PrintMap();
                    continue;
                }

                // The player is moving their unit
                else if (key == ConsoleKey.M || key == ConsoleKey.LeftArrow || key == ConsoleKey.RightArrow || key == ConsoleKey.UpArrow || key == ConsoleKey.DownArrow)
                {
                    _playerController.RunMovementPreview(activeUnit, board);
                } 
                else if (key == ConsoleKey.F) // Enter targeting/attack mode
                {
                    _playerController.RunTargetingMode(activeUnit, board);  
                }
                //pass turn to the enemy AI
                else if (key == ConsoleKey.E)
                {
                    _turnManager.EndCurrentTurn(board);
                }
                //If any other key is pressed, hadle the exception and re-loop
                else
                {
                    Console.Clear();
                    board.PrintMap();
                    Console.WriteLine($"Ignored key: {key}. Use WASD or Arrow Keys.");
                    continue;
                }
            }

            //ENEMY TURN -----------------------------------------------------------------------------------------------------------------------
            else if (_turnManager.CurrentTurn == Team.Enemy)
            {
                _enemyAI.ExecuteEnemyTurn(board, badGuys, goodGuys, _utilities);
                _turnManager.EndCurrentTurn(board);
            }
        }
    }
}