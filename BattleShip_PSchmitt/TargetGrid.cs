namespace BattleShip_PSchmitt
{
    class TargetGrid : BaseGrid
    {
        public static Dictionary<char, ConsoleColor> targetGridColors = new Dictionary<char, ConsoleColor>()
        {
            {'~', ConsoleColor.DarkBlue},
            {'M', ConsoleColor.White},      // Missed targets
            {'H', ConsoleColor.Red},        // Hit ships
            {'N', ConsoleColor.DarkRed}     // Color of a fully sunk ship 
        };

        /// <summary>
        /// Displays the inputed grid with numbered axises
        /// Meant for showing player shots
        /// </summary>
        /// <param name="player">The player that's target grid will be displayed</param>
        public static void DisplayTargetGrid(BasePlayer player)
        {
            char[,] displayTargetGrid = player.targetGrid;
            string[] numberedAxis = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10" };

            Console.WriteLine("         -Target Grid-       ");

            string numberedX_axis = string.Join(" ", numberedAxis);
            Console.WriteLine("  " + numberedX_axis);                                           // Displays the numbers of the x axis

            for (int y_axis = 0; y_axis < displayTargetGrid.GetLength(0); y_axis++)
            {
                Console.Write(numberedAxis[y_axis] + " ");                                      // Displays the numbers of each y axis
                for (int x_axis = 0; x_axis < displayTargetGrid.GetLength(1); x_axis++)
                {
                    char charAtIndex = displayTargetGrid[y_axis, x_axis];
                    Console.ForegroundColor = targetGridColors[charAtIndex];

                    if (charAtIndex == player.targetSunkDisplay)
                    {
                        Console.Write(player.targetHitDisplay + "  ");
                    }
                    else    // Displays hit, miss and ocean displays
                    {
                        Console.Write(charAtIndex + "  ");
                    }
                }
                Console.ResetColor();
                Console.WriteLine();
            }
        }
        /// <summary>
        /// Places a shot on the players Target Grid. Either 'H' for hitting a ship or 'M' for a missed shot.
        /// </summary>
        /// <param name="currentPlayer">The player doing the shooting.</param>
        /// <param name="opponentPlayer">The player being hit.</param>
        /// <param name="chosenShot_y">The y coordinate being shot at.</param>
        /// <param name="chosenShot_x">The x coordinate being shot at.</param>
        /// <returns></returns>
        public static string PlaceShotsOnTargetGrid(BasePlayer currentPlayer, BasePlayer opponentPlayer, int chosenShot_y, int chosenShot_x)
        {
            // Variable initializations
            char[,] playerTargetGrid = currentPlayer.targetGrid;
            char[,] opponentOceanGrid = opponentPlayer.oceanGrid;
            List<Battleship> opponentShips = opponentPlayer.shipList;
            string opponentName = opponentPlayer.name;
            string shotMessage;                                             // The message displayed to the player after a turn

            Battleship? hitShip = ReturnHitShip(chosenShot_y, chosenShot_x, opponentOceanGrid, opponentShips);  // Whether there is a ship being hit.
            currentPlayer.previousShot = [chosenShot_y, chosenShot_x];                                          // Initializes the players shot for display and cpu AI purposes

            if (hitShip != null)                                            // if a ship has been hit
            {
                hitShip.ShipLength--;

                shotMessage = opponentName + ": Ack! It's a hit.";
                playerTargetGrid[chosenShot_y, chosenShot_x] = currentPlayer.targetHitDisplay;
                opponentOceanGrid[chosenShot_y, chosenShot_x] = hitShip.DisplayWhenHit;

                if (!hitShip.IsStillFloating)                               // if the ship has been sunk (if the ships length is now 0).
                {
                    shotMessage += "\n" + opponentPlayer.name + ": You sunk my battleship!";
                    for (int index = 0; index < hitShip.EachIndexOnOceanGrid.Count; index++)    // Replace ship index positions with the "Sunk Ship" display
                    {
                        int y = hitShip.EachIndexOnOceanGrid[index][0];
                        int x = hitShip.EachIndexOnOceanGrid[index][1];
                        playerTargetGrid[y, x] = currentPlayer.targetSunkDisplay;
                        opponentOceanGrid[y, x] = currentPlayer.targetSunkDisplay;
                    }
                    opponentShips.Remove(hitShip);
                }
            }
            else
            {
                shotMessage = opponentName + ": That's a miss.";
                playerTargetGrid[chosenShot_y, chosenShot_x] = currentPlayer.targetMissDisplay;
                opponentOceanGrid[chosenShot_y, chosenShot_x] = currentPlayer.targetMissDisplay;
            }
            return shotMessage;
        }

        /// <summary>
        /// Returns whether the coordinate at [y,x] is a hit ship 
        /// </summary>
        /// <param name="y">The y coordinate being hit.</param>
        /// <param name="x">The x coordinate being hit.</param>
        /// <param name="opponentOceanGrid">The ocean grid being shot at.</param>
        /// <param name="opponentShipList">The list of ships the method will be checking.</param>
        /// <returns>Returns 'null' if a ship wasn't hit.
        /// Returns Battship from _opponentShipList if a ship was hit.</returns>
        static Battleship? ReturnHitShip(int y, int x, char[,] opponentOceanGrid, List<Battleship> opponentShipList)
        {
            Battleship? hitShip = null;

            foreach (Battleship ship in opponentShipList)
            {
                if (opponentOceanGrid[y, x] == ship.DisplayNuetral)           // if the [y,x] coordinate hits a ship
                {
                    hitShip = ship;
                }
            }
            return hitShip;
        }
    }
}
