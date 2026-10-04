using System.Net.NetworkInformation;

public class TurnManager
{
    public Team CurrentTurn { get; private set; } = Team.Player;

    public void StartPlayerTurn (Board board)
    {
        CurrentTurn = Team.Player;

        foreach (Unit unit in board.ActiveUnits)
        {
            if (unit.Team == Team.Player)
            {
                unit.CurrentMovePoints = unit.MaxMovePoints;
            }
        }
    }

    public void StartEnemyTurn (Board board)
    {
        CurrentTurn = Team.Enemy;

        foreach (Unit unit in board.ActiveUnits)
        {
            if (unit.Team == Team.Enemy)
            {
                unit.CurrentMovePoints = unit.MaxMovePoints;
            }
        }
    }

    public void EndCurrentTurn(Board board)
    {
        // could add a check for extra turns or whatnot here. That is for later though

        if (CurrentTurn == Team.Player)
        {
            StartEnemyTurn(board);
        }
        else if (CurrentTurn == Team.Enemy)
        {
            StartPlayerTurn(board);
        }
    }
}