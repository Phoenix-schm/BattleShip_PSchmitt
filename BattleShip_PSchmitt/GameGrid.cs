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
        /// Displays the inputed player grids.
        /// </summary>
        /// <param name="currentPlayer">The current player being displayed.</param>
        public static void DisplayPlayerGrids(Player currentPlayer)
        {
            Console.ResetColor();
            Console.WriteLine(currentPlayer.name + " Grids:");
            Console.WriteLine("         -Target Grid-       ");
            TargetGrid.DisplayTargetGrid(currentPlayer);
            Console.WriteLine("         -Ocean Grid-          ");
            OceanGrid.DisplayOceanGrid(currentPlayer);
            Console.WriteLine();
        }
        /// <summary>
        /// checks if the inputted char is a ship.DisplayWhenHit
        /// </summary>
        /// <param name="checkChar">the char being checked</param>
        /// <param name="opponentPlayer">the opponent player having their shiplist checked</param>
        /// <returns>True if checkedChar a DisplayWhenHit. False if checkedChar is anything else</returns>
        public static bool IsCharShipDisplayWhenHit(char checkChar, Player opponentPlayer)
        {
            bool isCharHitShip = false;
            foreach (Battleship ship in opponentPlayer.shipList)
            {
                if (checkChar == ship.DisplayWhenHit)
                {
                    isCharHitShip = true;
                }
            }
            return isCharHitShip;
        }
        /// <summary>
        /// checks if the inputted char is a nuetral ship display
        /// </summary>
        /// <param name="checkChar">the char being checked</param>
        /// <param name="shipList">the shiplist being checked</param>
        /// <returns>True if the checkedChar is a nuetral display. False if checkedChar is anything else.</returns>
        public static bool IsCharShipDisplayWhenNuetral(char checkChar, List<Battleship> shipList)
        {
            bool isCharNuetralShip = false;
            foreach (Battleship ship in shipList)
            {
                if (checkChar == ship.DisplayNuetral)
                {
                    isCharNuetralShip = true;
                }
            }
            return isCharNuetralShip;
        }
    }
}
