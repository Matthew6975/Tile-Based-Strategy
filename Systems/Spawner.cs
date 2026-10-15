using System.Text.RegularExpressions;

public class Spawner(Board board)
{
    public void SpawnEnemies(MatchConfig match)
    {
        List<Tile> spawnOptions = board.GetEnemySpawnZone();
        List<Unit> enemiesToSpawn = match.EnemyUnits;

        foreach (Unit enemy in enemiesToSpawn)
        {
            if (spawnOptions.Count == 0) break; // break out of the loop if we run out of locations to spawn units.
            Tile spawnTile = spawnOptions[Random.Shared.Next(spawnOptions.Count)]; //pick random tile from the list.
            board.SpawnUnit(enemy, spawnTile.X, spawnTile.Y);

            spawnOptions.Remove(spawnTile);
        }
    }

    public void SpawnPlayers(MatchConfig match)
    {
        List<Tile> spawnOptions = board.GetPlayerSpawnZone();
        List<Unit> playersToSpawn = match.PlayerUnits;

        foreach (Unit unit in playersToSpawn)
        {
            if (spawnOptions.Count == 0) break; // break out of the loop if we run out of locations to spawn units.
            Tile spawnTile = spawnOptions[Random.Shared.Next(spawnOptions.Count)]; //pick random tile from the list.
            board.SpawnUnit(unit, spawnTile.X, spawnTile.Y);

            spawnOptions.Remove(spawnTile);
        }
    }
}
