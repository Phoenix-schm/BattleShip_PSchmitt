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
        public List<int[]> sunkShipPositions = [];
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
            char charAtIndex;

            if (IsAShipHitButNotSunk(opponentPlayer) && cpuPlayer.previousShot != null)           // if a ship was hit and it hasn't been sunk, we know that previousShot cannot be null
            {
                charAtIndex = opponentOceanGrid[previousShot[0], previousShot[1]];
                
                if (GameGrid.IsCharShipDisplayWhenHit(charAtIndex, opponentPlayer))             // if previous shot was a hit
                {
                    cpuPlayer._knownHitLocations.Add([previousShot[0], previousShot[1]]);       // Add to the list of known hit locations
                }

                if (charAtIndex == cpuPlayer.targetSunkDisplay)                                 // if there is a sunk ship and a hit ship at the same time
                {
                    cpuPlayer._knownHitLocations = RemoveSunkShipCoordinates(cpuPlayer, opponentPlayer);
                    cpuPlayer._switchDirection = 0;
                    cpuPlayer._invalidDirection.Clear();
                }
                // if the previous shot was a hit, but we don't have a valid direction
                if (GameGrid.IsCharShipDisplayWhenHit(charAtIndex, opponentPlayer) && cpuPlayer._validDirection == null)
                {
                    int randomDirection = rand.Next(0, 4);
                    message = ShootAtDirection(cpuPlayer, opponentPlayer, previousShot[0], previousShot[1], randomDirection, rand);
                }
                // if the previous shot was a hit, and we have a valid direction
                else if (GameGrid.IsCharShipDisplayWhenHit(charAtIndex, opponentPlayer) && cpuPlayer._validDirection != null)
                {
                    message = ShootAtDirection(cpuPlayer, opponentPlayer, previousShot[0], previousShot[1], (int)cpuPlayer._validDirection, rand);
                }
                // if the previous shot missed, and we have a valid direction, and that valid direction has led to hits. and we haven't switch directions yet
                else if (!GameGrid.IsCharShipDisplayWhenHit(charAtIndex, opponentPlayer) && 
                         cpuPlayer._validDirection != null && cpuPlayer._knownHitLocations.Count > 1 && cpuPlayer._switchDirection <= 1)
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
                    message = ShootAtDirection(cpuPlayer, opponentPlayer, firstValidHit[0], firstValidHit[1], (int)cpuPlayer._validDirection, rand);
                }
                else // previous shot missed and we "don't" have a valid direction, but there is still a hit ship
                {
                    int[] firstValidHit = cpuPlayer._knownHitLocations[0];  // Shoot at the first known hit coordinates
                    int randomDirection = rand.Next(0, 4);

                    message = ShootAtDirection(cpuPlayer, opponentPlayer, firstValidHit[0], firstValidHit[1], randomDirection, rand);
                }

            }
            else            // there is no ship that has been hit
            {
                if (previousShot != null)
                {
                    if (targetGrid[previousShot[0], previousShot[1]] == opponentPlayer.targetSunkDisplay)    // if the previousShot sunk a ship
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
                    char charOnGrid = targetGrid[y, x];

                    if (charOnGrid == cpuPlayer.targetMissDisplay || charOnGrid == cpuPlayer.targetHitDisplay || charOnGrid == cpuPlayer.targetSunkDisplay)      // If it's already hit that spot
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
        static List<int[]> RemoveSunkShipCoordinates(CPU cpuPlayer, Player opponent)
        {
            List<Battleship> opponentShips = opponent.shipList;
            char[,] opponentOceanGrid = opponent.oceanGrid;
            List<int[]> hitLocations = cpuPlayer._knownHitLocations;

            foreach (char position in opponentOceanGrid)
            {
                for (int i = 0; i < hitLocations.Count; i++)
                {
                    int[] index = hitLocations[i];
                    if (opponentOceanGrid[index[0], index[1]] == opponent.targetSunkDisplay)
                    {
                        cpuPlayer._knownHitLocations.RemoveAt(i);
                    }
                }
            }
            return cpuPlayer._knownHitLocations;
        }

        static int[] PlaceShotInDirection(CPU cpu, int[] previousShot, char[,] opponentGrid, int chosenDirection, Random rand)
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
                    chosenDirection = rand.Next(0, 4);
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
            }

            return newCoordinates;
        }

        static string ShootAtDirection(CPU cpu, Player opponentPlayer, int validHit_y, int validHit_x, int chosenDirection, Random rand)
        {
            cpu.previousShot = PlaceShotInDirection(cpu, [validHit_y, validHit_x], opponentPlayer.oceanGrid, chosenDirection, rand);    // add to previousShot based on random direction
            string message = TargetGrid.PlaceShotsOnTargetGrid(cpu, opponentPlayer, cpu.previousShot[0], cpu.previousShot[1]);   // Shoot in direction

            if (!GameGrid.IsCharShipDisplayWhenHit(opponentPlayer.oceanGrid[cpu.previousShot[0], cpu.previousShot[1]], opponentPlayer))    // if the direction didn't hit a ship
            {
                cpu._invalidDirection.Add((Directions)cpu._validDirection);
            }

            return message;
        }
    }
}
