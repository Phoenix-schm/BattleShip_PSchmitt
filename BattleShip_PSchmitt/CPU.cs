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
                int x;
                int y;
                Battleship chosenShip = playerShipList[index];    // chooses a ship
                int direction;
                do
                {
                    y = rand.Next(0, oceanGrid.GetLength(0));
                    x = rand.Next(0, oceanGrid.GetLength(1));
                    direction = rand.Next(0, 4);
                    oceanGrid = OceanGrid.PlaceShipsOnOceanGrid(oceanGrid, chosenShip, direction, [y, x], ref isValid);

                } while (!isValid);
            }
            return oceanGrid;
        }
    }
}
