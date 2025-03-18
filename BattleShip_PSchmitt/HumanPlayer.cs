namespace BattleShip_PSchmitt
{
    class HumanPlayer : BasePlayer
    {
        // for color displays
        public Dictionary<char, ConsoleColor> oceanGridColors;
        public Dictionary<char, ConsoleColor> targetGridColors;

        public HumanPlayer(string newName)
        {
            name = newName;
            oceanGridColors = CreateOceanGridColors();
            targetGridColors = CreateTargetGridColors();
        }

        /// <summary>
        /// Has player place all battleships onto the grid
        /// </summary>
        /// <param name="player">Current player placing onto the grid.</param>
        /// <returns>Returns the updated grid.</returns>
        public static char[,] CreateOceanGrid(HumanPlayer player)
        {
            List<Battleship> playerShipList = player.shipList;
            Battleship? chosenShip = null;

            bool isValidCoordinates = false;
            int doneOnce = 0;

            for (int shipCountIndex = 0; shipCountIndex < playerShipList.Count; shipCountIndex++)     // Going through the number of ships in the ship list
            {
                do
                {
                    if (doneOnce == 1 && !isValidCoordinates)                               // If the previous coordinates weren't valid
                    {
                        PlayerInput.InvalidMessage("That was not a valid coordinate.");
                        Console.WriteLine("Current Player Grid:");
                        OceanGrid.DisplayOceanGrid(player);
                    }
                    else                                                                    // Shows the grid at least once
                    {
                        Console.WriteLine("Current Player Grid:");
                        OceanGrid.DisplayOceanGrid(player);
                    }

                    Console.WriteLine("You have " + (playerShipList.Count - shipCountIndex) + " ships left to place.");
                    DisplayShipsForPlacement(playerShipList);
                    Console.WriteLine();
                    chosenShip = PlayerInput.ChooseShipToPlace(player);

                    int userY = PlayerInput.InputValidNumOnGrid("Choose a y coordinate to place the ship");
                    int userX = PlayerInput.InputValidNumOnGrid("Choose an x coodrinate to place the ship");
                    Console.WriteLine();

                    DisplayDirections();
                    DirectionList chosenDirection = PlayerInput.ChooseDirectionToPlaceShip();

                    OceanGrid.PlaceShipOnOceanGrid(player, chosenShip, chosenDirection, [userY, userX], ref isValidCoordinates);

                    doneOnce = 1;
                    BattleshipGame.FullyClearConsole();

                    if (isValidCoordinates)                         // If the player correctly placed the ship, check if they want to undo
                    {
                        Console.WriteLine("Current Player Grid:");
                        OceanGrid.DisplayOceanGrid(player);
                        PlayerInput.CheckRedoGridInput(ref chosenShip, ref shipCountIndex, player.oceanGrid);
                    }
                } while (!isValidCoordinates);
            }
            return player.oceanGrid;
        }

        /// <summary>
        /// Displays each ship with their associated number and take up of spaces.
        /// Displays as grey if the ship has been updated
        /// </summary>
        /// <param name="shipList"></param>
        static void DisplayShipsForPlacement(List<Battleship> shipList)
        {
            for (int index = 0; index < shipList.Count; index++)            // going through each ship in the list
            {
                if (shipList[index].EachIndexOnOceanGrid.Count > 0)         // if the ship has already been placed onto the grid
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(index + 1 + ") " + shipList[index].name + " = " + shipList[index].ShipLength + " spaces");
                }
                else
                {
                    Console.WriteLine(index + 1 + ") " + shipList[index].name + " = " + shipList[index].ShipLength + " spaces");
                }
                Console.ResetColor();
            }
        }
        /// <summary>
        /// Displays the valid directions. 
        /// </summary>
        static void DisplayDirections()
        {
            int directionListIndex = 0;
            Console.WriteLine("Placement Directions: ");
            foreach (DirectionList direction in Enum.GetValues(typeof(DirectionList)))
            {
                if (direction == DirectionList.Invalid)
                {
                    continue;
                }
                else
                {
                    directionListIndex++;
                    Console.WriteLine(directionListIndex + ") " + direction);
                }
            }
        }

        /// <summary>
        /// The entire encapsulation of a player turn. Display player grids, Display shot message (if there is one), get player coordinates,
        /// Shoot at player coordinates and create shotMessage, Display modified player grids and shot message
        /// </summary>
        /// <param name="currentPlayer">The current player that is playing. Only HumanPlayers can make a turn</param>
        /// <param name="opponentPlayer">The opposing player. Can either be HumanPlayer or CPUPlayer without issue</param>
        /// <param name="shotMessage">The current shot message</param>
        /// <returns>The shotMessage of the current player.</returns>
        public static string PlayerTurn(HumanPlayer currentPlayer, BasePlayer opponentPlayer, string shotMessage)
        {
            BaseGrid.DisplayPlayerGrids(currentPlayer);
            DisplayShotTakenMessage(opponentPlayer, shotMessage, ConsoleColor.Red);   // Displays the shot message of the previous previous player shot

            int[] playerCoordinates = PlayerInput.ReturnValidUserCoordinates(currentPlayer);                             // askss for [y,x] coordinates from player
            shotMessage = TargetGrid.PlaceShotsOnTargetGrid(currentPlayer, opponentPlayer, playerCoordinates[0], playerCoordinates[1]); // shoots, creates a message from shot, clear console
            BattleshipGame.FullyClearConsole();

            BaseGrid.DisplayPlayerGrids(currentPlayer);
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
        static void DisplayShotTakenMessage(BasePlayer displayPlayer, string shotMessage, ConsoleColor color)
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

        /// <summary>
        /// Creates the ocean grid colors 
        /// </summary>
        /// <returns>A dictionary of colors as referenced by an associated character</returns>
        static Dictionary<char, ConsoleColor> CreateOceanGridColors()
        {
            BasePlayer tempPlayer = new BasePlayer();
            Dictionary<char, ConsoleColor> colorDictionary = new Dictionary<char, ConsoleColor>()
            {
                { '~', ConsoleColor.DarkBlue },
                { tempPlayer.targetMissDisplay, ConsoleColor.White },
                { tempPlayer.targetSunkDisplay, ConsoleColor.DarkRed }
            };

            foreach (Battleship ship in tempPlayer.shipList)                    // Adds colors of each ship state
            {
                colorDictionary.Add(ship.DisplayNuetral, ConsoleColor.Green);
                colorDictionary.Add(ship.DisplayWhenHit, ConsoleColor.Red);
            }

            return colorDictionary;
        }

        /// <summary>
        /// Creates the target grid colors. Does not referece ship displays for hits
        /// </summary>
        /// <returns>Creates a dictionary of colors as referenced by an associated character.</returns>
        static Dictionary<char, ConsoleColor> CreateTargetGridColors()
        {
            BasePlayer tempPlayer = new BasePlayer();
            Dictionary<char, ConsoleColor> colorDictionary = new Dictionary<char, ConsoleColor>()
            {
                { '~', ConsoleColor.DarkBlue },
                { tempPlayer.targetHitDisplay, ConsoleColor.Red },
                { tempPlayer.targetMissDisplay, ConsoleColor.White },
                { tempPlayer.targetSunkDisplay, ConsoleColor.DarkRed }
            };

            return colorDictionary;
        }
    }
}
