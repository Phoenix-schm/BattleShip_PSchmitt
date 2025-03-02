using System;
using System.Collections.Generic;
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
            Battleship destroyerShip = new Battleship((int)BattleshipList.Destroyer, destroyerSpaces, BattleshipList.Destroyer.ToString());
            Battleship submarineShip = new Battleship((int)BattleshipList.Submarine, submarineSpaces, BattleshipList.Submarine.ToString());
            Battleship cruiserShip = new Battleship((int)BattleshipList.Cruiser, cruiserSpaces, BattleshipList.Cruiser.ToString());
            Battleship battleShip = new Battleship((int)BattleshipList.Battleship, battleshipSpaces, BattleshipList.Battleship.ToString());
            Battleship carrierShip = new Battleship((int)BattleshipList.Carrier, carrierSpaces, BattleshipList.Carrier.ToString());

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
            string numberedX_axis = string.Join(" ", NumberedGridAxis());
            Console.WriteLine("  " + numberedX_axis);                                          // Displays the numbers of the x axis

            string[] numberedY_axis = NumberedGridAxis();
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
                        Console.Write(displayGrid[y_axis, x_axis] + "  ");
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

        public char[,] PlaceShipsOnOceanGrid(char[,] currentOceanGrid, Battleship chosenShip)
        {
            return currentOceanGrid;
        }

        public char[,] PlaceShotsOnTargetGrid(char[,] currentTargetGrid, int[] chosenShotIndex)
        {
            return currentTargetGrid;
        }
    }
}
