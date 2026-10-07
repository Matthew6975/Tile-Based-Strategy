using System.Net.NetworkInformation;

/// <summary>
/// This class handles the turn order and resets unit movement, action points, and other abilities each turn loop.
/// Could modify to allow abilities to "cool down" over multiple turns if wanted
/// </summary>
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
                unit.ActionPoints = 1;
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
                unit.ActionPoints = 1;
            }
        }
    }

    /// <summary>
    /// passes turn between player and enemy units each time. This can be modified for additional turns, if desired.
    /// </summary>
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