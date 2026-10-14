public class BattleManager
{
    private readonly TurnManager _turnManager = new();
    private readonly Combat _combatSystem = new();
    private readonly EnemyAI _enemyAI = new();
    private readonly Pathing _pathing = new();
    private readonly PlayerController _playerController;
    public BattleManager()
    {
        _playerController = new PlayerController(_combatSystem, _pathing);
    }

    public void RunBattle(Board board)
    {
        board.PrintMap();
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

            // Player turn
            if (_turnManager.CurrentTurn == Team.Player)
            {
                isRunning = _playerController.ExecutePlayerTurn(goodGuys, _turnManager, board);
            }
            //Enemy turn
            else if (_turnManager.CurrentTurn == Team.Enemy)
            {
                _enemyAI.ExecuteEnemyTurn(board, badGuys, goodGuys, _pathing);
                _turnManager.EndCurrentTurn(board);
            }
        }
    }
}