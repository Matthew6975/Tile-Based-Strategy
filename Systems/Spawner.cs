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

    public void SpawnCover(MatchConfig match)
    {
        List<Tile> activeTiles = [];
        List<Tile> spawnOptions = [];

        for (int y = 0; y < match.MapHeight; y++)
        {
            for (int x = 0; x < match.MapWidth; x++)
            {
                Tile currentTile = board.GetTile(x, y);
                if (currentTile.IsActive) activeTiles.Add(currentTile);

                if (currentTile.IsWalkable && !currentTile.IsOccupied && currentTile.IsActive) spawnOptions.Add(currentTile);
            }
        }

        int desiredCover = match.CoverChoice switch
        {
            CoverLevel.Sparse => 15,
            CoverLevel.Normal => 25,
            CoverLevel.Dense  => 40,
            _                 => 0
        };

        // int desiredCover = 0;
        // if (match.CoverChoice == CoverLevel.Off) desiredCover = 0;
        // else if (match.CoverChoice == CoverLevel.Sparse) desiredCover = 15;
        // else if (match.CoverChoice == CoverLevel.Normal) desiredCover = 25;
        // else if (match.CoverChoice == CoverLevel.Dense) desiredCover = 40;

        int numCoverTiles = activeTiles.Count * desiredCover /100;

        for (int i = 0; i < numCoverTiles; i++)
        {
            if (spawnOptions.Count == 0) break;

            Tile spawnTile = spawnOptions[Random.Shared.Next(spawnOptions.Count)];

            spawnTile.NorthCover = CoverType.Full;
            spawnTile.SouthCover = CoverType.Full;
            spawnTile.WestCover = CoverType.Full;
            spawnTile.EastCover = CoverType.Full;

            spawnTile.IsWalkable = false;

            spawnOptions.Remove(spawnTile);
        }
    }
}
