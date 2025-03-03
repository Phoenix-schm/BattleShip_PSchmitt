namespace BattleShip_PSchmitt
{
    class CPU : Player
    {
        public static char[,] CreateCPUoceanGrid(Player player, Random rand)
        {
            char[,] oceanGrid = player._playerOceanGrid;
            int[] eachShip = Player.RandomNumberList(5, 5, rand);
            bool isValid = false;

            for (int index = 0; index < eachShip.Length; index++)
            {
                int x = -1;
                int y = -1;
                Battleship chosenShip = player._playerShipList[eachShip[index]];    // chooses a random ship
                int direction = -1;

                do
                {
                    y = rand.Next(0, oceanGrid.GetLength(0));
                    x = rand.Next(0, oceanGrid.GetLength(1));
                    direction = rand.Next(0, 4);
                    if (direction == 0 || direction == 1)
                    {
                        oceanGrid = player.Vetical_PlaceShipsOnOceanGrid(oceanGrid, chosenShip, direction, [x, y], ref isValid);
                    }
                    else if (direction == 2 || direction == 3)
                    {
                        oceanGrid = player.Horizontal_PlaceShipsOnOceanGrid(oceanGrid, chosenShip, direction, [y, x], ref isValid);
                    }

                } while (!isValid);
            }
            return oceanGrid;
        }

        static Battleship ChooseShip(List<Battleship> shipList, Random rand)
        {
            bool isValid = false;
            int choice = -1;
            Battleship chosenShip = null;
            int allShipsUsed = 0;
            while (!isValid)
            {
                choice = rand.Next(0, shipList.Count);              // choose a random ship

                if (shipList[choice].eachIndexSpace.Count > 0)      // if that ship has already been chosen
                {
                    continue;
                }
                else
                {
                    chosenShip = shipList[choice];
                    isValid = true;
                }
            }
            return chosenShip;
        }

    }
}
