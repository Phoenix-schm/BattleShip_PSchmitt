namespace BattleShip_PSchmitt
{
    class CPU : Player
    {
        Directions? _validDirection;
        List<Directions> _invalidDirection = [];
        List<int[]> _knownHitLocations = [];
        int _switchDirection = 0;
        public int directionListMax = Enum.GetValues(typeof(Directions)).Length;
        public int directionListMin = Enum.GetValues(typeof(Directions)).GetLowerBound(0) + 1;   // Gets the minimum dimension of Directions, minus Invalid

        /// <summary>
        /// Creates an Ocean grid with random ship placements
        /// </summary>
        /// <param name="cpuPlayer">The player having their grid created</param>
        /// <param name="rand">Random generator</param>
        /// <returns>The new ocean grid</returns>
        public static char[,] CreateCPUoceanGrid(Player cpuPlayer, Random rand)
        {
            char[,] oceanGrid = cpuPlayer.oceanGrid;
            List<Battleship> playerShipList = cpuPlayer.shipList;
            bool isValidCoordinates = false;
            CPU temporaryPlayer = new CPU();
            
            for (int index = 0; index < playerShipList.Count; index++)
            {
                Battleship chosenShip = playerShipList[index];    // chooses a ship
                do
                {
                    int y = rand.Next(0, oceanGrid.GetLength(0));
                    int x = rand.Next(0, oceanGrid.GetLength(1));
                    int randomDirection = rand.Next(temporaryPlayer.directionListMin, temporaryPlayer.directionListMax);

                    Directions directionChoice = Directions.Invalid;
                    foreach(Directions direction in Enum.GetValues(typeof(Directions)))
                    {
                        int directionInt = (int)direction;
                        if (randomDirection == directionInt)
                        {
                            directionChoice = direction;
                        }
                    }
                    cpuPlayer.oceanGrid = OceanGrid.PlaceShipOnOceanGrid(cpuPlayer, chosenShip, directionChoice, [y, x], ref isValidCoordinates);

                } while (!isValidCoordinates);
            }
            return cpuPlayer.oceanGrid;
        }

        /// <summary>
        /// Randomaly shoots somewhere on the target grid. If it hits, then run through hunting down the hit ship.
        /// </summary>
        /// <param name="cpu">The computer player.</param>
        /// <param name="opponentPlayer">The human player.</param>
        /// <param name="rand">Random variable</param>
        /// <returns>The "shoot" message that will be displayed, whether the cpu successfully shot a player ship.</returns>
        public static string CPUShotAI(CPU cpu, Player opponentPlayer, Random rand)
        {
            char[,] targetGrid = cpu.targetGrid;
            char[,] opponentOceanGrid = opponentPlayer.oceanGrid;
            int[]? previousShot = cpu.previousShot;

            bool isValidCoordinates = false;
            string message = "";
            char charAtIndex;

            if (IsAShipHitButNotSunk(opponentPlayer))           // if a ship was hit and it hasn't been sunk, we know that previousShot cannot be null
            {
                charAtIndex = opponentOceanGrid[previousShot[0], previousShot[1]];

                if (GameGrid.IsCharShipDisplayWhenHit(charAtIndex, opponentPlayer))             // if previous shot was a hit
                {
                    cpu._knownHitLocations.Add([previousShot[0], previousShot[1]]);       // Add to the list of known hit locations
                }

                if (charAtIndex == cpu.targetSunkDisplay)                                 // if there is a sunk ship and a hit ship at the same time
                {
                    cpu._knownHitLocations = RemoveSunkShipCoordinates(cpu, opponentPlayer);
                    cpu._switchDirection = 0;
                    cpu._invalidDirection.Clear();
                }
                // if the previous shot was a hit, but we don't have a valid direction
                if (GameGrid.IsCharShipDisplayWhenHit(charAtIndex, opponentPlayer) && cpu._validDirection == null)
                {
                    int randomDirection = rand.Next(cpu.directionListMin, cpu.directionListMax);
                    message = ShootAtDirection(cpu, opponentPlayer, previousShot[0], previousShot[1], randomDirection, rand);
                }
                // if the previous shot was a hit, and we have a valid direction
                else if (GameGrid.IsCharShipDisplayWhenHit(charAtIndex, opponentPlayer) && cpu._validDirection != null)
                {
                    message = ShootAtDirection(cpu, opponentPlayer, previousShot[0], previousShot[1], (int)cpu._validDirection, rand);
                }
                // if the previous shot missed, and we have a valid direction, and that valid direction has led to hits. and we haven't switch directions yet
                //      Note: switching directions needs breathing room for edge cases (such as ships being right next to each other right next to a border)
                else if (!GameGrid.IsCharShipDisplayWhenHit(charAtIndex, opponentPlayer) &&
                         cpu._validDirection != null && cpu._knownHitLocations.Count > 1 && cpu._switchDirection <= 2)
                {
                    switch ((Directions)cpu._validDirection)           // Go in the opposite direction
                    {
                        case Directions.Up:
                            cpu._validDirection = Directions.Down;
                            break;
                        case Directions.Down:
                            cpu._validDirection = Directions.Up;
                            break;
                        case Directions.Left:
                            cpu._validDirection = Directions.Right;
                            break;
                        case Directions.Right:
                            cpu._validDirection = Directions.Left;
                            break;
                    }

                    cpu._switchDirection++;
                    int[] firstValidHit = cpu._knownHitLocations[0];  // Shoot at the first known hit coordinates
                    message = ShootAtDirection(cpu, opponentPlayer, firstValidHit[0], firstValidHit[1], (int)cpu._validDirection, rand);
                }
                else // previous shot missed and we "don't" have a valid direction, but there is still a hit ship
                {
                    int[] firstValidHit = cpu._knownHitLocations[0];  // Shoot at the first known hit coordinates
                    int randomDirection = rand.Next(cpu.directionListMin, cpu.directionListMax);

                    message = ShootAtDirection(cpu, opponentPlayer, firstValidHit[0], firstValidHit[1], randomDirection, rand);
                }

            }
            else     // there is no ship that has been hit
            {
                if (previousShot != null)
                {
                    if (targetGrid[previousShot[0], previousShot[1]] == opponentPlayer.targetSunkDisplay)    // if the previousShot sunk a ship
                    {
                        cpu._validDirection = null;
                        cpu._invalidDirection.Clear();
                        cpu._knownHitLocations.Clear();
                        cpu._switchDirection = 0;
                    }
                }
                // reset _invalidDirections, validDirection, and knownHitLocations
                while (!isValidCoordinates)
                {
                    int y = rand.Next(0, targetGrid.GetLength(0));
                    int x = rand.Next(0, targetGrid.GetLength(1));
                    char charOnGrid = targetGrid[y, x];

                    if (charOnGrid == cpu.targetMissDisplay || charOnGrid == cpu.targetHitDisplay || charOnGrid == cpu.targetSunkDisplay)      // If it's already hit that spot
                    {
                        continue;
                    }
                    else
                    {
                        message = TargetGrid.PlaceShotsOnTargetGrid(cpu, opponentPlayer, y, x);
                        cpu.previousShot = [y, x];
                        isValidCoordinates = true;
                    }
                }
            }
            return message;
        }

        /// <summary>
        /// Runs through the list of opponent ships and checks if there is a sunk ship
        /// </summary>
        /// <param name="opponent">opponent player being checked</param>
        /// <returns></returns>
        static bool IsAShipHitButNotSunk(Player opponent)
        {
            bool isShipHitNotSunk = false;
            foreach (Battleship ship in opponent.shipList)
            {
                if (ship.IsHit)
                {
                    isShipHitNotSunk = true;
                }
            }

            return isShipHitNotSunk;
        }
        /// <summary>
        /// Removes sunk ship coordinates from _knownHitLocations list
        /// </summary>
        /// <param name="cpuPlayer">cpu player being modified</param>
        /// <param name="opponent">opponent player being checked</param>
        /// <returns>The new _knownHitLocations without the sunk ship coordinates</returns>
        static List<int[]> RemoveSunkShipCoordinates(CPU cpuPlayer, Player opponent)
        {
            char[,] opponentOceanGrid = opponent.oceanGrid;
            List<int[]> hitLocations = cpuPlayer._knownHitLocations;

            foreach (char position in opponentOceanGrid)
            {
                for (int i = 0; i < hitLocations.Count; i++)
                {
                    int[] index = hitLocations[i];
                    if (opponentOceanGrid[index[0], index[1]] == opponent.targetSunkDisplay)        // if a coordinate in hitlocations has a sunkShip char
                    {
                        cpuPlayer._knownHitLocations.RemoveAt(i);
                    }
                }
            }
            return cpuPlayer._knownHitLocations;
        }

        /// <summary>
        /// Runs through verifying a chosenDirection to see if the cpu can place a shot
        /// </summary>
        /// <param name="cpu">the cpu player shooting</param>
        /// <param name="previousShot">the previous shot taken by the cpu player</param>
        /// <param name="chosenDirection">the chosenDirection, represented as an int</param>
        /// <param name="rand">random variable</param>
        /// <returns>valid coordinates for the next shot and a valid direction</returns>
        static int[] PlaceShotInDirection(CPU cpu, int[] previousShot, int chosenDirection, Random rand)
        {
            bool isValidDirection = false;
            int[] checkCoordinates = [];

            while (!isValidDirection)
            {
                int y = previousShot[0];
                int x = previousShot[1];
                // if direction is invalid
                if (cpu._invalidDirection.Contains((Directions)chosenDirection))
                {
                    // choose a new direction that isn't in _invalidDirections
                    chosenDirection = rand.Next(cpu.directionListMin, cpu.directionListMax);
                }
                else // the Direction wasn't on the invalid List
                {
                    checkCoordinates = ModifyCoordinatesBasedOnDirection(chosenDirection, y, x);

                    if (checkCoordinates[0] > 9 || checkCoordinates[0] < 0)         // if y coordinates out of bounds
                    {
                        cpu._invalidDirection.Add((Directions)chosenDirection);
                    }
                    else if (checkCoordinates[1] > 9 || checkCoordinates[1] < 0)    // if x coordinates out of bounds
                    {
                        cpu._invalidDirection.Add((Directions)chosenDirection);
                    }
                    else if (cpu.targetGrid[checkCoordinates[0], checkCoordinates[1]] != '~') // if it's already hit that spot
                    {
                        cpu._invalidDirection.Add((Directions)chosenDirection);
                    }
                    else
                    {
                        cpu._validDirection = (Directions)chosenDirection;
                        isValidDirection = true;
                    }
                }
            }

            return checkCoordinates;
        }
        /// <summary>
        /// Modifies inputted coordinates based on direction
        /// </summary>
        /// <param name="direction">direction that will be converted to a Direction </param>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns>A new modified coordinate based on direction</returns>
        static int[] ModifyCoordinatesBasedOnDirection(int direction, int y, int x)
        {
            int[] newCoordinates = [];
            switch ((Directions)direction)
            {
                case Directions.Up:
                    newCoordinates = [--y, x];
                    break;
                case Directions.Down:
                    newCoordinates = [++y, x];
                    break;
                case Directions.Left:
                    newCoordinates = [y, --x];
                    break;
                case Directions.Right:
                    newCoordinates = [y, ++x];
                    break;
                default:
                    newCoordinates = [-1, - 1];
                    break;
            }

            return newCoordinates;
        }
        /// <summary>
        /// Places a shot at direction (may not end up being chosenDirection)
        /// </summary>
        /// <param name="cpu">cpu player doing the shooting</param>
        /// <param name="opponentPlayer">player being shot at</param>
        /// <param name="validHit_y">the valid coordinate y, (usually either peviousShot or from _knownHitList)</param>
        /// <param name="validHit_x">the valid coordinate x, (usually either peviousShot or from _knownHitList)</param>
        /// <param name="chosenDirection">the chosenDirection the shot will be attemoted at</param>
        /// <param name="rand">random variable</param>
        /// <returns>the Modified target grid of cpu, modified ocean grid of opponent, and the shot Message</returns>
        static string ShootAtDirection(CPU cpu, Player opponentPlayer, int validHit_y, int validHit_x, int chosenDirection, Random rand)
        {
            cpu.previousShot = PlaceShotInDirection(cpu, [validHit_y, validHit_x], chosenDirection, rand);    // add to previousShot based on random direction
            string message = TargetGrid.PlaceShotsOnTargetGrid(cpu, opponentPlayer, cpu.previousShot[0], cpu.previousShot[1]);   // Shoot in direction

            if (!GameGrid.IsCharShipDisplayWhenHit(opponentPlayer.oceanGrid[cpu.previousShot[0], cpu.previousShot[1]], opponentPlayer))    // if the direction didn't hit a ship
            {
                cpu._invalidDirection.Add((Directions)cpu._validDirection);
            }

            return message;
        }
    }
}
