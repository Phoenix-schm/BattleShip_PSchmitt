﻿using System.Numerics;

namespace BattleShip_PSchmitt
{
    class TargetGrid
    {
        public static Dictionary<char, ConsoleColor> targetGridColors = new Dictionary<char, ConsoleColor>()
        {
            {'~', ConsoleColor.DarkBlue },
            {'M', ConsoleColor.White },
            {'H', ConsoleColor.DarkRed }
        };
        /// <summary>
        /// Displays the inputted grid with numbered axis'.
        /// Meant for showing player shots
        /// </summary>
        /// <param name="displayGrid"></param>
        public static void DisplayTargetGrid(char[,] displayGrid)
        {
            string[] numberedAxis = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10" };

            string numberedX_axis = string.Join(" ", numberedAxis);
            Console.WriteLine("  " + numberedX_axis);                                           // Displays the numbers of the x axis

            for (int y_axis = 0; y_axis < displayGrid.GetLength(0); y_axis++)
            {
                Console.Write(numberedAxis[y_axis] + " ");                                      // Displays the numbers of each y axis
                for (int x_axis = 0; x_axis < displayGrid.GetLength(1); x_axis++)
                {
                    Console.ForegroundColor = targetGridColors[displayGrid[y_axis, x_axis]];
                    Console.Write(displayGrid[y_axis, x_axis] + "  ");
                }
                Console.ResetColor();
                Console.WriteLine();
            }
            //Console.WriteLine();
        }
        public static string PlaceShotsOnTargetGrid(Player currentPlayer, Player opponentPlayer, int chosenShot_y, int chosenShot_x)
        {
            char[,] playerTargetGrid = currentPlayer.playerTargetGrid;
            char[,] opponentOceanGrid = opponentPlayer.playerOceanGrid;
            List<Battleship> opponentShips = opponentPlayer.playerShipList;
            string opponentName = opponentPlayer.name;
            string message = "";

            //Console.WriteLine(currentPlayer.name + " shoots coordinate " + chosenShot_x + "," + chosenShot_y + ".");
            Battleship? hitShip = ReturnHitShip(chosenShot_y, chosenShot_x, opponentOceanGrid, opponentShips);
            currentPlayer.previousShot = [chosenShot_y, chosenShot_x];
            if (hitShip != null)
            {
                hitShip.EachIndexSpace.RemoveAt(0);

                message = opponentName + ": Ack! It's a hit.";
                playerTargetGrid[chosenShot_y, chosenShot_x] = 'H';
                opponentOceanGrid[chosenShot_y, chosenShot_x] = 'H';

                if (!hitShip.IsStillFloating)                               // if the ship has been sunk.
                {
                    message += "\nYou sunk my battleship!";
                    opponentShips.Remove(hitShip);
                }
            }
            else
            {
                message = opponentName + ": That's a miss.";
                playerTargetGrid[chosenShot_y, chosenShot_x] = 'M';
            }
            return message;
        }

        public static Battleship ReturnHitShip(int y, int x, char[,] oceanGrid, List<Battleship> shipList)
        {
            Battleship hitShip = null;

            foreach (Battleship ship in shipList)
            {
                if (oceanGrid[y,x] == ship.Display)
                {
                    hitShip = ship;
                }
            }

            return hitShip;
        }

        public static int[] ReturnValidUserCoordinates(Player player)
        {
            bool isValidCoordinates = false;
            int yCoord = -1;
            int xCoord = -1;
            while(!isValidCoordinates)
            {
                yCoord = PlayerInput.CheckInputNumIsOnGrid("Choose a y coordinate to shoot");
                xCoord = PlayerInput.CheckInputNumIsOnGrid("Choose an x coordinate to shoot");

                if (player.playerTargetGrid[yCoord, xCoord] == 'M' || player.playerTargetGrid[yCoord, xCoord] == 'H')
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("You've already hit that coordinate");
                    Console.ResetColor();
                }
                else
                {
                    isValidCoordinates = true;
                }
            }

            return [yCoord, xCoord];
        }
    }
}
