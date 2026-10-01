public class Combat
{
    public void Attack(Unit attacker, Unit defender)
    {
        if (attacker.EquippedWeapon == null)
        {
            Console.WriteLine($"{attacker.Name} has no weapon equipped!");
            return;
        }

        Weapon weapon = attacker.EquippedWeapon;

        int distance = CalculateDistance(attacker.X, attacker.Y, defender.X, defender.Y);

        //This is not done yet, keep adding more.
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