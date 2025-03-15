namespace BattleShip_PSchmitt
{
    class CPUPlayer : BasePlayer
    {
        Random random = new Random();
        DirectionList? _validDirection;
        List<DirectionList> _invalidDirections = [];
        List<int[]> _knownHitLocations = [];
        int _switchDirection = 0;
        int _directionListMax = Enum.GetValues(typeof(DirectionList)).Length;
        int _directionListMin = Enum.GetValues(typeof(DirectionList)).GetLowerBound(0) + 1;   // Gets the minimum dimension of Directions, minus Invalid

        public CPUPlayer(string newName)
        {
            name = newName;
        }
        /// <summary>
        /// Creates an Ocean grid with random ship placements
        /// </summary>
        /// <param name="cpuPlayer">The player having their grid created</param>
        /// <returns>The new ocean grid with every ship placed in random places and directions</returns>
        public static char[,] CreateCPUoceanGrid(BasePlayer cpuPlayer)
        {
            List<Battleship> playerShipList = cpuPlayer.shipList;
            CPUPlayer temporaryCPU = new CPUPlayer("temp");                        // Creation of a temporary CPU player for use of auto grid creation in Player players. (for debugging)
            Random random = temporaryCPU.random;

            for (int index = 0; index < playerShipList.Count; index++)
            {
                Battleship chosenShip = playerShipList[index];    // chooses a ship
                bool isValidCoordinates = false;

                while (!isValidCoordinates)
                {
                    int y = random.Next(0, cpuPlayer.oceanGrid.GetLength(0));
                    int x = random.Next(0, cpuPlayer.oceanGrid.GetLength(1));
                    int randomDirection = random.Next(temporaryCPU._directionListMin, temporaryCPU._directionListMax);

                    DirectionList directionChoice = DirectionList.Invalid;
                    foreach (DirectionList direction in Enum.GetValues(typeof(DirectionList)))
                    {
                        if (randomDirection == (int)direction)
                        {
                            directionChoice = direction;
                        }
                    }
                    // Modify oceanGrid with a random ship placement (continues while loop until the ship is placed)
                    cpuPlayer.oceanGrid = OceanGrid.PlaceShipOnOceanGrid(cpuPlayer, chosenShip, directionChoice, [y, x], ref isValidCoordinates);
                }
            }
            return cpuPlayer.oceanGrid;
        }

        /// <summary>
        /// Randomaly shoots somewhere on the target grid. If it hits, then run through hunting down the hit ship and shooting in a valid direction.
        /// </summary>
        /// <param name="cpu">The computer player.</param>
        /// <param name="opponentPlayer">The human player.</param>
        /// <param name="rand">Random variable</param>
        /// <returns>The "shoot" message that will be displayed, whether the cpu successfully shot a player ship.</returns>
        public static string CPUPlayerTurn(CPUPlayer cpu, BasePlayer opponentPlayer, Random rand)
        {
            char[,] targetGrid = cpu.targetGrid;
            char[,] opponentOceanGrid = opponentPlayer.oceanGrid;
            int[]? previousShot = cpu.previousShot;

            bool isValidCoordinates = false;
            string message = "";
            char charAtTargetIndex;

            if (IsAnOpponoentShipHit(opponentPlayer))               // if a ship was hit and it hasn't been sunk, we know that previousShot cannot be null
            {
                charAtTargetIndex = targetGrid[previousShot[0], previousShot[1]];
                bool isCharAtIndexHitShip = charAtTargetIndex == cpu.targetHitDisplay;

                if (isCharAtIndexHitShip)                                               // if previous shot was a hit
                {
                    cpu._knownHitLocations.Add([previousShot[0], previousShot[1]]);     // Add to the list of known hit locations
                }

                if (charAtTargetIndex == cpu.targetSunkDisplay)                         // if there is a sunk ship and a hit ship at the same time
                {
                    cpu._knownHitLocations = RemoveSunkShipCoordinates(cpu);
                    cpu._switchDirection = 0;
                    cpu._invalidDirections.Clear();
                }

                // if the previous shot was a hit, but we don't have a valid direction (Occurs at the first valid hit)
                if (isCharAtIndexHitShip && cpu._validDirection == null)
                {
                    int randomDirection = rand.Next(cpu._directionListMin, cpu._directionListMax);
                    message = ShootAtDirection(cpu, opponentPlayer, previousShot[0], previousShot[1], randomDirection, rand);
                }
                // if the previous shot was a hit, and we have a valid direction
                else if (isCharAtIndexHitShip && cpu._validDirection != null)
                {
                    message = ShootAtDirection(cpu, opponentPlayer, previousShot[0], previousShot[1], (int)cpu._validDirection, rand);
                }
                // if the previous shot missed, and we have a valid direction, and that valid direction has led to hits. and we haven't switch directions yet
                //      Note: switching directions needs breathing room for edge cases (such as ships being right next to each other right next to a border)
                else if (!isCharAtIndexHitShip && cpu._validDirection != null && cpu._knownHitLocations.Count > 1 && cpu._switchDirection <= 2)
                {
                    cpu._validDirection = GoInOppositeDirection((DirectionList)cpu._validDirection);

                    int[] firstHit = GetFirstHitOfLastShip(cpu, opponentPlayer);
                    cpu._switchDirection++;

                    message = ShootAtDirection(cpu, opponentPlayer, firstHit[0], firstHit[1], (int)cpu._validDirection, rand);
                }
                else // previous shot missed and we "don't" have a valid direction, but there is still a hit ship
                {    //  Note: _validDirection is never made null after the first assignment (For practical use in the above else if)
                    int[] firstHit = GetFirstHitOfLastShip(cpu, opponentPlayer);

                    int randomDirection = rand.Next(cpu._directionListMin, cpu._directionListMax);
                    message = ShootAtDirection(cpu, opponentPlayer, firstHit[0], firstHit[1], randomDirection, rand);
                }

            }
            else     // there is no ship that has been hit
            {
                if (previousShot != null)
                {
                    if (targetGrid[previousShot[0], previousShot[1]] == opponentPlayer.targetSunkDisplay)    // if the previousShot sunk a ship
                    {
                        // Reset all variables
                        cpu._validDirection = null;
                        cpu._invalidDirections.Clear();
                        cpu._knownHitLocations.Clear();
                        cpu._switchDirection = 0;
                    }
                }
                while (!isValidCoordinates)
                {
                    int y = rand.Next(0, targetGrid.GetLength(0));
                    int x = rand.Next(0, targetGrid.GetLength(1));

                    if (targetGrid[y, x] != '~')      // If it's already hit that spot
                    {
                        continue;
                    }
                    else
                    {
                        message = TargetGrid.PlaceShotsOnTargetGrid(cpu, opponentPlayer, y, x);
                        isValidCoordinates = true;
                    }
                }
            }
            return message;
        }

        /// <summary>
        /// Runs through the list of opponent ships and checks if there is a hit ship
        /// </summary>
        /// <param name="opponentPlayer">opponent player being checked</param>
        /// <returns>True if there is a hit ship, false if there is no hit ship</returns>
        static bool IsAnOpponoentShipHit(BasePlayer opponentPlayer)
        {
            bool isShipHitNotSunk = false;
            foreach (Battleship ship in opponentPlayer.shipList)
            {
                if (ship.IsHit)
                {
                    isShipHitNotSunk = true;
                    break;
                }
            }

            return isShipHitNotSunk;
        }

        /// <summary>
        /// Removes sunk ship coordinates from _knownHitLocations list
        /// </summary>
        /// <param name="cpuPlayer">cpu player being modified</param>
        /// <returns>The new _knownHitLocations without the sunk ship coordinates</returns>
        static List<int[]> RemoveSunkShipCoordinates(CPUPlayer cpuPlayer)
        {
            char[,] targetGrid = cpuPlayer.targetGrid;
            List<int[]> hitLocationsList = cpuPlayer._knownHitLocations;

            for (int index = 0; index < hitLocationsList.Count;)
            {
                int[] hitCoordinates = hitLocationsList[index];
                if (targetGrid[hitCoordinates[0], hitCoordinates[1]] == cpuPlayer.targetSunkDisplay)        // if a coordinate in hitlocations has a sunkShip char
                {
                    cpuPlayer._knownHitLocations.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }

            return cpuPlayer._knownHitLocations;
        }
        static DirectionList GoInOppositeDirection(DirectionList direction)
        {
            DirectionList chosenDirection = DirectionList.Invalid;
            switch (direction)           // Go in the opposite direction
            {
                case DirectionList.Up:
                    chosenDirection = DirectionList.Down;
                    break;
                case DirectionList.Down:
                    chosenDirection = DirectionList.Up;
                    break;
                case DirectionList.Left:
                    chosenDirection = DirectionList.Right;
                    break;
                case DirectionList.Right:
                    chosenDirection = DirectionList.Left;
                    break;
            }
            return chosenDirection;
        }

        /// <summary>
        /// Runs through verifying a chosenDirection to see if the cpu can place a shot
        /// if the chosenDirection wasn't valid, then it'll choose a new one until it can shoot
        /// *should* never cause a permanent while loop due to IsShipHitButNotSunk() and previous checks
        /// </summary>
        /// <param name="cpuPlayer">The cpu player shooting</param>
        /// <param name="lastValidHit">The last valid hit. Usually the cpu._previousShot</param>
        /// <param name="chosenDirection">The chosenDirection, represented as an int</param>
        /// <param name="rand">Random variable</param>
        /// <returns>Valid coordinates for the next shot. Adds to _validDirection and _invalidDirection</returns>
        static int[] PlaceShotInDirection(CPUPlayer cpuPlayer, BasePlayer opponentPlayer, int[] lastValidHit, int chosenDirection, Random rand)
        {
            bool isValidDirection = false;
            int[] checkCoordinates = [];
            bool edgeCaseSituation2 = false;
            bool edgeCaseSituation1 = false;
            int y_axis = 0;
            int x_axis = 1;

            while (!isValidDirection)
            {
                int y_shot = lastValidHit[y_axis];
                int x_shot = lastValidHit[x_axis];
                if (edgeCaseSituation1)
                {
                    y_shot = cpuPlayer._knownHitLocations[0][y_axis];
                    x_shot = cpuPlayer._knownHitLocations[0][x_axis];
                }

                if (cpuPlayer._invalidDirections.Count == 4)                            // occurs if two ships are right next to each other,
                {                                                                       //  it hit the latter ship in a middle section,
                                                                                        //  and the last hit is at a corner, resulting in all four directions being invalid
                    y_shot = GetFirstHitOfLastShip(cpuPlayer, opponentPlayer)[y_axis];
                    x_shot = GetFirstHitOfLastShip(cpuPlayer, opponentPlayer)[x_axis];
                    cpuPlayer._invalidDirections.Clear();
                    edgeCaseSituation2 = true;
                }

                if (cpuPlayer._invalidDirections.Contains((DirectionList)chosenDirection))      // if the chosenDirection is invalid
                {
                    // continuously choose a new direction until it isn't an invalid direction
                    chosenDirection = rand.Next(cpuPlayer._directionListMin, cpuPlayer._directionListMax);
                }
                else // the Direction wasn't on the invalid List
                {
                    checkCoordinates = ModifyCoordinatesBasedOnDirection(chosenDirection, y_shot, x_shot);
                    bool isCoordinatesOutOfBounds = checkCoordinates[y_axis] > 9 || checkCoordinates[y_axis] < 0 ||
                                                    checkCoordinates[x_axis] > 9 || checkCoordinates[x_axis] < 0;

                    if (isCoordinatesOutOfBounds && cpuPlayer._validDirection != null && cpuPlayer._switchDirection < 2)   // if two ships are right next to each other and the latter is at a border
                    {                                                                                                      // going in the chosenDirection will result in out of bounds
                                                                                                                           // so, go in the opposite direction from the first hit coordinates
                        edgeCaseSituation1 = true;                                                                         // this will probably result in a miss,
                        cpuPlayer._invalidDirections.Add((DirectionList)chosenDirection);
                        chosenDirection = (int)GoInOppositeDirection((DirectionList)cpuPlayer._validDirection);
                        cpuPlayer._switchDirection++;
                    }
                    else if (isCoordinatesOutOfBounds)  // If the first shot taken is out of bounds
                    {
                        cpuPlayer._invalidDirections.Add((DirectionList)chosenDirection);
                    }
                    else if (cpuPlayer.targetGrid[checkCoordinates[y_axis], checkCoordinates[x_axis]] != '~') // if it's already hit that spot
                    {
                        cpuPlayer._invalidDirections.Add((DirectionList)chosenDirection);
                    }
                    else if (edgeCaseSituation2 && opponentPlayer.oceanGrid[checkCoordinates[y_axis], checkCoordinates[x_axis]] == '~')    // technically cheating, if cpu won't hit a ship
                    {
                        // edgeCaseSituation reset cpu._invalidDirections
                        // edge case occurs at a corner so first two out of bounds, if else statements will result in an invalid direction
                        // and the last two directions will either be a ship or the ocean, so...
                        cpuPlayer._invalidDirections.Add((DirectionList)chosenDirection);
                    }
                    else
                    {
                        cpuPlayer._validDirection = (DirectionList)chosenDirection;
                        isValidDirection = true;
                    }
                }
            }

            return checkCoordinates;
        }
        /// <summary>
        /// Modifies inputed coordinates based on direction
        /// </summary>
        /// <param name="direction">direction that will be converted to a Direction </param>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns>A new modified coordinate based on direction</returns>
        static int[] ModifyCoordinatesBasedOnDirection(int direction, int y, int x)
        {
            int[] newCoordinates;
            switch ((DirectionList)direction)
            {
                case DirectionList.Up:
                    newCoordinates = [--y, x];
                    break;
                case DirectionList.Down:
                    newCoordinates = [++y, x];
                    break;
                case DirectionList.Left:
                    newCoordinates = [y, --x];
                    break;
                case DirectionList.Right:
                    newCoordinates = [y, ++x];
                    break;
                default:                            // in case Invalid somehow comes through
                    newCoordinates = [-1, -1];
                    break;
            }
            return newCoordinates;
        }

        /// <summary>
        /// Gets the first hit locations of the last ship that's been targeted
        /// </summary>
        /// <param name="cpuPlayer"></param>
        /// <param name="opponentPlayer"></param>
        /// <returns>Returns the coordinates of the first hit location of the last ship.</returns>
        static int[] GetFirstHitOfLastShip(CPUPlayer cpuPlayer, BasePlayer opponentPlayer)
        {
            // Player and CPU variables
            List<int[]> hitLocations = cpuPlayer._knownHitLocations;
            List<Battleship> shipList = opponentPlayer.shipList;
            char[,] oceanGrid = opponentPlayer.oceanGrid;

            // Variables related to hit coordinates
            int lastHitIndex = hitLocations.Count - 1;
            int[] lastHitLocation = hitLocations[lastHitIndex];

            Battleship? lastHitShip = null;
            foreach (Battleship ship in shipList)                                           // Going through ship list
            {
                if (ship.DisplayWhenHit == oceanGrid[lastHitLocation[0], lastHitLocation[1]])     // return the Battlship that was last hit
                {
                    lastHitShip = ship;
                    break;
                }
            }

            int[] firstHitLocationOfLastShip = [];
            for (int index = lastHitIndex; index >= 0; index--)                                 // Running through hitLocations backwards
            {
                lastHitLocation = hitLocations[index];
                if (oceanGrid[lastHitLocation[0], lastHitLocation[1]] == lastHitShip.DisplayWhenHit)
                {
                    firstHitLocationOfLastShip = [lastHitLocation[0], lastHitLocation[1]];      // Repeatidly change firstHitLocation...
                }
                else                                                                            // until it hits a new ship
                {
                    break;
                }
            }

            return firstHitLocationOfLastShip;
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
        static string ShootAtDirection(CPUPlayer cpu, BasePlayer opponentPlayer, int validHit_y, int validHit_x, int chosenDirection, Random rand)
        {
            cpu.previousShot = PlaceShotInDirection(cpu, opponentPlayer, [validHit_y, validHit_x], chosenDirection, rand);    // add to previousShot based on random direction
            string message = TargetGrid.PlaceShotsOnTargetGrid(cpu, opponentPlayer, cpu.previousShot[0], cpu.previousShot[1]);   // Shoot in direction

            if (cpu.targetGrid[cpu.previousShot[0], cpu.previousShot[1]] != cpu.targetHitDisplay)    // if the direction didn't hit a ship
            {
                cpu._invalidDirections.Add((DirectionList)cpu._validDirection); // Will never be null due to function always being entered with a validDirection
            }

            return message;
        }
    }
}
