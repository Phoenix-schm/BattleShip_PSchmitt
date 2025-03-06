﻿namespace BattleShip_PSchmitt
{
    internal class TargetGrid
    {
        /// <summary>
        /// Displays the inputted grid with numbered axis'.
        /// Meant for showing player shots
        /// </summary>
        /// <param name="displayGrid"></param>
        public static void DisplayTargetGrid(char[,] displayGrid)
        {
            string[] numberedAxis = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10" };

            string numberedX_axis = string.Join(" ", numberedAxis);
            Console.WriteLine("  " + numberedX_axis);                                          // Displays the numbers of the x axis

            for (int y_axis = 0; y_axis < displayGrid.GetLength(0); y_axis++)
            {
                Console.Write(numberedAxis[y_axis] + " ");                              // Displays the numbers of each y axis
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
        public static char[,] PlaceShotsOnTargetGrid(Player currentPlayer, Player opponentPlayer, int chosenShot_y, int chosenShot_x)
        {
            char[,] playerTargetGrid = currentPlayer.playerTargetGrid;
            char[,] opponentOceanGrid = opponentPlayer.playerTargetGrid;
            string opponentName = opponentPlayer.name;

            if (opponentOceanGrid[chosenShot_y, chosenShot_x] != '~')
            {
                Console.WriteLine(opponentName + ": Ack! It's a hit.");
                playerTargetGrid[chosenShot_y, chosenShot_x] = 'H';
                opponentOceanGrid[chosenShot_y, chosenShot_x] = 'H';
            }
            else
            {
                Console.WriteLine(opponentName + ": That's a miss.");
                playerTargetGrid[chosenShot_y, chosenShot_x] = 'M';
            }
            return playerTargetGrid;
        }
    }
}
