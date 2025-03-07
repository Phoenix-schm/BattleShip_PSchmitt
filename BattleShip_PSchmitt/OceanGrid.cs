namespace BattleShip_PSchmitt
{
    internal class OceanGrid
    {
        public static Dictionary<char, ConsoleColor> oceanGridColors = new Dictionary<char, ConsoleColor>()
        {
            {'~', ConsoleColor.DarkBlue },
            {'d', ConsoleColor.Green },
            {'s', ConsoleColor.Green },
            {'c', ConsoleColor.Green },
            {'B', ConsoleColor.Green },
            {'C', ConsoleColor.Green },
            {'H', ConsoleColor.DarkRed }
        };
        /// <summary>
        /// Displays the inputted grid with numbered axis'. Currently only displays '~'
        /// Meant for showing player ships
        /// </summary>
        /// <param name="displayGrid">Grid to be displayed</param>
        public static void DisplayOceanGrid(char[,] displayGrid)
        {
            string[] numberedAxis = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10" };

            string numberedX_axis = string.Join(" ", numberedAxis);
            Console.WriteLine("  " + numberedX_axis);                                       // Displays the numbers of the x axis

            for (int y_axis = 0; y_axis < displayGrid.GetLength(0); y_axis++)
            {
                Console.Write(numberedAxis[y_axis] + " ");                                  // Displays the numbers of each y axis
                for (int x_axis = 0; x_axis < displayGrid.GetLength(1); x_axis++)
                {
                    Console.ForegroundColor = oceanGridColors[displayGrid[y_axis, x_axis]];
                    if (displayGrid[y_axis, x_axis] != '~' && displayGrid[y_axis, x_axis] != 'H')
                    {
                        Console.Write('S' + "  ");
                    }
                    else
                    {
                        Console.Write(displayGrid[y_axis, x_axis] + "  ");
                    }
                }
                Console.ResetColor();
                Console.WriteLine();
            }
            //Console.WriteLine();
        }

        /// <summary>
        /// Flips the x axis to the y axis and vice versa. For vertical outputs
        /// </summary>
        /// <param name="normalGrid">The grid that is being flipped</param>
        /// <returns>flipped grid.</returns>
        public static char[,] FlipGameGridXYAxis(char[,] normalGrid)
        {
            char[,] flippedGrid = new char[normalGrid.GetLength(0), normalGrid.GetLength(1)];
            for (int x_axis = 0; x_axis < normalGrid.GetLength(1); x_axis++)
            {
                for (int y_axis = 0; y_axis < normalGrid.GetLength(0); y_axis++)
                {
                    flippedGrid[y_axis, x_axis] = normalGrid[x_axis, y_axis];
                }
            }
            return flippedGrid;
        }

        /// <summary>
        /// Horizontal placement of ships onto the ocean grid.
        /// </summary>
        /// <param name="currentOceanGrid">The current modified grid being modified.</param>
        /// <param name="chosenShip">The ship being placed onto the board.</param>
        /// <param name="direction">Whether the ship will be placed forwards or backwards.</param>
        /// <param name="coordinates">The coordinates the the ship will be placed at.</param>
        /// <param name="canShipBePlaced">The check if the ship can be placed at the user coordinates.</param>
        /// <returns>Modified ocean grid.</returns>
        public static char[,] PlaceShipsOnOceanGrid(char[,] currentOceanGrid, Battleship chosenShip, int direction, int[] coordinates, ref bool canShipBePlaced)
        {
            int y_coord = coordinates[0];
            int x_coord = coordinates[1];
            if (direction == 0 || direction == 1)                           // If ship is being placed vertically
            {
                currentOceanGrid = FlipGameGridXYAxis(currentOceanGrid);
                y_coord = coordinates[1];
                x_coord = coordinates[0];
            }

            canShipBePlaced = CheckCanShipBePlaced(currentOceanGrid, chosenShip, direction, y_coord, x_coord);   // Whether the ship can be placed at the user coordinates

            if (canShipBePlaced)
            {
                currentOceanGrid = PlaceShipOnOceanGrid(currentOceanGrid, chosenShip, direction, y_coord, x_coord);
            }

            if (direction == 0 || direction == 1)
            {
                currentOceanGrid = FlipGameGridXYAxis(currentOceanGrid);
            }
            return currentOceanGrid;
        }
        public static char[,] PlaceShipOnOceanGrid(char[,] currentOceanGrid, Battleship chosenShip, int direction, int y, int x)
        {
            if (direction == 1 || direction == 3)                                               // Going forwards
            {
                for (int shipLength = 0; shipLength < chosenShip.ShipLength; shipLength++, x++)
                {
                    currentOceanGrid[y, x] = chosenShip.Display;
                    chosenShip.EachIndexSpace.Add([y, x]);                                      // Adds coordinates to chosenShip index list
                }
            }
            else if (direction == 0 || direction == 2)                                          // Going backwards
            {
                for (int shipLength = 0; shipLength < chosenShip.ShipLength; shipLength++, x--)
                {
                    currentOceanGrid[y, x] = chosenShip.Display;
                    chosenShip.EachIndexSpace.Add([y, x]);                                      // Adds coordinates to chosenShip index list
                }
            }
            return currentOceanGrid;
        }

        /// <summary>
        /// Checks if the chosenShip can be placed at the user coordinates.
        /// </summary>
        /// <param name="currentOceanGrid">The current modified grid being checked</param>
        /// <param name="chosenShip">The current ship being placed onto the board</param>
        /// <param name="direction">The direction the ship is being placed.</param>
        /// <param name="index">The user [y,x] coordinates that are being checked.</param>
        /// <returns>Returns a bool of whether the chosenShip can be placed at coordinates.</returns>
        public static bool CheckCanShipBePlaced(char[,] currentOceanGrid, Battleship chosenShip, int direction, int y, int x)
        {
            bool isValidIndex = false;
            int userY = y;
            int userX = x;
            int canShipFitHere = 0;
            if (direction == 1 || direction == 3)
            {
                while (userX != currentOceanGrid.GetLength(1) && currentOceanGrid[userY, userX] == '~')
                {
                    if (canShipFitHere < chosenShip.ShipLength)
                    {
                        canShipFitHere++;
                    }
                    if (canShipFitHere == chosenShip.ShipLength)
                    {
                        isValidIndex = true;
                        break;
                    }
                    userX++;
                }
            }
            else if (direction == 0 || direction == 2)
            {
                while (userX != currentOceanGrid.GetLowerBound(1) - 1 && currentOceanGrid[userY, userX] == '~')
                {
                    if (canShipFitHere < chosenShip.ShipLength)
                    {
                        canShipFitHere++;
                    }
                    if (canShipFitHere == chosenShip.ShipLength)
                    {
                        isValidIndex = true;
                        break;
                    }
                    userX--;
                }
            }

            return isValidIndex;
        }
    }
}
