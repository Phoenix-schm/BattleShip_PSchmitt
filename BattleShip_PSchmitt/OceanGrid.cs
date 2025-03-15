namespace BattleShip_PSchmitt
{
    class OceanGrid
    {
        static Dictionary<char, ConsoleColor> oceanGridColors = new Dictionary<char, ConsoleColor>()
        {
            {'~', ConsoleColor.DarkBlue},                           // ocean
            // Non-hit ship spaces          Hit ship spaces
            {'d', ConsoleColor.Green}, {'Z', ConsoleColor.Red},
            {'s', ConsoleColor.Green}, {'Y', ConsoleColor.Red},
            {'c', ConsoleColor.Green}, {'X', ConsoleColor.Red},
            {'B', ConsoleColor.Green}, {'W', ConsoleColor.Red},
            {'C', ConsoleColor.Green}, {'V', ConsoleColor.Red},

            {'N', ConsoleColor.DarkRed},                            // Color of a fully sunk ship
            {'M', ConsoleColor.White}                               // an opponent missed target
        };

        /// <summary>
        /// Displays the inputed grid with numbered axises 
        /// Meant for showing player ships
        /// </summary>
        /// <param name="player">The player grid being displayed.</param>
        public static void DisplayOceanGrid(BasePlayer player)
        {
            char[,] displayOceanGrid = player.oceanGrid;
            List<Battleship> playerShipList = player.shipList;
            string[] numberedAxis = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10" };

            Console.WriteLine("         -Ocean Grid-          ");

            string numberedX_axis = string.Join(" ", numberedAxis);
            Console.WriteLine("  " + numberedX_axis);                                               // Displays the numbers of the x axis

            for (int y_axis = 0; y_axis < displayOceanGrid.GetLength(0); y_axis++)
            {
                Console.Write(numberedAxis[y_axis] + " ");                                          // Displays the numbers of each y axis
                for (int x_axis = 0; x_axis < displayOceanGrid.GetLength(1); x_axis++)
                {
                    char charAtIndex = displayOceanGrid[y_axis, x_axis];
                    Console.ForegroundColor = oceanGridColors[charAtIndex];                 // Changes color based on char at [y,x]

                    if (charAtIndex == '~')                                                 // Displays ocean
                    {
                        Console.Write(charAtIndex + "  ");
                    }
                    else if (IsCharShipDisplayWhenNuetral(charAtIndex, playerShipList))     // Displays unhit ships
                    {
                        Console.Write('S' + "  ");
                    }
                    else // Displays opponent targets: sunk, hit, and misses
                    {
                        Console.Write('*' + "  ");
                    }
                }
                Console.ResetColor();
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Horizontal placement of ships onto the ocean grid.
        /// </summary>
        /// <param name="player">The current player being updated</param>
        /// <param name="chosenShip">The ship being placed onto the board.</param>
        /// <param name="direction">Whether the ship will be placed forwards or backwards.</param>
        /// <param name="userCoordinates">The coordinates the the ship will be placed at.</param>
        /// <param name="canShipBePlaced">The check if the ship can be placed at the user coordinates.</param>
        /// <returns>Modified ocean grid.</returns>
        public static char[,] PlaceShipOnOceanGrid(BasePlayer player, Battleship chosenShip, BasePlayer.DirectionList direction, int[] userCoordinates, ref bool canShipBePlaced)
        {
            int y_axis = userCoordinates[0];
            int x_axis = userCoordinates[1];
            if (direction == BasePlayer.DirectionList.Up || direction == BasePlayer.DirectionList.Down)           // If ship is being placed vertically
            {
                player.oceanGrid = FlipGameGridXYAxis(player.oceanGrid);
                y_axis = userCoordinates[1];                                                        // Flip x/y to accomodate flip
                x_axis = userCoordinates[0];
            }

            canShipBePlaced = CheckCanShipBePlaced(player.oceanGrid, chosenShip, direction, y_axis, x_axis);   // Whether the ship can be placed at the user coordinates

            if (canShipBePlaced)
            {
                player.oceanGrid = PlaceShipOnOceanGrid_BasedOnDirection(player.oceanGrid, chosenShip, direction, y_axis, x_axis);
            }

            if (direction == BasePlayer.DirectionList.Up || direction == BasePlayer.DirectionList.Down)           // Undoes board flip.
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
        /// <param name="y">The y coordinate the ship is being placed.</param>
        /// <param name="x">The x coordinate the ship is being placed.</param>
        /// <returns>The modified ocean grid with the placed ship.</returns>
        static char[,] PlaceShipOnOceanGrid_BasedOnDirection(char[,] currentOceanGrid, Battleship chosenShip, BasePlayer.DirectionList direction, int y, int x)
        {
            switch (direction)
            {
                case BasePlayer.DirectionList.Down:
                case BasePlayer.DirectionList.Right:
                    for (int shipLength = 0; shipLength < chosenShip.ShipLength; shipLength++, x++)
                    {
                        currentOceanGrid[y, x] = chosenShip.DisplayNuetral;

                        if (direction == BasePlayer.DirectionList.Down)
                        {
                            chosenShip.EachIndexOnOceanGrid.Add([x, y]);                  // Inverse to [x,y] to its "proper" coordinates on the board 
                        }
                        else
                        {
                            chosenShip.EachIndexOnOceanGrid.Add([y, x]);
                        }
                    }
                    break;
                case BasePlayer.DirectionList.Up:
                case BasePlayer.DirectionList.Left:
                    for (int shipLength = 0; shipLength < chosenShip.ShipLength; shipLength++, x--)
                    {
                        currentOceanGrid[y, x] = chosenShip.DisplayNuetral;

                        if (direction == BasePlayer.DirectionList.Up)
                        {
                            chosenShip.EachIndexOnOceanGrid.Add([x, y]);                  // Inverse to [x,y] to its "proper" coordinates on the board 
                        }
                        else
                        {
                            chosenShip.EachIndexOnOceanGrid.Add([y, x]);
                        }
                    }

                    break;
            }

            return currentOceanGrid;
        }

        /// <summary>
        /// Checks if the chosenShip can be placed at the user coordinates in direction.
        /// </summary>
        /// <param name="currentOceanGrid">The current modified grid being checked</param>
        /// <param name="chosenShip">The current ship being placed onto the board</param>
        /// <param name="direction">The direction the ship is being placed.</param>
        /// <param name="y">The y coordinate being checked</param>
        /// <param name="x">The x coordinate being checked.</param>
        /// <returns>Returns a bool of whether the chosenShip can be placed at coordinates.</returns>
        static bool CheckCanShipBePlaced(char[,] currentOceanGrid, Battleship chosenShip, BasePlayer.DirectionList direction, int y, int x)
        {
            bool isValidIndex = false;
            int canShipFitHere = 0;
            switch (direction)
            {
                case BasePlayer.DirectionList.Down: case BasePlayer.DirectionList.Right:
                    while (x != currentOceanGrid.GetLength(1) && currentOceanGrid[y, x] == '~') // If x hasn't hit the edge of the grid and another ship
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
                        x++;
                    }
                    break;
                case BasePlayer.DirectionList.Up: case BasePlayer.DirectionList.Left:
                    while (x != currentOceanGrid.GetLowerBound(1) - 1 && currentOceanGrid[y, x] == '~') // If x hasn't hit the edge of the grid and another ship
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
                        x--;
                    }
                    break;
            }

            return isValidIndex;
        }

        /// <summary>
        /// Flips the x axis to the y axis and vice versa. For vertical outputs
        /// </summary>
        /// <param name="normalGrid">The grid that is being flipped</param>
        /// <returns>flipped grid.</returns>
        static char[,] FlipGameGridXYAxis(char[,] normalGrid)
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
        /// checks if the inputed char is a nuetral ship display
        /// </summary>
        /// <param name="checkChar">the char being checked</param>
        /// <param name="shipList">the shiplist being checked</param>
        /// <returns>True if the checkedChar is a nuetral display. False if checkedChar is anything else.</returns>
        static bool IsCharShipDisplayWhenNuetral(char checkChar, List<Battleship> shipList)
        {
            bool isCharNuetralShip = false;
            foreach (Battleship ship in shipList)
            {
                if (checkChar == ship.DisplayNuetral)
                {
                    isCharNuetralShip = true;
                    break;
                }
            }
            return isCharNuetralShip;
        }
    }
}
