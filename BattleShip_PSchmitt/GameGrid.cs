using System;
using System.Collections.Generic;
using System.Security;
namespace BattleShip_PSchmitt
{
    internal class GameGrid : Battleship
    {
        protected List<int[]> destroyerSpaces = [];
        protected List<int[]> submarineSpaces = [];
        protected List<int[]> cruiserSpaces = [];
        protected List<int[]> battleshipSpaces = [];
        protected List<int[]> carrierSpaces = [];
        
        /// <summary>
        /// Creation of each battleship
        /// </summary>
        /// <returns>An array of each batteship: amount of int spaces it takes up, an empty List of spaces it takes on a grid,
        /// and its string name</returns>
        public List<Battleship> CreateShips()
        {
            Battleship destroyerShip = new Battleship((int)BattleshipList.Destroyer, destroyerSpaces, BattleshipList.Destroyer.ToString(), 'D');
            Battleship submarineShip = new Battleship((int)BattleshipList.Submarine, submarineSpaces, BattleshipList.Submarine.ToString(), 'C');
            Battleship cruiserShip = new Battleship((int)BattleshipList.Cruiser, cruiserSpaces, BattleshipList.Cruiser.ToString(), 'B');
            Battleship battleShip = new Battleship((int)BattleshipList.Battleship, battleshipSpaces, BattleshipList.Battleship.ToString(), 'A');
            Battleship carrierShip = new Battleship((int)BattleshipList.Carrier, carrierSpaces, BattleshipList.Carrier.ToString(), 'S');

            return  [destroyerShip, submarineShip, cruiserShip, battleShip, carrierShip ];
        }

        /// <summary>
        /// Creates the default version of the battleship grid filled with '~'
        /// </summary>
        /// <returns>returns a char[,] filled with '~'</returns>
        public static char[,] CreateDefaultGrid()
        {
            char[,] grid = new char[10, 10];

            for (int y_axis = 0;  y_axis < grid.GetLength(0); y_axis++)
            {
                for (int x_axis = 0; x_axis < grid.GetLength(1); x_axis++)
                {
                    grid[y_axis, x_axis] = '~';
                }
            }
            return grid;
        }
        /// <summary>
        /// Displays the inputted grid with numbered axis'. Currently only displays '~'
        /// Meant for showing player ships
        /// </summary>
        /// <param name="displayGrid">Grid to be displayed</param>
        public void DisplayOceanGrid(char[,] displayGrid)
        {
            string[] numberedY_axis = NumberedGridAxis();
            string numberedX_axis = string.Join(" ", NumberedGridAxis());
            Console.WriteLine("  " + numberedX_axis);                                          // Displays the numbers of the x axis

            for(int y_axis = 0; y_axis < displayGrid.GetLength(0); y_axis++)
            {
                Console.Write(numberedY_axis[y_axis] + " ");                              // Displays the numbers of each y axis
                for (int x_axis = 0; x_axis < displayGrid.GetLength(1); x_axis++)
                {
                    if (displayGrid[y_axis, x_axis] == '~')                             // Displays blue '~'
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.Write(displayGrid[y_axis, x_axis] + "  ");
                    }
                    else
                    {
                        Console.Write("$" + "  ");
                    }
                }
                Console.ResetColor();
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Displays the inputted grid with numbered axis'.
        /// Meant for showing player shots
        /// </summary>
        /// <param name="displayGrid"></param>
        public void DisplayTargetGrid(char[,] displayGrid)
        {
            string numberedX_axis = string.Join(" ", NumberedGridAxis());
            Console.WriteLine("  " + numberedX_axis);                                          // Displays the numbers of the x axis

            string[] numberedY_axis = NumberedGridAxis();
            for (int y_axis = 0; y_axis < displayGrid.GetLength(0); y_axis++)
            {
                Console.Write(numberedY_axis[y_axis] + " ");                              // Displays the numbers of each y axis
                for (int x_axis = 0; x_axis < displayGrid.GetLength(1); x_axis++)
                {
                    if (displayGrid[y_axis, x_axis] == '~')                             // Displays blue '~'
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.Write(displayGrid[y_axis, x_axis] + "  ");
                    }
                }
                Console.ResetColor();
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        public static string[] NumberedGridAxis()
        {
            return ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10" ];
        }
        public void DisplayPlayerGrids(Player player)
        {
            Console.WriteLine("       -Target Grid-       ");
            player.DisplayTargetGrid(player._playerTargetGrid);
            Console.WriteLine();
            Console.WriteLine("        -Ocean Grid-          ");
            player.DisplayOceanGrid(player._playerOceanGrid);
        }
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

        public char[,] PlaceShipsOnOceanGrid(char[,] currentOceanGrid, Battleship chosenShip, int direction, int[] userCoordinate, ref bool canShipBePlaced)
        {
            canShipBePlaced = CheckCanShipBePlaced(currentOceanGrid, chosenShip, direction, userCoordinate);
            int userY = userCoordinate[0];
            int userX = userCoordinate[1];

            if (direction == 0 || direction == 1)
            {
                currentOceanGrid = FlipGameGridXYAxis(currentOceanGrid);
                userX = userCoordinate[0];
                userY = userCoordinate[1];
            }

            if (canShipBePlaced && (direction == 0 || direction == 3))
            {
                for (int shipLength = 0; shipLength < chosenShip.shipLength; shipLength++, userX++)
                {
                    currentOceanGrid[userY, userX] = chosenShip._display;
                }
            }
            else if (canShipBePlaced && (direction == 1 || direction == 2))
            {
                for (int shipLength = 0; shipLength < chosenShip.shipLength; shipLength++, userX--)
                {
                    currentOceanGrid[userY, userX] = chosenShip._display;
                }
            }
            else
            {
                Console.WriteLine("That was not a valid coordinate");
            }

            if (direction == 0 || direction == 1)
            {
                currentOceanGrid = FlipGameGridXYAxis(currentOceanGrid);
            }
            return currentOceanGrid;
        }
        public static bool CheckCanShipBePlaced(char[,] currentOceanGrid, Battleship chosenShip,int direction, int[] index)
        {
            bool isValidIndex = false;
            int userY = index[0];
            int userX = index[1];
            int canShipFitHere = 0;

            if (direction == 0 || direction == 1)
            {
                currentOceanGrid = FlipGameGridXYAxis(currentOceanGrid);
                userX = index[0];
                userX = index[1];
            }

            if (direction == 0 || direction == 3)
            {
                while(userX != currentOceanGrid.GetLength(1) && currentOceanGrid[userY, userX] == '~')
                {
                    if (canShipFitHere < chosenShip.shipLength)
                    {
                        canShipFitHere++;
                    }
                    else if (canShipFitHere == chosenShip.shipLength)
                    {
                        isValidIndex = true;
                        break;
                    }
                    userX++;
                }
            }
            else if (direction == 1 ||  direction == 2)
            {
                while (userX !< currentOceanGrid.GetLowerBound(1) && currentOceanGrid[userY, userX] == '~')
                {
                    if (canShipFitHere < chosenShip.shipLength)
                    {
                        canShipFitHere++;
                    }
                    else if (canShipFitHere == chosenShip.shipLength)
                    {
                        isValidIndex = true;
                        break;
                    }
                    userX--;
                }
            }

            return isValidIndex;
        }

        public char[,] PlaceShotsOnTargetGrid(char[,] currentTargetGrid, int[] chosenShotIndex)
        {
            return currentTargetGrid;
        }
    }
}
