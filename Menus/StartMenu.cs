public class StartMenu
{
    public MatchConfig Run()
    {
        MatchConfig config = new();
        ConfigureMatchSettings(config);
        ConfigurePlayerSquad(config);
        ConfigureEnemySquad(config);
        return config;
    }

    private void ConfigureMatchSettings(MatchConfig config)
    {
        int selectedIndex = 0;
        bool isConfiguring = true;

        while (isConfiguring)
        {
            Console.Clear();
            Console.WriteLine("=== MATCH SETUP ===");
            
            // Draw the menu
            string[] options = [
                $"Map Width:    < {config.MapWidth} >",
                $"Map Height:   < {config.MapHeight} >",
                $"Player Units: < {config.PlayerUnitCount} >",
                $"Enemy Count:  < {config.EnemyUnitCount} >",
                "[ Next: Squad Loadout ]"
            ];

            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine(i == selectedIndex ? $"> {options[i]}" : $"  {options[i]}");
            }

            // Handle Input
            ConsoleKey key = Console.ReadKey(true).Key;
            
            if (key == ConsoleKey.UpArrow) selectedIndex = (selectedIndex - 1 + options.Length) % options.Length;
            else if (key == ConsoleKey.DownArrow) selectedIndex = (selectedIndex + 1) % options.Length;
            else if (key == ConsoleKey.LeftArrow || key == ConsoleKey.RightArrow)
            {
                int delta = key == ConsoleKey.RightArrow ? 1 : -1;
                
                if (selectedIndex == 0) config.MapWidth = Math.Clamp(config.MapWidth + delta, 5, 30);
                else if (selectedIndex == 1) config.MapHeight = Math.Clamp(config.MapHeight + delta, 5, 30);
                else if (selectedIndex == 2) config.PlayerUnitCount = Math.Clamp(config.PlayerUnitCount + delta, 1, 4);
                else if (selectedIndex == 3) config.EnemyUnitCount = Math.Clamp(config.EnemyUnitCount + delta, 1, 10);
            }
            else if (key == ConsoleKey.Enter && selectedIndex == 4)
            {
                isConfiguring = false;
            }
        }
    }

    private void ConfigurePlayerSquad(MatchConfig config)
    {
        int[] weaponSelections = new int[config.PlayerUnitCount];
        int selectedIndex = 0;
        int totalRows = config.PlayerUnitCount + 1; // +1 allows for a start button option after all the units
        bool isEquipping = true;

        while (isEquipping)
        {
            Console.Clear();
            Console.WriteLine("=== SQUAD LOADOUT ===");

            for (int i = 0; i < config.PlayerUnitCount; i++)
            {
                // Look up the currently selected weapon for this specific unit
                Weapon w = Armory.Weapons[weaponSelections[i]];
                string line = $"Unit {i + 1} Weapon: < {w.Name} > (DMG: {w.Damage}), (Range: {w.MaxRange})";
                
                // Print with the cursor check (i == selectedIndex) just like Stage 1
                Console.WriteLine(i == selectedIndex ? $"> {line}" : $"  {line}");
            }

            Console.WriteLine(selectedIndex == config.PlayerUnitCount ? "\n> [ CONFIRM PLAYER LOADOUT ]" : "\n  [ CONFIRM PLAYER LOADOUT ]");

            ConsoleKey key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow) selectedIndex = (selectedIndex - 1 + totalRows) % totalRows;
            else if (key == ConsoleKey.DownArrow) selectedIndex = (selectedIndex + 1) % totalRows;
            else if ((key == ConsoleKey.LeftArrow || key == ConsoleKey.RightArrow) && selectedIndex < config.PlayerUnitCount)
            {
                int count = Armory.Weapons.Count;
                int delta = key == ConsoleKey.RightArrow ? 1 : -1;
                weaponSelections[selectedIndex] = (weaponSelections[selectedIndex] + delta + count) % count;
            }
            else if (key == ConsoleKey.Enter && selectedIndex == config.PlayerUnitCount)
            {
                // Instantiate the actual Unit objects with their chosen weapons here
                for (int i = 0; i < config.PlayerUnitCount; i++)
                {
                    Unit unit = new($"Operative {i + 1}", Team.Player)
                    {
                        EquippedWeapon = Armory.Weapons[weaponSelections[i]]
                    };
                    config.PlayerUnits.Add(unit);
                }
                isEquipping = false;
            }
        } 
    }


    private void ConfigureEnemySquad(MatchConfig config)
    {
        int[] weaponSelections = new int[config.EnemyUnitCount];
        int selectedIndex = 0;
        int totalRows = config.EnemyUnitCount + 1; // +1 allows for a start button option after all the units
        bool isEquipping = true;

        while (isEquipping)
        {
            Console.Clear();
            Console.WriteLine("=== ENEMY LOADOUT ===");

            for (int i = 0; i < config.EnemyUnitCount; i++)
            {
                // Look up the currently selected weapon for this specific unit
                Weapon w = Armory.Weapons[weaponSelections[i]];
                string line = $"Unit {i + 1} Weapon: < {w.Name} > (DMG: {w.Damage}), (Range: {w.MaxRange})";
                
                Console.WriteLine(i == selectedIndex ? $"> {line}" : $"  {line}");
            }

            Console.WriteLine(selectedIndex == config.EnemyUnitCount ? "\n> [ CONFIRM ENEMY LOADOUT ]" : "\n  [ CONFIRM ENEMY LOADOUT ]");

            ConsoleKey key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow) selectedIndex = (selectedIndex - 1 + totalRows) % totalRows;
            else if (key == ConsoleKey.DownArrow) selectedIndex = (selectedIndex + 1) % totalRows;
            else if ((key == ConsoleKey.LeftArrow || key == ConsoleKey.RightArrow) && selectedIndex < config.EnemyUnitCount)
            {
                int count = Armory.Weapons.Count;
                int delta = key == ConsoleKey.RightArrow ? 1 : -1;
                weaponSelections[selectedIndex] = (weaponSelections[selectedIndex] + delta + count) % count;
            }
            else if (key == ConsoleKey.Enter && selectedIndex == config.EnemyUnitCount)
            {
                // Instantiate the actual Unit objects with their chosen weapons here
                for (int i = 0; i < config.EnemyUnitCount; i++)
                {
                    Unit unit = new($"Enemy {i + 1}", Team.Enemy)
                    {
                        EquippedWeapon = Armory.Weapons[weaponSelections[i]]
                    };
                    config.EnemyUnits.Add(unit);
                }
                isEquipping = false;
            }
        } 
    }
}