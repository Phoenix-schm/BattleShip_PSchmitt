using System;
using System.Collections.Generic;
using System.Security;
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
        /// Displays the player grids.
        /// </summary>
        /// <param name="player"></param>
        public static void DisplayPlayerGrids(Player player)
        {
            Console.WriteLine(player.name + " Grids");
            Console.WriteLine("       -Target Grid-       ");
            TargetGrid.DisplayTargetGrid(player.playerTargetGrid);
            //Console.WriteLine();
            Console.WriteLine("        -Ocean Grid-          ");
            OceanGrid.DisplayOceanGrid(player.playerOceanGrid);
            Console.WriteLine();
        }
    }
}
