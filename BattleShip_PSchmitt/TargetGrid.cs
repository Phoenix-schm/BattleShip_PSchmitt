using System.Numerics;

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
        }
        /// <summary>
        /// Places a shot on the players Target Grid. Either 'H' for hitting a ship or 'M' for a missed shot.
        /// </summary>
        /// <param name="currentPlayer">The player doing the shooting.</param>
        /// <param name="opponentPlayer">The player being hit.</param>
        /// <param name="chosenShot_y">The y coordinate being shot at.</param>
        /// <param name="chosenShot_x">The x coordinate being shot at.</param>
        /// <returns></returns>
        public static string PlaceShotsOnTargetGrid(Player currentPlayer, Player opponentPlayer, int chosenShot_y, int chosenShot_x)
        {
            // Variable initializations
            char[,] playerTargetGrid = currentPlayer.playerTargetGrid;
            char[,] opponentOceanGrid = opponentPlayer.playerOceanGrid;
            List<Battleship> opponentShips = opponentPlayer.playerShipList;
            string opponentName = opponentPlayer.name;
            string shotMessage;                                             // The message displayed to the player after a turn

            Battleship? hitShip = ReturnHitShip(chosenShot_y, chosenShot_x, opponentOceanGrid, opponentShips);  // Whether there is a ship being hit.
            currentPlayer.previousShot = [chosenShot_y, chosenShot_x];                                          // Initializes the players shot for display purposes

            if (hitShip != null)                                            // if a ship has been hit
            {
                hitShip.EachIndexOnOceanGrid.RemoveAt(0);                   // Removes an index position in the ships list of positions (doesn't matter which one)

                shotMessage = opponentName + ": Ack! It's a hit.";
                playerTargetGrid[chosenShot_y, chosenShot_x] = 'H';
                opponentOceanGrid[chosenShot_y, chosenShot_x] = 'H';

                if (!hitShip.IsStillFloating)                               // if the ship has been sunk (if all the ships index positions have been removed).
                {
                    shotMessage += "\n" + opponentPlayer.name + ": You sunk my battleship!";
                    opponentShips.Remove(hitShip);
                }
            }
            else
            {
                shotMessage = opponentName + ": That's a miss.";
                playerTargetGrid[chosenShot_y, chosenShot_x] = 'M';
                opponentOceanGrid[chosenShot_y, chosenShot_x] = 'M';
            }
            return shotMessage;
        }

        /// <summary>
        /// Returns whether the coordinate at [y,x] is a hit ship 
        /// </summary>
        /// <param name="y">The y coordinate being hit.</param>
        /// <param name="x">The x coordinate being hit.</param>
        /// <param name="_opponentOceanGrid">The ocean grid being shot at.</param>
        /// <param name="_opponentShipList">The list of ships the method will be checking.</param>
        /// <returns>Returns 'null' if a ship wasn't hit.
        /// Returns Battship from _opponentShipList if a ship was hit.</returns>
        public static Battleship? ReturnHitShip(int y, int x, char[,] _opponentOceanGrid, List<Battleship> _opponentShipList)
        {
            Battleship? hitShip = null;

            foreach (Battleship ship in _opponentShipList)
            {
                if (_opponentOceanGrid[y, x] == ship.Display)           // if the [y,x] coordinate hits a ship
                {
                    hitShip = ship;
                }
            }
            return hitShip;
        }
        /// <summary>
        /// Asks the currentPlayer to give two coordinates.
        /// If the index at those coordinates has already been shot at, then redo.
        /// </summary>
        /// <param name="currentPlayer">The current player being asked for coordinates.</param>
        /// <returns>Returns valid coordinates the player can shoot at.</returns>
        public static int[] ReturnValidUserCoordinates(Player currentPlayer)
        {
            bool isValidCoordinates = false;
            int yCoord = -1;
            int xCoord = -1;
            while (!isValidCoordinates)
            {
                yCoord = PlayerInput.CheckInputNumIsOnGrid("Choose a y coordinate to shoot");
                xCoord = PlayerInput.CheckInputNumIsOnGrid("Choose an x coordinate to shoot");

                if (currentPlayer.playerTargetGrid[yCoord, xCoord] == 'M' || currentPlayer.playerTargetGrid[yCoord, xCoord] == 'H')
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("You've already hit that coordinate.");
                    Console.ResetColor();
                }
                else
                {
                    isValidCoordinates = true;
                }
            }
            return [yCoord, xCoord];
        }
    }
}
