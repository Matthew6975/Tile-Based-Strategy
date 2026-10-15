//The actual game logic that runs
class Program
{
    static void Main()
    {
        StartMenu startMenu = new();
        MatchConfig match = startMenu.Run();

        //Sapwn a board using the size selected during the menus 
        Board board = new(match.MapWidth, match.MapHeight);
        board.BuildMap();

        Spawner spawner = new(board);
        spawner.SpawnEnemies(match);
        spawner.SpawnPlayers(match);

        BattleManager battle = new();
        battle.RunBattle(board);
    }
}