namespace BattleShip_PSchmitt
{
    class CPU : Player
    {
        int _validDirection;
        List<int> _invalidDirection = [];
        List<int[]> _knownHitLocations = [];
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
                    OceanGrid.PlaceShipOnOceanGrid(cpuPlayer, chosenShip, directionList[directionChoice], [y, x], ref isValidCoordinates);

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

            bool isValid = false;
            string message = "";

            if (IsAShipHitButNotSunk(opponentPlayer))           // if a ship was hit and it hasn't been sunk
            {
                if (opponentOceanGrid[previousShot[0], previousShot[1]] == 'H')
                {
                    // add to the _knownHitLocations[]
                    // choose a direction
                    // shoot at direction
                    // if direction gets an 'H', then save as valid direction.
                    // keep shooting in that direction until its no longer valid (won't occur if the ship ahas been sunk)
                    // if direction gets an 'M',
                    //      then it isn't valid.
                    //      add to _invalidDirections
                    //      Next else statement will occur
                }
                else if ()// previous shot missed, but we know the previous valid direction
                {
                    // if valid direction was up, go down. Vice versa 
                    //      new valid direction
                    //      potential* for loop to the first ship.HitDisplay
                    // if valid direction was right, go left
                    //      new valid direction
                    //      potential* for loop to the first ship.HitDisplay
                }
                else // previous shot missed an we don't have a valid direction
                {
                    // choose a random direction that isn't in the _invalidDirections
                    // shoot in that direction

                    // if it leads to a ship.HitDisplay
                    // _validDirection
                    // else it wasn't a hit
                    // add to _invalidDirections
                }

            }
            else            // there is no ship that has been hit and not sunk
            {
                // reset _invalidDirections, validDirection, and knownHitLocations
                while (!isValid)
                {
                    int y = rand.Next(0, targetGrid.GetLength(0));
                    int x = rand.Next(0, targetGrid.GetLength(1));

                    if (targetGrid[y, x] == 'M' || targetGrid[y, x] == 'H')      // If it's already hit that spot
                    {
                        continue;
                    }
                    else
                    {
                        message = TargetGrid.PlaceShotsOnTargetGrid(cpuPlayer, opponentPlayer, y, x);
                        cpuPlayer.previousShot = [y, x];
                        isValid = true;
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

        static int ReturnADirection(int[] previousShot, char[,] opponentGrid)
        {
            // choose a random int for direction
            bool isValidDirection = false;
            int y = previousShot[0];
            int x = previousShot[1];

            int[] checkCoordinates;

            // do
                // if direction is invalid
                    // choose a new direction that isn't in _invalidDirections
                // else if _invalidDirections doesn't contain that direction then proceed
                    // switchcase direction

                    //  up
                    //      checkCoordinates = [y++, x]
                    //  down
                    //      checkCoordinates = [y--, x]
                    //  left
                    //      checkCoordinates = [y, x++]
                    //  right
                    //      checkCoordinates = [y, x--]
                //
                // if (checkCoordinates[0] > 9 || checkCoordinates [0] < 0)
                    // isn't valid direction
                    // add to invalidDirections
                // if (checkCoordinates[1] > 9 || checkCoordinates[1] < 0)
                    // isn'tvalid direciton
                    // add to invalidDirections
            // while (!isValidDirection)

            // return a valid direction
        }
            
    }
}
