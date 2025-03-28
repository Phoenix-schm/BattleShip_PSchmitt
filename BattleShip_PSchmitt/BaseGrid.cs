﻿namespace BattleShip_PSchmitt
{
    class BaseGrid
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
        /// Displays the inputed player grids. Will only display HumanPlayers
        /// </summary>
        /// <param name="currentPlayer">The current player being displayed.</param>
        public static void DisplayPlayerGrids(HumanPlayer currentPlayer)
        {
            Console.WriteLine(currentPlayer.name + " Grids:");
            TargetGrid.DisplayTargetGrid(currentPlayer);
            OceanGrid.DisplayOceanGrid(currentPlayer);
            Console.WriteLine();
        }
    }
}
