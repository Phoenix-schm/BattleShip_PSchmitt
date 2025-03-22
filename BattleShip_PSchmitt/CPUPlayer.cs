namespace BattleShip_PSchmitt
{
    class CPUPlayer : BasePlayer
    {
        Random random = new Random();
        DirectionList? _validDirection;
        List<DirectionList> _invalidDirections = [];
        List<int[]> _knownHitLocations = [];
        
        int _switchDirection = 0;
        int _targetShip = 0;

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
        /// <param name="cpuPlayer">The computer player.</param>
        /// <param name="opponentPlayer">The opponent player. Usually a HumanPlayer</param>
        /// <param name="rand">Random variable</param>
        /// <returns>The "shoot" message that will be displayed, whether the cpu successfully shot a player ship.</returns>
        public static string CPUPlayerTurn(CPUPlayer cpuPlayer, BasePlayer opponentPlayer, Random rand)
        {
            char[,] targetGrid = cpuPlayer.targetGrid;
            char[,] opponentOceanGrid = opponentPlayer.oceanGrid;
            int[]? previousShot = cpuPlayer.previousShot;

            string shotMessage = "";

            if (IsOpponoentShipHit(opponentPlayer))               // if a ship was hit and it hasn't been sunk, we know that previousShot cannot be null
            {
                char charAtTargetIndex = targetGrid[previousShot[0], previousShot[1]];
                bool isCharAtIndexHitShip = charAtTargetIndex == cpuPlayer.targetHitDisplay;

                if (isCharAtIndexHitShip)                                                       // if previous shot was a hit
                {
                    cpuPlayer._knownHitLocations.Add([previousShot[0], previousShot[1]]);       // Add to the list of known hit locations
                }

                if (charAtTargetIndex == cpuPlayer.targetSunkDisplay)                           // if there is a sunk ship and a hit ship at the same time
                {
                    cpuPlayer._knownHitLocations = RemoveSunkShipCoordinates(cpuPlayer);
                    cpuPlayer._switchDirection = 0;
                    cpuPlayer._targetShip = 0;
                    cpuPlayer._invalidDirections.Clear();
                }

                // if the previous shot was a hit, but we don't have a valid direction (Occurs at the first valid hit)
                if (isCharAtIndexHitShip && cpuPlayer._validDirection == null)
                {
                    int randomDirection = rand.Next(cpuPlayer._directionListMin, cpuPlayer._directionListMax);
                    shotMessage = ShootAtDirection(cpuPlayer, opponentPlayer, previousShot[0], previousShot[1], randomDirection, rand);
                }
                // if the previous shot was a hit, and we have a valid direction
                else if (isCharAtIndexHitShip && cpuPlayer._validDirection != null)
                {
                    shotMessage = ShootAtDirection(cpuPlayer, opponentPlayer, previousShot[0], previousShot[1], (int)cpuPlayer._validDirection, rand);
                }
                // if previous shot missed, but current direction has led to hits and we haven't switched directions yet.
                else if (!isCharAtIndexHitShip && cpuPlayer._knownHitLocations.Count > 1 && cpuPlayer._switchDirection < 1)
                {
                    // go in the opposite direction from the first hit coordinates
                    cpuPlayer._invalidDirections.Clear();           // Must clear invalidDirection as opposite direction was invalid in previousShot
                    cpuPlayer._validDirection = GoInOppositeDirection((DirectionList)cpuPlayer._validDirection);
                    cpuPlayer._switchDirection++;

                    shotMessage = ShootAtDirection(cpuPlayer, opponentPlayer, previousShot[0], previousShot[1], (int)cpuPlayer._validDirection, rand);
                }
                // if previous shot missed, but we've already switched directions, so we need to go in an perpendicular direction
                else if (!isCharAtIndexHitShip && cpuPlayer._switchDirection == 2)
                {
                    int [] firstValidHit = GetTargetShip(cpuPlayer, opponentPlayer);
                    GetRandomValidDirection(cpuPlayer, rand);
                    cpuPlayer._switchDirection++;

                    shotMessage = ShootAtDirection(cpuPlayer, opponentPlayer, firstValidHit[0], firstValidHit[1], (int)cpuPlayer._validDirection, rand);
                }
                // we've already switched directions and gone in a perpendicular direction but the previousShot missed
                // we go in the oppsoite direction of perpendicular direction
                else if (!isCharAtIndexHitShip && cpuPlayer._switchDirection == 3 && cpuPlayer._validDirection != null)
                {
                    cpuPlayer._invalidDirections.Clear();                           // must clear all invalid directions as the opposite direction was invalid in the previousShot
                    int[] firstValidHit = GetTargetShip(cpuPlayer, opponentPlayer);
                    cpuPlayer._validDirection = GoInOppositeDirection((DirectionList)cpuPlayer._validDirection);
                    cpuPlayer._switchDirection++;

                    shotMessage = ShootAtDirection(cpuPlayer, opponentPlayer, firstValidHit[0], firstValidHit[1], (int)cpuPlayer._validDirection, rand);
                }
                // previous shot missed and we "don't" have a valid direction, but there is still a hit ship. Shoot in random directions
                //  Note: _validDirection is never made null after the first assignment (For practical use in the above else if)
                else
                {
                    int[] firstValidHit = GetTargetShip(cpuPlayer, opponentPlayer);
                    int randomDirection = rand.Next(cpuPlayer._directionListMin, cpuPlayer._directionListMax);

                    shotMessage = ShootAtDirection(cpuPlayer, opponentPlayer, firstValidHit[0], firstValidHit[1], randomDirection, rand);
                }
            }
            else     // there is no ship that has been hit
            {
                if (previousShot != null)
                {
                    if (targetGrid[previousShot[0], previousShot[1]] == opponentPlayer.targetSunkDisplay)    // if the previousShot sunk a ship
                    {
                        // Reset all variables
                        cpuPlayer._validDirection = null;
                        cpuPlayer._invalidDirections.Clear();
                        cpuPlayer._knownHitLocations.Clear();
                        cpuPlayer._switchDirection = 0;
                        cpuPlayer._targetShip = 0;
                    }
                }

                bool isValidCoordinates = false;
                while (!isValidCoordinates)
                {
                    int y = rand.Next(0, targetGrid.GetLength(0));
                    int x = rand.Next(0, targetGrid.GetLength(1));

                    if (targetGrid[y, x] != '~')                            // If it's already hit that spot
                    {
                        continue;
                    }
                    else if (!IsSurroundingDirectionsInvalid(cpuPlayer, y, x))        // If every spot surrounding this shot has already been hit
                    {
                        cpuPlayer._invalidDirections.Clear();
                        continue;
                    }
                    else
                    {
                        shotMessage = TargetGrid.PlaceShotsOnTargetGrid(cpuPlayer, opponentPlayer, y, x);
                        isValidCoordinates = true;
                    }
                }
            }
            return shotMessage;
        }

        /// <summary>
        /// Places a shot at direction (may not end up being chosenDirection)
        /// </summary>
        /// <param name="cpuPlayer">cpu player doing the shooting</param>
        /// <param name="opponentPlayer">player being shot at</param>
        /// <param name="validHit_y">the valid coordinate y, (usually either peviousShot or from _knownHitList)</param>
        /// <param name="validHit_x">the valid coordinate x, (usually either peviousShot or from _knownHitList)</param>
        /// <param name="chosenDirection">the chosenDirection the shot will be attemoted at</param>
        /// <param name="rand">random variable</param>
        /// <returns>the Modified target grid of cpu, modified ocean grid of opponent, and the shot Message</returns>
        static string ShootAtDirection(CPUPlayer cpuPlayer, BasePlayer opponentPlayer, int validHit_y, int validHit_x, int chosenDirection, Random rand)
        {
            cpuPlayer.previousShot = CheckShotInDirection(cpuPlayer, opponentPlayer, [validHit_y, validHit_x], chosenDirection, rand);    // add to previousShot based on random direction
            string message = TargetGrid.PlaceShotsOnTargetGrid(cpuPlayer, opponentPlayer, cpuPlayer.previousShot[0], cpuPlayer.previousShot[1]);   // Shoot in direction

            if (cpuPlayer.targetGrid[cpuPlayer.previousShot[0], cpuPlayer.previousShot[1]] != cpuPlayer.targetHitDisplay)    // if the direction didn't hit a ship
            {
                cpuPlayer._invalidDirections.Add((DirectionList)cpuPlayer._validDirection); // Will never be null due to function always being entered with a validDirection
            }

            return message;
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
        static int[] CheckShotInDirection(CPUPlayer cpuPlayer, BasePlayer opponentPlayer, int[] lastValidHit, int chosenDirection, Random rand)
        {
            int y_axis = 0;
            int x_axis = 1;

            int[]? shootAtCoordinates = null;
            DirectionList direction = (DirectionList)chosenDirection;

            while (shootAtCoordinates == null)
            {
                Dictionary<DirectionList, int[]> DirectionCoordinatesDict = GetCoordinatesAtDirection(lastValidHit[y_axis], lastValidHit[x_axis], cpuPlayer);

                // If _invalidDirections doesn't have chosenDirection
                if (!cpuPlayer._invalidDirections.Contains(direction) || cpuPlayer._invalidDirections.Count == 0)
                {
                    shootAtCoordinates = DirectionCoordinatesDict[direction];
                    cpuPlayer._validDirection = direction;
                }
                // if we have a valid direction, but that direction is leading to out of bounds, but that direction has led to hits. 
                // when the direction would lead to out of bounds
                else if (cpuPlayer._invalidDirections.Contains(direction) && cpuPlayer._knownHitLocations.Count > 1 && cpuPlayer._switchDirection < 1)
                {
                    // go in the opposite direction
                    direction = GoInOppositeDirection(direction);
                    cpuPlayer._switchDirection++;
                }
                // we've switched directions and now have to verify which ship will lead to a valid direction
                else if (cpuPlayer._switchDirection == 1)
                {
                    cpuPlayer._invalidDirections.Clear();
                    lastValidHit = cpuPlayer._knownHitLocations[0];
                    DirectionCoordinatesDict = GetCoordinatesAtDirection(lastValidHit[y_axis], lastValidHit[x_axis], cpuPlayer);

                    cpuPlayer._targetShip = 0;          // the ship being targeted is the first hit ship, number does not matter as long as its consistently referenced

                    if (cpuPlayer._invalidDirections.Contains(direction))                   // if oppositeDirection is invalid at original ship coordinates
                    {
                        cpuPlayer._invalidDirections.Clear();
                        lastValidHit = GetFirstHitOfLastShip(cpuPlayer, opponentPlayer);    // then go in oppositeDirection at last hit ship coordinates
                        DirectionCoordinatesDict = GetCoordinatesAtDirection(lastValidHit[y_axis], lastValidHit[x_axis], cpuPlayer);
                        cpuPlayer._targetShip = 1;      // the ship being targeted is that last hit ship
                    }

                    cpuPlayer._switchDirection++;
                }
                // current direction is invalid, but we've already switched directions, so we need to go in an perpendicular direction
                // when direction leads to out of bounds
                else if (cpuPlayer._invalidDirections.Contains(direction) && cpuPlayer._switchDirection == 2)
                {
                    lastValidHit = GetTargetShip(cpuPlayer, opponentPlayer);                            // lastValidHit is acutally first valid hit
                    GetRandomValidDirection(cpuPlayer, rand);
                    cpuPlayer._switchDirection++;
                }
                // we've switched directions, gone in a perpendicular direction, and now that perpendicular direction leads to out of bounds
                // so we go in the oppsite direction of perpendicular direction
                else if (cpuPlayer._invalidDirections.Contains(direction) && cpuPlayer._switchDirection == 3)
                {
                    cpuPlayer._invalidDirections.Clear();                                               // Must clear as opposite direction was invalid
                    lastValidHit = GetTargetShip(cpuPlayer, opponentPlayer);                            // lastValidHit is actually the first valid hit
                    direction = GoInOppositeDirection(direction);
                    cpuPlayer._switchDirection++;
                }
                else // choose a random direction
                {
                    chosenDirection = rand.Next(cpuPlayer._directionListMin, cpuPlayer._directionListMax);
                    direction = (DirectionList)chosenDirection;
                }
            }

            return shootAtCoordinates;
        }

        /// <summary>
        /// gets the coordinates in each direction as a dictionary. As well as adjust invalidDirections list if they result in invalid directions
        /// </summary>
        /// <param name="userY">the inputed y coordinate</param>
        /// <param name="userX">the inputed x coordinate</param>
        /// <param name="cpuPlayer">the cpu player being modified</param>
        /// <returns>returns a dictionary of each coordinate modified by direction</returns>
        static Dictionary<DirectionList, int[]> GetCoordinatesAtDirection(int userY, int userX, CPUPlayer cpuPlayer)
        {
            Dictionary<DirectionList, int[]> directionList = new Dictionary<DirectionList, int[]>();

            foreach (DirectionList direction in Enum.GetValues(typeof(DirectionList)))
            {
                int y = userY;
                int x = userX;
                switch (direction)
                {
                    case DirectionList.Up:
                        directionList.Add(direction, [--y, x]);
                        break;
                    case DirectionList.Down:
                        directionList.Add(direction, [++y, x]);
                        break;
                    case DirectionList.Left:
                        directionList.Add(direction, [y, --x]);
                        break;
                    case DirectionList.Right:
                        directionList.Add(direction, [y, ++x]);
                        break;
                }
            }

            // Modifies invalidDirections with each direction that leads to invalid coordinates
            foreach (DirectionList direction in directionList.Keys)
            {
                char[,] targetGrid = cpuPlayer.targetGrid;
                int y_axis = directionList[direction][0];
                int x_axis = directionList[direction][1];

                bool isYOutOfBounds = y_axis > targetGrid.GetUpperBound(0) || y_axis < targetGrid.GetLowerBound(0);
                bool isXOutOfBounds = x_axis > targetGrid.GetUpperBound(1) || x_axis < targetGrid.GetLowerBound(1);

                if (!cpuPlayer._invalidDirections.Contains(direction))
                {
                    if (isYOutOfBounds || isXOutOfBounds)               // if that location is out of bounds
                    {
                        cpuPlayer._invalidDirections.Add(direction);
                    }
                    else if (targetGrid[y_axis, x_axis] != '~')         // if they've already hit that location
                    {
                        cpuPlayer._invalidDirections.Add(direction);
                    }
                }
            }

            return directionList;
        }

        /// <summary>
        /// Gets a random valid direction.
        /// Mostly in use to get a perpendicular direction from two invalid directions
        /// </summary>
        /// <param name="cpuPlayer"> cpuPlayer being modified</param>
        /// <param name="rand">random variable</param>
        static void GetRandomValidDirection(CPUPlayer cpuPlayer, Random rand)
        {
            List<DirectionList> validDirectionList = new List<DirectionList>();
            foreach (DirectionList direction in Enum.GetValues(typeof(DirectionList)))
            {
                if (direction == DirectionList.Invalid)
                {
                    continue;
                }
                else if (cpuPlayer._invalidDirections.Contains(direction))
                {
                    continue;
                }
                else
                {
                    validDirectionList.Add(direction);
                }
            }

            int index = rand.Next(validDirectionList.Count);        // choose a random valid direction

            cpuPlayer._validDirection = validDirectionList[index];
        }

        /// <summary>
        /// Runs through the list of opponent ships and checks if there is a hit ship
        /// </summary>
        /// <param name="opponentPlayer">opponent player being checked</param>
        /// <returns>True if there is a hit ship, false if there is no hit ship</returns>
        static bool IsOpponoentShipHit(BasePlayer opponentPlayer)
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

        /// <summary>
        /// Finds the opposite direction of the inputed direction
        /// </summary>
        /// <param name="direction">inputed direction</param>
        /// <returns>the opposite direction</returns>
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
                if (ship.DisplayWhenHit == oceanGrid[lastHitLocation[0], lastHitLocation[1]])     // return the Battleship that was last hit
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
                else                                                                            // until it hits a new ship, there are cases where a hitShip coordinates are not placed all together
                {
                    continue;
                }
            }

            return firstHitLocationOfLastShip;
        }

        /// <summary>
        /// Gets the current targeted ship
        /// </summary>
        /// <param name="cpuPlayer">cpuPlayer being checked for targeted ship</param>
        /// <param name="opponentPlayer">opponent player being checked for their last hit ship</param>
        /// <returns>valid coordinates of a targeted hit ship</returns>
        static int[] GetTargetShip(CPUPlayer cpuPlayer, BasePlayer opponentPlayer)
        {
            int[] validHit = [];            // get the first hit of the targetted ship, defaults is the first known ship
            int targetShip = cpuPlayer._targetShip;

            if (targetShip == 0)
            {
                validHit = cpuPlayer._knownHitLocations[0];
            }
            else if (targetShip == 1)
            {
                validHit = GetFirstHitOfLastShip(cpuPlayer, opponentPlayer);
            }

            return validHit;
        }

        /// <summary>
        /// Creates a list of modified [y,x] coordinates based on direction and checks if each results in a valid direction (unshot coordinate)
        /// if at least one direction is valid, then will return true
        /// </summary>
        /// <param name="cpuPlayer">the cpu player having it's target grid checked</param>
        /// <param name="y_shot">the potential y coordinate being modified</param>
        /// <param name="x_shot">the potential x coordinates being modified</param>
        /// <returns>return true if at aleast one direction results in an unshot targetGrid char
        /// Returns false if all directions have already been hit</returns>
        static bool IsSurroundingDirectionsInvalid(CPUPlayer cpuPlayer, int y_shot, int x_shot)
        {
            Dictionary<DirectionList, int[]> directionDict = GetCoordinatesAtDirection(y_shot, x_shot, cpuPlayer);

            int isLogicalCoordinates = 0;
            foreach (DirectionList direction in directionDict.Keys)
            {
                if (!cpuPlayer._invalidDirections.Contains(direction))
                {
                    isLogicalCoordinates++;
                }
            }
            return isLogicalCoordinates > 0;
        }
    }
}
