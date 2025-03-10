namespace BattleShip_PSchmitt
{
    class CPU : Player
    {
        enum Directions
        {
            Up = 0,
            Down, 
            Left, 
            Right
        }
        Directions? _validDirection;
        List<Directions> _invalidDirection = [];
        List<int[]> _knownHitLocations = [];
        int _switchDirection = 0;
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
            string[] directionList = { "up", "down", "left", "right" };

            for (int index = 0; index < playerShipList.Count; index++)
            {
                Battleship chosenShip = playerShipList[index];    // chooses a ship
                do
                {
                    int y = rand.Next(0, oceanGrid.GetLength(0));
                    int x = rand.Next(0, oceanGrid.GetLength(1));
                    int directionChoice = rand.Next(0, 4);
                    cpuPlayer.oceanGrid = OceanGrid.PlaceShipOnOceanGrid(cpuPlayer, chosenShip, directionList[directionChoice], [y, x], ref isValidCoordinates);

                } while (!isValidCoordinates);
            }
            return cpuPlayer.oceanGrid;
        }

        /// <summary>
        /// Randomaly shoots somewhere on the target grid.
        /// </summary>
        /// <param name="cpuPlayer">The computer player.</param>
        /// <param name="opponentPlayer">The human player.</param>
        /// <param name="rand">Random variable</param>
        /// <returns>The "shoot" message that will be displayed, whether the cpu successfully shot a player ship.</returns>
        public static string ChooseRandomShot(CPU cpuPlayer, Player opponentPlayer, Random rand)
        {
            char[,] targetGrid = cpuPlayer.targetGrid;
            char[,] opponentOceanGrid = opponentPlayer.oceanGrid;
            int[]? previousShot = cpuPlayer.previousShot;
            
            bool isValidCoordinates = false;
            string message = "";

            if (IsAShipHitButNotSunk(opponentPlayer))           // if a ship was hit and it hasn't been sunk, we know that previousShot cannot be null
            {
                // if the previous shot hit
                if (GameGrid.IsCharShipDisplayWhenHit(opponentOceanGrid[previousShot[0], previousShot[1]], opponentPlayer.shipList))
                {
                    cpuPlayer._knownHitLocations.Add([previousShot[0], previousShot[1]]);                               // Add to the list of known hit locations
                }
                // if there is a sunk ship and a hit ship at the same time
                if (GameGrid.IsCharShipDisplayWhenSunk(opponentOceanGrid[previousShot[0], previousShot[1]], opponentPlayer.shipList))
                {
                    cpuPlayer = RemoveSunkShipCoordinates(cpuPlayer, opponentPlayer);
                    cpuPlayer._switchDirection = 0;

                }
                // if the previous shot was a hit, but we don't have a valid direction
                if (GameGrid.IsCharShipDisplayWhenHit(opponentOceanGrid[previousShot[0], previousShot[1]], opponentPlayer.shipList) && cpuPlayer._validDirection == null)
                {
                    cpuPlayer.previousShot = PlaceShotInRandomDirection(cpuPlayer, [previousShot[0], previousShot[1]], opponentOceanGrid, rand);    // add to previousShot based on random direction
                    message = TargetGrid.PlaceShotsOnTargetGrid(cpuPlayer, opponentPlayer, cpuPlayer.previousShot[0], cpuPlayer.previousShot[1]);   // Shoot in direction

                    if (!GameGrid.IsCharShipDisplayWhenHit(opponentOceanGrid[cpuPlayer.previousShot[0], cpuPlayer.previousShot[1]], opponentPlayer.shipList))    // it didn't hit a ship
                    {
                        cpuPlayer._invalidDirection.Add((Directions)cpuPlayer._validDirection);
                    }
                }
                // if the previous shot was a hit, and we have a valid direction
                else if (GameGrid.IsCharShipDisplayWhenHit(opponentOceanGrid[previousShot[0], previousShot[1]], opponentPlayer.shipList) && cpuPlayer._validDirection != null)
                {
                    cpuPlayer.previousShot = PlaceShotInValidDirection((int)cpuPlayer._validDirection, previousShot[0], previousShot[1]);
                    message = TargetGrid.PlaceShotsOnTargetGrid(cpuPlayer, opponentPlayer, cpuPlayer.previousShot[0], cpuPlayer.previousShot[1]);   // Shoot in direction

                    if (!GameGrid.IsCharShipDisplayWhenHit(opponentOceanGrid[cpuPlayer.previousShot[0], cpuPlayer.previousShot[1]], opponentPlayer.shipList))     // if the direction didn't hit a ship
                    {
                        cpuPlayer._invalidDirection.Add((Directions)cpuPlayer._validDirection);
                    }
                }
                // if the previous shot missed, and we have a valid direction, and that valid direction has led to hits
                else if (!GameGrid.IsCharShipDisplayWhenHit(opponentOceanGrid[previousShot[0], previousShot[1]], opponentPlayer.shipList) && 
                         cpuPlayer._validDirection != null && cpuPlayer._knownHitLocations.Count > 1 && cpuPlayer._switchDirection == 0)
                {
                    switch((Directions)cpuPlayer._validDirection)           // Go in the opposite direction
                    {
                        case Directions.Up:
                            cpuPlayer._validDirection = Directions.Down;
                            break;
                        case Directions.Down:
                            cpuPlayer._validDirection = Directions.Up;
                            break;
                        case Directions.Left:
                            cpuPlayer._validDirection = Directions.Right;
                            break;
                        case Directions.Right:
                            cpuPlayer._validDirection = Directions.Left;
                            break;
                    }

                    cpuPlayer._switchDirection++;
                    int[] firstValidHit = cpuPlayer._knownHitLocations[0];  // Shoot at the first known hit coordinates
                    cpuPlayer.previousShot = PlaceShotInValidDirection((int)cpuPlayer._validDirection, firstValidHit[0], firstValidHit[1]);
                    message = TargetGrid.PlaceShotsOnTargetGrid(cpuPlayer, opponentPlayer, cpuPlayer.previousShot[0], cpuPlayer.previousShot[1]);   // Shoot in direction
                }
                else // previous shot missed an we "don't" have a valid direction, but there is still a hit ship
                {
                    int[] firstValidHit = cpuPlayer._knownHitLocations[0];  // Shoot at the first known hit coordinates
                    cpuPlayer.previousShot = PlaceShotInRandomDirection(cpuPlayer, [firstValidHit[0], firstValidHit[1]], opponentOceanGrid, rand);    // add to previousShot based on random direction
                    message = TargetGrid.PlaceShotsOnTargetGrid(cpuPlayer, opponentPlayer, cpuPlayer.previousShot[0], cpuPlayer.previousShot[1]);   // Shoot in direction

                    if (!GameGrid.IsCharShipDisplayWhenHit(opponentOceanGrid[cpuPlayer.previousShot[0], cpuPlayer.previousShot[1]], opponentPlayer.shipList))     // if the direction didn't hit a ship
                    {
                        cpuPlayer._invalidDirection.Add((Directions)cpuPlayer._validDirection);
                    }
                }

            }
            else            // there is no ship that has been hit and not sunk
            {
                if (previousShot != null)
                {
                    if (targetGrid[previousShot[0], previousShot[1]] == 'N')
                    {
                        cpuPlayer._validDirection = null;
                        cpuPlayer._invalidDirection.Clear();
                        cpuPlayer._knownHitLocations.Clear();
                        cpuPlayer._switchDirection = 0;
                    }
                }
                // reset _invalidDirections, validDirection, and knownHitLocations
                while (!isValidCoordinates)
                {
                    int y = rand.Next(0, targetGrid.GetLength(0));
                    int x = rand.Next(0, targetGrid.GetLength(1));

                    if (targetGrid[y, x] == 'M' || targetGrid[y, x] == 'H' || targetGrid[y, x] == 'N')      // If it's already hit that spot
                    {
                        continue;
                    }
                    else
                    {
                        message = TargetGrid.PlaceShotsOnTargetGrid(cpuPlayer, opponentPlayer, y, x);
                        cpuPlayer.previousShot = [y, x];
                        isValidCoordinates = true;
                    }
                }
            }
            return message;
        }

        static bool IsAShipHitButNotSunk(Player opponent)
        {
            List<Battleship> opponentShips = opponent.shipList;
            bool isShipHitNotSunk = false;
            foreach(Battleship ship in opponentShips)
            {
                if (ship.IsHit)
                {
                    isShipHitNotSunk = true;
                }
            }

            return isShipHitNotSunk;
        }
        static CPU RemoveSunkShipCoordinates(CPU cpuPlayer, Player opponent)
        {
            List<Battleship> opponentShips = opponent.shipList;

            foreach (Battleship ship in opponentShips)
            {
                if (!ship.IsStillFloating)
                {
                    for (int i = 0; i < ship.EachIndexOnOceanGrid.Count ; i++)
                    {
                        int[] something = ship.EachIndexOnOceanGrid[i];
                        cpuPlayer._knownHitLocations.Remove(something);
                    }
                }
            }

            return cpuPlayer;
        }

        static int[] PlaceShotInRandomDirection(CPU cpu, int[] previousShot, char[,] opponentGrid, Random rand)
        {
            // choose a random int for direction
            int randomDirection = rand.Next(0, 4);
            
            bool isValidDirection = false;
            int[] checkCoordinates = [];

            do
            {
                int y = previousShot[0];
                int x = previousShot[1];
                // if direction is invalid
                if (cpu._invalidDirection.Contains((Directions)randomDirection))
                {
                    // choose a new direction that isn't in _invalidDirections
                    randomDirection = rand.Next(0, 4);
                }
                else // the Direction wasn't on the invalid List
                {
                    checkCoordinates = PlaceShotInValidDirection(randomDirection, y, x);
                    if (checkCoordinates[0] > 9 || checkCoordinates[0] < 0)         // if they're out of bounds
                    {
                        cpu._invalidDirection.Add((Directions)randomDirection);
                    }
                    else if (checkCoordinates[1] > 9 || checkCoordinates[1] < 0)    // if they're out of bounds
                    {
                        cpu._invalidDirection.Add((Directions)randomDirection);
                    }
                    else if (cpu.targetGrid[checkCoordinates[0], checkCoordinates[1]] == 'H' || cpu.targetGrid[checkCoordinates[0], checkCoordinates[1]] == 'M') // if it's already hit that spot
                    {
                        cpu._invalidDirection.Add((Directions)randomDirection);
                    }
                    else
                    {
                        cpu._validDirection = (Directions)randomDirection;
                        isValidDirection = true;
                    }
                }

            } while (!isValidDirection);

            return checkCoordinates;
        }

        static int[] PlaceShotInValidDirection(int direction, int y, int x)
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
            }

            return newCoordinates;
        }   
    }
}
