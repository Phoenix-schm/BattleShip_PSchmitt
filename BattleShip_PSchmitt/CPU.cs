namespace BattleShip_PSchmitt
{
    class CPU : Player
    {
        /// <summary>
        /// Creates an Ocean grid with random ship placements
        /// </summary>
        /// <param name="autoPlayer">The player having their grid created</param>
        /// <param name="rand">Random generator</param>
        /// <returns>The new ocean grid</returns>
        public static char[,] CreateCPUoceanGrid(Player autoPlayer, Random rand)
        {
            char[,] oceanGrid = autoPlayer.oceanGrid;
            List<Battleship> playerShipList = autoPlayer.shipList;
            bool isValidCoordinates = false;

            for (int index = 0; index < playerShipList.Count; index++)
            {
                Battleship chosenShip = playerShipList[index];    // chooses a ship
                do
                {
                    int y = rand.Next(0, oceanGrid.GetLength(0));
                    int x = rand.Next(0, oceanGrid.GetLength(1));
                    int direction = rand.Next(0, 4);
                    autoPlayer.oceanGrid = OceanGrid.PlaceShipsOnOceanGrid(autoPlayer.oceanGrid, chosenShip, direction, [y, x], ref isValidCoordinates);

                } while (!isValidCoordinates);
            }
            return autoPlayer.oceanGrid;
        }

        /// <summary>
        /// Randomaly shoots somewhere on the target grid.
        /// </summary>
        /// <param name="cpu">The computer player.</param>
        /// <param name="player">The human player.</param>
        /// <param name="rand">Random variable</param>
        /// <returns>The "shoot" message that will be displayed, whether the cpu successfully shot a player ship.</returns>
        public static string ChooseRandomShot(CPU cpu, Player player, Random rand)
        {
            char[,] targetGrid = cpu.targetGrid;
            char[,] opponentOceanGrid = player.oceanGrid;
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
                    message = TargetGrid.PlaceShotsOnTargetGrid(cpu, player, y, x);
                    cpu.previousShot = [y, x];
                    isValid = true;
                }
            }
            return message;
        }

        //static Battleship? isShipHit()
    }
}
