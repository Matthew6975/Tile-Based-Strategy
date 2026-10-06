using System.Runtime.Versioning;

public class Combat
{
    public void Attack(Unit attacker, Tile target, Board board)
    {
        if (attacker.EquippedWeapon == null)
        {
            Console.WriteLine($"{attacker.Name} has no weapon equipped!");
            return;
        }

        Weapon weapon = attacker.EquippedWeapon;

        int distance = CalculateDistance(attacker.X, attacker.Y, target.X, target.Y);

        if (distance > weapon.MaxRange)
        {
            Console.WriteLine("The target is out of range for {attacker.Name}'s {weapon.Name}!");
            return;
        }

        else if (distance < weapon.MinRange)
        {
            Console.WriteLine("The target is too close for {attacker.Name}'s {weapon.Name}!");
            return;
        }
        else
        {
            if (target.Occupant != null)
            {
                Unit defender = target.Occupant;
                Tile originTile = board.GetTile(attacker.X, attacker.Y);
                float hitChance = CalculateHitChance(attacker, originTile, target);
                int roll = new Random().Next(0, 101);
                // if roll is smaller (inside) or equal to the hit chance, the attack succeeds.
                if (roll <= hitChance)
                {
                    Console.WriteLine($"{attacker.Name} hits {defender.Name} for {attacker.EquippedWeapon.Damage}!");
                    TakeDamage(target.Occupant, attacker.EquippedWeapon.Damage, board);
                }
                else if (roll > hitChance)
                {
                    Console.WriteLine($"{attacker.Name} misses {target.Occupant.Name}!");
                }
            }
            else
            {
                // The shot hits the cover at the target coordinates. This should allow destructible cover later
                Console.WriteLine($"{attacker.Name} blasts the empty ground at ({target.X}, {target.Y})!");
            }           
            attacker.ActionPoints --;
        }    
    }

    public void TakeDamage (Unit target, int amount, Board board)
    {
        target.Health -= amount;
        if (target.Health <= 0)
        {
            board.DespawnUnit(target);
            Console.WriteLine($"{target.Name} has been defeated!");
        }
    }

    public int CalculateHitChance(Unit attacker, Tile originTile, Tile target)
    {
        if (attacker.EquippedWeapon == null)
        {
            Console.WriteLine($"{attacker.Name} has no weapon equipped!");
            return 0;
        }

        if (target.Occupant == null)
        {
            return 100; // If there is no occupant (i.e. an empty tile) the weapon is guarenteed to hit.
        }

        Weapon weapon = attacker.EquippedWeapon;

        CoverType cover = GetFacingCover(target, originTile);
        // Calculate hit chance based on weapon accuracy and cover
        int hitChance = weapon.Accuracy;

        if (cover == CoverType.Full)
        {
            hitChance = 0; // Full cover means no chance to hit
        }
        else if (cover == CoverType.Half)
        {
            hitChance /= 2; // Half cover reduces hit chance by half
        }
        return hitChance; 
    }


    public int CalculateDistance(int x1, int y1, int x2, int y2)
    {
        return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
    }

    public CoverType GetFacingCover(Tile targetTile, Tile originTile)
    {
        int deltaX = originTile.X - targetTile.X;
        int deltaY = originTile.Y - targetTile.Y;

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

    public List<Unit> GetValidTargets(Unit attacker, Board board)
    {
        List<Unit> validTargets = [];

        if (attacker.EquippedWeapon == null)
        {
            return validTargets; // No weapon equipped, no valid targets
        }

        foreach (Unit target in board.ActiveUnits)
        {
            if (target.Team == attacker.Team)
            {
                continue; // Skip units on the same team
            }
            else
            {
                int distance = CalculateDistance(attacker.X, attacker.Y, target.X, target.Y);
                if (distance >= attacker.EquippedWeapon.MinRange && distance <= attacker.EquippedWeapon.MaxRange)
                {
                    validTargets.Add(target);
                }
            }
        }
        return validTargets;
    }
}