﻿namespace BattleShip_PSchmitt
{
    internal class OceanGrid : GameGrid
    {
        public static Dictionary<char, ConsoleColor> oceanGridColors = new Dictionary<char, ConsoleColor>()
        {
            {'~', ConsoleColor.DarkBlue},
            // Non-hit ship spaces          Hit ship spaces
            {'d', ConsoleColor.Green}, {'Z', ConsoleColor.Red},
            {'s', ConsoleColor.Green}, {'Y', ConsoleColor.Red},
            {'c', ConsoleColor.Green}, {'X', ConsoleColor.Red},
            {'B', ConsoleColor.Green}, {'W', ConsoleColor.Red},
            {'C', ConsoleColor.Green}, {'V', ConsoleColor.Red},
            {'N', ConsoleColor.DarkRed},                            // Color of a fully sunk ship
            {'M', ConsoleColor.White}
        };
        /// <summary>
        /// Displays the inputted grid with numbered axis'. Currently only displays '~'
        /// Meant for showing player ships
        /// </summary>
        /// <param name="displayGrid">Grid to be displayed</param>
        public static void DisplayOceanGrid(Player player)
        {
            char[,] playerOceanGrid = player.oceanGrid;
            List<Battleship> playerShipList = player.shipList;
            string[] numberedAxis = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10" };

            string numberedX_axis = string.Join(" ", numberedAxis);
            Console.WriteLine("  " + numberedX_axis);                                               // Displays the numbers of the x axis

            for (int y_axis = 0; y_axis < playerOceanGrid.GetLength(0); y_axis++)
            {
                Console.Write(numberedAxis[y_axis] + " ");                                          // Displays the numbers of each y axis
                for (int x_axis = 0; x_axis < playerOceanGrid.GetLength(1); x_axis++)
                {
                    Console.ForegroundColor = oceanGridColors[playerOceanGrid[y_axis, x_axis]];     // Changes color based on char at [y,x]
                    char indexOnGrid = playerOceanGrid[y_axis, x_axis];
                    if (indexOnGrid == '~')                                                 // Displays ocean
                    {
                        Console.Write(indexOnGrid + "  ");
                    }
                    else if (IsCharShipDisplayWhenNuetral(indexOnGrid, playerShipList))    // Displays unhit ships
                    {
                        Console.Write('S' + "  ");
                    }                             //        hit ship locatiosn            missed locations      sunk ships
                    else if (IsCharShipDisplayWhenHit(indexOnGrid, playerShipList) || indexOnGrid == 'M' || indexOnGrid == 'N') // Displays opponent targets
                    {
                        Console.Write('*' + "  ");
                    }
                }
                Console.ResetColor();
                Console.WriteLine();
            }
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
        /// <param name="userCoordinates">The coordinates the the ship will be placed at.</param>
        /// <param name="canShipBePlaced">The check if the ship can be placed at the user coordinates.</param>
        /// <returns>Modified ocean grid.</returns>
        public static char[,] PlaceShipOnOceanGrid(Player player, Battleship chosenShip, string direction, int[] userCoordinates, ref bool canShipBePlaced)
        {
            int y_axis = userCoordinates[0];
            int x_axis = userCoordinates[1];
            if (direction == "up" || direction == "down")                           // If ship is being placed vertically
            {
                player.oceanGrid = FlipGameGridXYAxis(player.oceanGrid);
                y_axis = userCoordinates[1];
                x_axis = userCoordinates[0];
            }

            canShipBePlaced = CheckCanShipBePlaced(player.oceanGrid, chosenShip, direction, y_axis, x_axis);   // Whether the ship can be placed at the user coordinates

            if (canShipBePlaced)
            {
                player.oceanGrid = PlaceShipOnOceanGrid_BasedOnDirection(player.oceanGrid, chosenShip, direction, y_axis, x_axis);
            }

            if (direction == "up" || direction == "down")                           // Undoes board flip.
            {
                player.oceanGrid = FlipGameGridXYAxis(player.oceanGrid);
            }

            return player.oceanGrid;
        }
        /// <summary>
        /// Updates the currentOceanGrid with the chosenShip.Display at [y,x]
        /// </summary>
        /// <param name="currentOceanGrid">The current grid being updated.</param>
        /// <param name="chosenShip">The ship being placed on the board.</param>
        /// <param name="direction">The direction the ship is being placed.</param>
        /// <param name="y_axis">The y coordinate the ship is being placed.</param>
        /// <param name="x_axis">The x coordinate the ship is being placed.</param>
        /// <returns>The modified ocean grid with the placed ship.</returns>
        public static char[,] PlaceShipOnOceanGrid_BasedOnDirection(char[,] currentOceanGrid, Battleship chosenShip, string direction, int y_axis, int x_axis)
        {
            if (direction == "down" || direction == "right")                                               // Going forwards
            {
                for (int shipLength = 0; shipLength < chosenShip.ShipLength; shipLength++, x_axis++)
                {
                    currentOceanGrid[y_axis, x_axis] = chosenShip.DisplayNuetral;
                    if (direction == "down")
                    {
                        chosenShip.EachIndexOnOceanGrid.Add([x_axis, y_axis]);                  // Inverse to [x,y] to accomodate grid flippage 
                    }
                    else
                    {
                        chosenShip.EachIndexOnOceanGrid.Add([y_axis, x_axis]);
                    }
                }
            }
            else if (direction == "up" || direction == "left")                                          // Going backwards
            {
                for (int shipLength = 0; shipLength < chosenShip.ShipLength; shipLength++, x_axis--)
                {
                    currentOceanGrid[y_axis, x_axis] = chosenShip.DisplayNuetral;
                    if (direction == "up")
                    {
                        chosenShip.EachIndexOnOceanGrid.Add([x_axis, y_axis]);                  // Inverse to [x,y] to accomodate grid flippage
                    }
                    else
                    {
                        chosenShip.EachIndexOnOceanGrid.Add([y_axis, x_axis]);
                    }
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
        /// <param name="y_axis">The y coordinate being checked</param>
        /// <param name="x_axis">The x coordinate being checked.</param>
        /// <returns>Returns a bool of whether the chosenShip can be placed at coordinates.</returns>
        public static bool CheckCanShipBePlaced(char[,] currentOceanGrid, Battleship chosenShip, string direction, int y_axis, int x_axis)
        {
            bool isValidIndex = false;
            int canShipFitHere = 0;
            if (direction == "down" || direction == "right")           // Checking ship placement to the right of coordinate
            {
                while (x_axis != currentOceanGrid.GetLength(1) && currentOceanGrid[y_axis, x_axis] == '~')
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
                    x_axis++;
                }
            }
            else if (direction == "up" || direction == "left")      // Checking ship placement to the left of coordinate
            {
                while (x_axis != currentOceanGrid.GetLowerBound(1) - 1 && currentOceanGrid[y_axis, x_axis] == '~')
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
                    x_axis--;
                }
            }

            return isValidIndex;
        }
    }
}
