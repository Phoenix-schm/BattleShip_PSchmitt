namespace BattleShip_PSchmitt
{
    class Player : PlayerBase
    {
        public Player(string newName)
        {
            name = newName;
        }

        /// <summary>
        /// The entire encapsulation of a player turn. Display player grids, Display shot message (if there is one), get player coordinates,
        /// Shoot at player coordinates and create shotMessage, Display modified player grids and shot message
        /// </summary>
        /// <param name="currentPlayer">The current player that is playing</param>
        /// <param name="opponentPlayer">The opposing player</param>
        /// <param name="shotMessage">The current shot message</param>
        /// <returns>The shotMessage of the current player.</returns>
        public static string PlayerTurn(PlayerBase currentPlayer, PlayerBase opponentPlayer, string shotMessage)
        {
            GameGrid.DisplayPlayerGrids(currentPlayer);
            DisplayShotTakenMessage(opponentPlayer, shotMessage, ConsoleColor.Red);   // Displays the shot message of the previous previous player shot

            int[] playerCoordinates = PlayerInput.ReturnValidUserCoordinates(currentPlayer);                             // askss for [y,x] coordinates from player
            shotMessage = TargetGrid.PlaceShotsOnTargetGrid(currentPlayer, opponentPlayer, playerCoordinates[0], playerCoordinates[1]); // shoots, creates a message from shot, clear console
            BattleshipGame.FullyClearConsole();

            GameGrid.DisplayPlayerGrids(currentPlayer);
            DisplayShotTakenMessage(currentPlayer, shotMessage, ConsoleColor.Cyan);  // Displays the shot message of the current player shot
            currentPlayer.shotsTaken++;

            return shotMessage;
        }

        /// <summary>
        /// When a shot is made on the board, a shot message is created. This method displays that message along with who just shot and where. 
        /// </summary>
        /// <param name="displayPlayer"></param>
        /// <param name="shotMessage">The shot message that was returned by TargetGrid.PlaceShotsOnTargetGrid()</param>
        /// <param name="color">The color the whole thing will display as</param>
        static void DisplayShotTakenMessage(PlayerBase displayPlayer, string shotMessage, ConsoleColor color)
        {
            if (displayPlayer.previousShot != null)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(displayPlayer.name + " Turn:");
                Console.WriteLine("----------------------");
                Console.WriteLine(displayPlayer.name + " shoots coordinate " + (displayPlayer.previousShot[0] + 1) + "," + (displayPlayer.previousShot[1] + 1) + ".");
                Console.WriteLine(shotMessage);
                Console.WriteLine();
            }
            Console.ResetColor();
        }
    }
}
