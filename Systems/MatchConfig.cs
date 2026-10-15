// Systems/MatchConfig.cs
public class MatchConfig
{
    public int MapWidth { get; set; } = 10;
    public int MapHeight { get; set; } = 10;
    public int PlayerUnitCount { get; set; } = 2;
    public int EnemyUnitCount { get; set; } = 3;
    public List<Unit> EnemyUnits { get; set; } = [];
    public List<Unit> PlayerUnits { get; set; } = [];
}