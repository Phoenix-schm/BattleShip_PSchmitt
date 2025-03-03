namespace BattleShip_PSchmitt
{
    class CPU : Player
    {
        public static char[,] CreateCPUOceanGrid(Player player, Random rand)
        {
            char[,] oceanGrid = new char[10, 10];
            int shipCount = player._playerShipList.Count;
            bool isValid = false;

            for (int index = 0; index < shipCount; index++)
            {
                int x = -1;
                int y = -1;
                Battleship chosenShip = null;
                int direction = -1;

                while (!isValid)
                {
                    y = rand.Next(0, oceanGrid.GetLength(0));
                    x = rand.Next(0, oceanGrid.GetLength(1));
                    direction = rand.Next(0, 4);
                    chosenShip = ChooseShip(player._playerShipList, rand);
                    if (direction == 0 || direction == 1)
                    {
                        oceanGrid = player.Vetical_PlaceShipsOnOceanGrid(oceanGrid, chosenShip, direction, [x, y], ref isValid);
                    }
                    else if (direction == 2 || direction == 3)
                    {
                        oceanGrid = player.Horizontal_PlaceShipsOnOceanGrid(oceanGrid, chosenShip, direction, [x, y], ref isValid);
                    }
                }
            }
            return oceanGrid;
        }

        static Battleship ChooseShip(List<Battleship> shipList, Random rand)
        {
            bool isValid = false;
            int choice = -1;
            Battleship chosenShip = null;
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
