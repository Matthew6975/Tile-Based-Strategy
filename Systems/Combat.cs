using System.Runtime.Versioning;
public class Combat
{
    ///<summary>
    /// Let's a unit attack a tile in range of their weapon.
    /// </summary>
    public void Attack(Unit attacker, Tile target, Board board)
    {
        if (attacker.EquippedWeapon == null)
        {
            Console.WriteLine($"{attacker.Name} has no weapon equipped!");
            return;
        }

        Weapon weapon = attacker.EquippedWeapon;

        int distance = CalculateDistance(attacker.X, attacker.Y, target.X, target.Y);

        //Is the target within the weapons range(s)?
        if (distance > weapon.MaxRange)
        {
            Console.WriteLine($"The target is out of range for {attacker.Name}'s {weapon.Name}!");
            return;
        }

        else if (distance < weapon.MinRange)
        {
            Console.WriteLine($"The target is too close for {attacker.Name}'s {weapon.Name}!");
            return;
        }
        else
        {
            //If there is a target, roll to hit
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
                // The shot hits the cover at the target coordinates with 100% accuracy. This should allow destructible cover later
                Console.WriteLine($"{attacker.Name} blasts the empty ground at ({target.X}, {target.Y})!");
            }
            // if the attack happens, subtract a unit's action point           
            attacker.ActionPoints --;
        }    
    }

    ///<summary>
    /// This has been decoupled from attack so we can add environmental damage or status effects
    /// or any type of damage that does not come from another unit later. Also does death checks and despawns units.
    /// </summary>
    public void TakeDamage (Unit target, int amount, Board board)
    {
        target.Health -= amount;
        if (target.Health <= 0)
        {
            board.DespawnUnit(target);
            Console.WriteLine($"{target.Name} has been defeated!");
        }
    }

    ///<summary>
    /// This function checks an atacker and a target and applies HitChance modifiers if the target is in cover.
    /// If the target is an empty space, the hit chance is automatically set to 100%
    /// </summary> 
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

    /// <summary>
    /// Simply checks the delta between two spaces using the manhattan algorithm
    /// yeilding the amount of spaces/movment needed to reach the target.
    /// </summary>
    public int CalculateDistance(int x1, int y1, int x2, int y2)
    {
        return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
    }

    /// <summary>
    /// This functons uses the target and targeter tiles to check if there is directional cover that will block an incoming attack
    /// </summary>
    public CoverType GetFacingCover(Tile targetTile, Tile originTile)
    {
        //calc the relative deltas between x and y coordinates
        int deltaX = originTile.X - targetTile.X;
        int deltaY = originTile.Y - targetTile.Y;

        // Calculate the absolute values of the differences
        int absX = Math.Abs(deltaX);
        int absY = Math.Abs(deltaY);

        //if the greatest difference is on the X-axis, the unit is primarily to the left or right of the target,
        //so one of the side covers will be used in the calculation.
        if (absX > absY)
        {
            //is the relative value positive or negative? this tells us if the unit is on the left or the right of the target,
            // and therefore which side's cover to use.
            return deltaX < 0 ? targetTile.WestCover : targetTile.EastCover;
        }
        // if the greatest difference is on the Y-axis, the unit is primarily above or below the target,
        // so the upper or lower cover will be used in the calculation.
        else if (absY > absX)
        {
            // is the relative value positive or negative? This will tell us if the unit is above or below the target
            // and therefor which cover to use.
            return deltaY < 0 ? targetTile.NorthCover : targetTile.SouthCover; 
        }
        else //the absolute delta's are equal, meaning the attack is coming from a perfect diagonal.
        {
            //since it is a corner, we will use whichever cover value is larger.
            //here, we are calculating which corner the attack is hitting so we can apply the correct cover values.

            //are they to the left or right of us?
            CoverType horizontalCover = deltaX < 0 ? targetTile.WestCover : targetTile.EastCover;
            //are they above or below us?
            CoverType verticalCover = deltaY < 0 ? targetTile.NorthCover : targetTile.SouthCover;

            //if either cover is full cover, apply full cover.
            if (horizontalCover == CoverType.Full || verticalCover == CoverType.Full)
            {
                return CoverType.Full;
            }
            //if neither side has full cover but at least one is half cover, apply half cover.
            else if (horizontalCover == CoverType.Half || verticalCover == CoverType.Half)
            {
                return CoverType.Half;
            }
            //we will only hit this block if neither side has any cover at all. In that case, the defender gets no cover from the tile.
            return CoverType.None;
        }
    }

    /// <summary>
    /// This function loops through all units on the board and returns a list of any that are on different teams and within weapon range
    /// </summary>
    public List<Unit> GetValidTargets(Unit attacker, Board board)
    {
        List<Unit> validTargets = [];

        // No weapon equipped, return empty list since you cannot attack. I may want to remove this check later, but we'll see.
        if (attacker.EquippedWeapon == null)
        {
            return validTargets; 
        }

        //if the unit is not on your team, and they are in range of your weapon, add them to the list.
        //Once all the loops are finished, return the loaded list.
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