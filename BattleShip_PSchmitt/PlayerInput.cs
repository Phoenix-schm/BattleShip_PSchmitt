namespace BattleShip_PSchmitt
{
    class PlayerInput
    {
        /// <summary>
        /// Used on the Main Menu. Checks if the player input is one of the main menu choices
        /// </summary>
        /// <returns></returns>
        public static string CheckMainMenuChoice(string[] mainMenuChoices)
        {
            bool isValidChoice = false;
            string choice = "Invalid";

            while (!isValidChoice)
            {
                string? playerInput = Console.ReadLine();
                if (playerInput != null && playerInput != "")
                {
                    for (int index = 0; index < mainMenuChoices.Length; index++)
                    {
                        choice = mainMenuChoices[index];

                        if (choice == "Invalid")    // Invalid is 0
                        {
                            continue;
                        }
                        else if (playerInput.ToLower() == choice.ToLower() || playerInput == index.ToString())        // If playerinput is one of the main menu
                        {                                                                                             //    string or ints
                            isValidChoice = true;
                            break;
                        }
                    }
                    if (!isValidChoice)
                    {
                        InvalidMessage("That is not a valid menu choice.");
                    }
                }
                else
                {
                    InvalidMessage("Cannot input nothing.");
                }
            }
            return choice.ToLower();
        }

        /// <summary>
        /// Prompts the player for a ship
        /// </summary>
        /// <param name="player">The player placing the ship.</param>
        /// <returns>Battleship that is being placed onto the grid.</returns>
        public static Battleship ChooseShipToPlace(BasePlayer player)
        {
            bool isValidShip = false;
            Battleship chosenShip = null;
            while (!isValidShip)                                                // Should always break when the ship is no longer null.
            {
                Console.Write("Choose a ship to place: ");
                string? playerInput = Console.ReadLine();

                if (playerInput != null && playerInput != "")
                {
                    for (int index = 1; index < player.shipList.Count + 1; index++)              // Going through the list of ships
                    {
                        if (playerInput.ToLower() == player.shipList[index - 1].name.ToLower() || playerInput == index.ToString())  // if player picked the correct a listed ship
                        {
                            isValidShip = true;
                            chosenShip = player.shipList[index - 1];                             // minus 1 index for off-by-one
                            break;
                        }
                    }

                    if (isValidShip)
                    {
                        if (chosenShip.EachIndexOnOceanGrid.Count > 0)                           // if the ship has been chosen previously
                        {
                            isValidShip = false;
                            InvalidMessage("You have already chosen that ship.");
                        }
                    }

                    if (chosenShip == null)
                    {
                        InvalidMessage("That is not a ship you can choose.");
                    }
                }
                else
                {
                    InvalidMessage("Cannot input nothing.");
                }
            }
            return chosenShip;
        }

        /// <summary>
        /// Checks that the player is inputting a possible number on the grid
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static int CheckInputNumIsOnGrid(string message)
        {
            bool isValidNumber = false;
            int ValidNumber = -1;
            while (!isValidNumber)
            {
                Console.Write(message + ": ");
                string? playerInput = Console.ReadLine();

                if (playerInput != null && playerInput != string.Empty)
                {
                    for (int index = 1; index <= 10; index++)           // going through the numbers 1 - 10
                    {
                        if (playerInput[0] == '0')                      // get rid of the zero, if the player inputs it as the first number
                        {
                            playerInput = playerInput.Replace('0', ' ').Trim();
                        }
                        if (playerInput == index.ToString())
                        {
                            isValidNumber = true;
                            ValidNumber = index - 1;                    // Minus one to deal with off by one
                            break;
                        }
                    }
                    if (!isValidNumber)
                    {
                        InvalidMessage("That is not a coordinate on the grid.");
                    }
                }
                else
                {
                    InvalidMessage("Cannot input nothing.");
                }
            }
            return ValidNumber;
        }

        /// <summary>
        /// Checks if player input is a valid direction
        /// </summary>
        /// <returns></returns>
        public static BasePlayer.DirectionList ChooseDirectionToPlaceShip()
        {
            bool isValidDirection = false;
            BasePlayer.DirectionList chosenDirection = BasePlayer.DirectionList.Invalid;

            while (!isValidDirection)
            {
                Console.Write("Choose a direction to place the ship: ");
                string? playerInput = Console.ReadLine();
                if (playerInput != null)
                {
                    foreach (BasePlayer.DirectionList direction in Enum.GetValues(typeof(BasePlayer.DirectionList)))
                    {
                        string directionString = direction.ToString();
                        int directionIndex = (int)direction;
                        if (directionString == "Invalid")
                        {
                            continue;
                        }
                        else if (playerInput.ToLower() == directionString.ToLower())
                        {
                            isValidDirection = true;
                            chosenDirection = direction;
                            break;
                        }
                        else if (playerInput == directionIndex.ToString())
                        {
                            isValidDirection = true;
                            chosenDirection = direction;
                            break;
                        }
                    }
                    if (!isValidDirection)
                    {
                        InvalidMessage("That was not a valid direction.");
                    }
                }
                else
                {
                    InvalidMessage("Cannot input nothing.");
                }
            }
            return chosenDirection;
        }

        /// <summary>
        /// Asks the currentPlayer to give two coordinates.
        /// If the index at those coordinates has already been shot at, then redo.
        /// </summary>
        /// <param name="currentPlayer">The current player being asked for coordinates.</param>
        /// <returns>Returns valid coordinates the player can shoot at.</returns>
        public static int[] ReturnValidUserCoordinates(BasePlayer currentPlayer)
        {
            bool isValidCoordinates = false;
            int y_axis = -1;
            int x_axis = -1;
            while (!isValidCoordinates)
            {
                y_axis = CheckInputNumIsOnGrid("Choose a y coordinate to shoot");
                x_axis = CheckInputNumIsOnGrid("Choose an x coordinate to shoot");
                char charAtIndex = currentPlayer.targetGrid[y_axis, x_axis];

                if (charAtIndex != '~')
                {
                    InvalidMessage("You've already hit that coordinate.");
                }
                else
                {
                    isValidCoordinates = true;
                }
            }
            return [y_axis, x_axis];
        }

        /// <summary>
        /// Checks if the player wishes to undo a chosenShip placement on the player Ocean Grid.
        /// </summary>
        /// <param name="chosenShip">The ship potentially being undone</param>
        /// <param name="totalShipIndex">Adjusts if the player wishes to undo a ship.</param>
        /// <param name="playerOceanGrid">The grid being updated.</param>
        /// <param name="direction">Adjusts the coordinate index based on vertical or horizontal</param>
        /// <returns></returns>
        public static char[,] CheckRedoGridInput(ref Battleship chosenShip, ref int totalShipIndex, char[,] playerOceanGrid)
        {
            Console.WriteLine("You have placed " + chosenShip.name + " onto the grid.");
            Console.WriteLine("Are you happy with this placement?");
            Console.WriteLine("1) Yes");
            Console.WriteLine("2) No");

            bool isValidChoice = false;
            string? userInput;
            while (!isValidChoice)
            {
                userInput = Console.ReadLine();
                if (userInput != null && userInput != "")
                {
                    if (userInput.ToLower() == "no" || userInput == "2")
                    {
                        isValidChoice = true;
                        totalShipIndex--;
                        for (int index = chosenShip.EachIndexOnOceanGrid.Count - 1; index >= 0; index--)    // remove each index space from player grid
                        {
                            int[] shipSpaces = chosenShip.EachIndexOnOceanGrid[index];
                            int y = shipSpaces[0];
                            int x = shipSpaces[1];

                            playerOceanGrid[y, x] = '~';
                        }
                        chosenShip.EachIndexOnOceanGrid.Clear();                                            // Clear out the chosenShip indexes
                    }
                    else if (userInput.ToLower() == "yes" || userInput == "1")
                    {
                        isValidChoice = true;
                    }
                    else
                    {
                        InvalidMessage("That wasn't a valid input.");
                    }
                }
                else
                {
                    InvalidMessage("Cannot input nothing.");
                }
            }
            BattleshipGame.FullyClearConsole();
            return playerOceanGrid;
        }

        /// <summary>
        /// Asks the player if they'd like to play again.
        /// </summary>
        /// <returns>Returns true if they want to redo. Returns false if they want to quit.</returns>
        public static bool PlayAgainInput()
        {
            Console.WriteLine("Would you like to try again?");
            Console.WriteLine("1) Yes");
            Console.WriteLine("2) No");

            bool isValidInput = false;
            string? userInput;
            bool outputValue = false;

            while (!isValidInput)
            {
                userInput = Console.ReadLine();
                if (userInput != null && userInput != "")
                {
                    if (userInput.ToLower() == "yes" || userInput == "1")
                    {
                        isValidInput = true;
                        outputValue = true;
                    }
                    else if (userInput.ToLower() == "no" || userInput == "2")
                    {
                        isValidInput = true;
                        outputValue = false;
                    }
                    else
                    {
                        InvalidMessage("That wasn't a valid input");
                    }
                }
                else
                {
                    InvalidMessage("Cannot input nothing");
                }
            }
            return outputValue;
        }

        /// <summary>
        /// Choice of player that goes first.
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        public static BasePlayer[] ChooseWhoGoesFirstInput(BasePlayer player1, BasePlayer player2)
        {
            bool isValidName = false;
            BasePlayer[]? playerOrder = null;
            string? userinput;

            while (!isValidName)
            {
                userinput = Console.ReadLine();
                if (userinput != null && userinput != "")
                {
                    if (userinput.ToLower() == player1.name.ToLower())
                    {
                        isValidName = true;
                        playerOrder = [player1, player2];
                    }
                    else if (userinput.ToLower() == player2.name.ToLower())
                    {
                        isValidName = true;
                        playerOrder = [player2, player1];
                    }
                    else
                    {
                        InvalidMessage("That wasn't a valid player name.");
                    }
                }
                else
                {
                    InvalidMessage("Cannot input nothing.");
                }
            }

            return playerOrder;
        }

        static void InvalidMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
