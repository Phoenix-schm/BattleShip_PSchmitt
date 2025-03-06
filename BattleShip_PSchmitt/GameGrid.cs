using System;
using System.Collections.Generic;
namespace BattleShip_PSchmitt
{
    internal class GameGrid
    {
        /// <summary>
        /// Creates the default version of the battleship grid filled with '~'
        /// </summary>
        /// <returns>returns a char[,] filled with '~'</returns>
        public static char[,] CreateDefaultGrid()
        {
            char[,] grid = new char[10, 10];

            for (int y_axis = 0; y_axis < grid.GetLength(0); y_axis++)
            {
                for (int x_axis = 0; x_axis < grid.GetLength(1); x_axis++)
                {
                    grid[y_axis, x_axis] = '~';
                }
            }
            return grid;
        }

        /// <summary>
        /// Displays the inputted grid with numbered axis'.
        /// Meant for showing player shots
        /// </summary>
        /// <param name="displayGrid"></param>
        public static void DisplayTargetGrid(char[,] displayGrid)
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
                    else if (displayGrid[y_axis, x_axis] == 'H')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(displayGrid[y_axis, x_axis] + "  ");
                    }
                    else if (displayGrid[y_axis, x_axis] == 'M')
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(displayGrid[y_axis, x_axis] + "  ");
                    }
                }
                Console.ResetColor();
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        /// <summary>
        /// The numbered axis of the grid. To be used for both grids y and x axis'
        /// </summary>
        /// <returns>a list of numbers from 01 - 10</returns>
        public static string[] NumberedGridAxis()
        {
            return ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10"];
        }
        /// <summary>
        /// Displays the player grids.
        /// </summary>
        /// <param name="player"></param>
        public void DisplayPlayerGrids(Player player)
        {
            Console.WriteLine("       -Target Grid-       ");
            DisplayTargetGrid(player.playerTargetGrid);
            Console.WriteLine();
            Console.WriteLine("        -Ocean Grid-          ");
            OceanGrid.DisplayOceanGrid(player.playerOceanGrid);
        }

        public static char[,] PlaceShotsOnTargetGrid(Player currentPlayer, Player opponentPlayer, int chosenShot_y, int chosenShot_x)
        {
            char[,] playerTargetGrid = currentPlayer.playerTargetGrid;
            char[,] opponentOceanGrid = opponentPlayer.playerTargetGrid;

            if (opponentOceanGrid[chosenShot_y, chosenShot_x] != '~')
            {

            }
            return playerTargetGrid;
        }
    }
}
