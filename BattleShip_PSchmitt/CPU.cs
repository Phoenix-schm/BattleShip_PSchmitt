namespace BattleShip_PSchmitt
{
    class CPU : Player
    {
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
                    cpuPlayer.oceanGrid = OceanGrid.PlaceShipOnOceanGrid(cpuPlayer.oceanGrid, chosenShip, directionList[directionChoice], [y, x], ref isValidCoordinates);

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
            bool isValid = false;
            string message = "";

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
            return message;
        }
    }
}
