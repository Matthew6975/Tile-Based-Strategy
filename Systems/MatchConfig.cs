// Systems/MatchConfig.cs
public class MatchConfig
{
    public int MapWidth { get; set; } = 10;
    public int MapHeight { get; set; } = 10;
    public int PlayerUnitCount { get; set; } = 2;
    public int EnemyCount { get; set; } = 3;
    public List<Unit> PlayerUnits { get; set; } = [];
}