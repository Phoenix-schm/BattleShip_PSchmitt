namespace BattleShip_PSchmitt
{
    class CPU : Player
    {
        public static char[,] CreateCPUoceanGrid(Player player, Random rand)
        {
            char[,] oceanGrid = player.playerOceanGrid;
            List<Battleship> playerShipList = player.playerShipList;
            bool isValid = false;

            for (int index = 0; index < playerShipList.Count; index++)
            {
                Battleship chosenShip = playerShipList[index];    // chooses a ship
                do
                {
                    int y = rand.Next(0, oceanGrid.GetLength(0));
                    int x = rand.Next(0, oceanGrid.GetLength(1));
                    int direction = rand.Next(0, 4);
                    oceanGrid = OceanGrid.PlaceShipsOnOceanGrid(oceanGrid, chosenShip, direction, [y, x], ref isValid);

                } while (!isValid);
            }
            return oceanGrid;
        }

        public static void ChooseRandomShot(Player cpu, Player player, Random rand)
        {
            char[,] targetGrid = cpu.playerTargetGrid;
            bool isValid = false;

            while (!isValid)
            {
                int y = rand.Next(0, targetGrid.GetLength(0));
                int x = rand.Next(0, targetGrid.GetLength(1));

                if (targetGrid[y, x] == 'M' || targetGrid[y,x] == 'H')
                {
                    continue;
                }
                else if (targetGrid[y,x] == '~')
                {
                    TargetGrid.PlaceShotsOnTargetGrid(cpu, player, y, x);
                    isValid = true;
                }
            }
        }
    }
}
