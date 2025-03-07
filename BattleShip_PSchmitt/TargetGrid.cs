namespace BattleShip_PSchmitt
{
    class TargetGrid
    {
        public static Dictionary<char, ConsoleColor> targetGridColors = new Dictionary<char, ConsoleColor>()
        {
            {'~', ConsoleColor.DarkBlue },
            {'M', ConsoleColor.White },
            {'H', ConsoleColor.DarkRed }
        };
        /// <summary>
        /// Displays the inputted grid with numbered axis'.
        /// Meant for showing player shots
        /// </summary>
        /// <param name="displayGrid"></param>
        public static void DisplayTargetGrid(char[,] displayGrid)
        {
            string[] numberedAxis = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10" };

            string numberedX_axis = string.Join(" ", numberedAxis);
            Console.WriteLine("  " + numberedX_axis);                                           // Displays the numbers of the x axis

            for (int y_axis = 0; y_axis < displayGrid.GetLength(0); y_axis++)
            {
                Console.Write(numberedAxis[y_axis] + " ");                                      // Displays the numbers of each y axis
                for (int x_axis = 0; x_axis < displayGrid.GetLength(1); x_axis++)
                {
                    Console.ForegroundColor = targetGridColors[displayGrid[y_axis, x_axis]];
                    Console.Write(displayGrid[y_axis, x_axis] + "  ");
                }
                Console.ResetColor();
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        public static void PlaceShotsOnTargetGrid(Player currentPlayer, Player opponentPlayer, int chosenShot_y, int chosenShot_x)
        {
            char[,] playerTargetGrid = currentPlayer.playerTargetGrid;
            char[,] opponentOceanGrid = opponentPlayer.playerOceanGrid;
            List<Battleship> opponentShips = opponentPlayer.playerShipList;
            string opponentName = opponentPlayer.name;

            Console.WriteLine(currentPlayer.name + " shoots coordinate " + chosenShot_x + "," + chosenShot_y + ".");
            Battleship? hitShip = ReturnHitShip(chosenShot_y, chosenShot_x, opponentOceanGrid, opponentShips);

            if (hitShip != null)
            {
                hitShip.EachIndexSpace.RemoveAt(0);

                Console.WriteLine(opponentName + ": Ack! It's a hit.");
                playerTargetGrid[chosenShot_y, chosenShot_x] = 'H';
                opponentOceanGrid[chosenShot_y, chosenShot_x] = 'H';

                if (!hitShip.IsStillFloating)                               // if the ship has been sunk.
                {
                    Console.WriteLine("You sunk my battleship!");
                    opponentShips.Remove(hitShip);
                }
            }
            else
            {
                Console.WriteLine(opponentName + ": That's a miss.");
                playerTargetGrid[chosenShot_y, chosenShot_x] = 'M';
            }

        }

        public static Battleship ReturnHitShip(int y, int x, char[,] oceanGrid, List<Battleship> shipList)
        {
            Battleship hitShip = null;

            foreach (Battleship ship in shipList)
            {
                if (oceanGrid[y,x] == ship.Display)
                {
                    hitShip = ship;
                }
            }

            return hitShip;
        }
    }
}
