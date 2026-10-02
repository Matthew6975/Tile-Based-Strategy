public class Combat
{
    public void Attack(Unit attacker, Unit defender, Grid grid)
    {
        if (attacker.EquippedWeapon == null)
        {
            Console.WriteLine($"{attacker.Name} has no weapon equipped!");
            return;
        }

        Weapon weapon = attacker.EquippedWeapon;

        int distance = CalculateDistance(attacker.X, attacker.Y, defender.X, defender.Y);

        if (distance > weapon.MaxRange)
        {
            Console.WriteLine($"{defender.Name} is out of range for {attacker.Name}'s {weapon.Name}!");
            return;
        }

        else if (distance < weapon.MinRange)
        {
            Console.WriteLine($"{defender.Name} is too close for {attacker.Name}'s {weapon.Name}!");
            return;
        }
        else
        {
            Tile defenderTile = grid.GetTile(defender.X, defender.Y);

            CoverType cover = GetFacingCover(defenderTile, attacker);
            // Calculate hit chance based on weapon accuracy and cover
            float hitChance = weapon.Accuracy;

            if (cover == CoverType.Full)
            {
                hitChance = 0.0f; // Full cover means no chance to hit
            }
            else if (cover == CoverType.Half)
            {
                hitChance *= 0.5f; // Half cover reduces hit chance by 50%
            }

            hitChance *= 100; // Convert to percentage (i.e., 0.75 becomes 75%)
            int roll = new Random().Next(0, 101);
            // if roll is smaller (inside) or equal to the hit chance, the attack succeeds.
            if (roll <= hitChance)
            {
                defender.Health -= weapon.Damage;
                Console.WriteLine($"{attacker.Name} hits {defender.Name} for {weapon.Damage} damage! {defender.Name} now has {defender.Health} health.");
            }
            else
            {
                Console.WriteLine($"{attacker.Name} misses {defender.Name}!");
            }

            if (defender.Health <= 0)
            {
                Console.WriteLine($"{defender.Name} has been defeated by {attacker.Name}!");
                grid.DespawnUnit(defender);
            }
        }    
    }


    public int CalculateDistance(int x1, int y1, int x2, int y2)
    {
        return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
    }

    public CoverType GetFacingCover(Tile targetTile, Unit attacker)
    {
        int deltaX = attacker.X - targetTile.X;
        int deltaY = attacker.Y - targetTile.Y;

        // Calculate the absolute values of the differences
        int absX = Math.Abs(deltaX);
        int absY = Math.Abs(deltaY);

        if (absX > absY)
        {
            return deltaX < 0 ? targetTile.WestCover : targetTile.EastCover;
        }
        else if (absY > absX)
        {
            return deltaY < 0 ? targetTile.NorthCover : targetTile.SouthCover; 
        }
        else
        {
            // If the differences are equal, we can choose either direction
            CoverType horizontalCover = deltaX < 0 ? targetTile.WestCover : targetTile.EastCover;
            CoverType verticalCover = deltaY < 0 ? targetTile.NorthCover : targetTile.SouthCover;

            if (horizontalCover == CoverType.Full || verticalCover == CoverType.Full)
            {
                return CoverType.Full;
            }
            else if (horizontalCover == CoverType.Half || verticalCover == CoverType.Half)
            {
                return CoverType.Half;
            }
            return CoverType.None;
        }
    }
}