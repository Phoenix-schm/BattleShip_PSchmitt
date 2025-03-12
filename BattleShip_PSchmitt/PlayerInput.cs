namespace BattleShip_PSchmitt
{
    internal class PlayerInput
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

                        if (choice == "Invalid")
                        {
                            continue;
                        }
                        else if (playerInput.ToLower() == choice.ToLower() || playerInput == index.ToString())        // If playerinput is one of the main menu
                        {                                                                                                   //      string or ints
                            isValidChoice = true;
                            break;
                        }
                    }
                    if (!isValidChoice)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("That was not a valid menu choice.");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Cannot input nothing.");
                }
                Console.ResetColor();
            }
            return choice.ToLower();
        }

        /// <summary>
        /// Prompts the player for a ship
        /// </summary>
        /// <param name="player">The player placing the ship.</param>
        /// <returns>Battleship that is being placed onto the grid.</returns>
        public static Battleship ChooseShipToPlace(Player player)
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
                        if (chosenShip.EachIndexOnOceanGrid.Count > 0)                            // if the ship has been chosen previously
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            isValidShip = false;
                            Console.WriteLine("You've already picked that ship before.");
                        }
                    }

                    if (chosenShip == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("That isn't a ship you can choose.");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Cannot input nothing");
                }
                Console.ResetColor();
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
                        if (playerInput == index.ToString())
                        {
                            isValidNumber = true;
                            ValidNumber = index - 1;                    // Minus one to deal with off by one
                            break;
                        }
                    }
                    if (!isValidNumber)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Those are not valid coordinates");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Cannot input nothing");
                }
                Console.ResetColor();
            }
            return ValidNumber;
        }

        /// <summary>
        /// checks if player input is a valid direction
        /// </summary>
        /// <param name="directionList"></param>
        /// <returns></returns>
        public static Player.Directions ChooseDirectionToPlaceShip()
        {
            bool isValidDirection = false;
            Player.Directions chosenDirection = Player.Directions.Invalid;

            while (!isValidDirection)
            {
                Console.Write("Choose a direction to place the ship: ");
                string? playerInput = Console.ReadLine();
                if (playerInput != null)
                {
                    foreach (Player.Directions direction in Enum.GetValues(typeof(Player.Directions)))
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
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid direction.");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Cannot input nothing.");
                }
                Console.ResetColor();
            }
            return chosenDirection;
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
            bool isValidChoice = false;
            string? userInput = "";
            Console.WriteLine("You have placed " + chosenShip.name + " onto the grid.");
            Console.WriteLine("Are you happy with this placement?");
            Console.WriteLine("1) Yes");
            Console.WriteLine("2) No");

            while (!isValidChoice)
            {
                userInput = Console.ReadLine();

                if (userInput.ToLower() == "no" || userInput == "2")
                {
                    isValidChoice = true;
                    totalShipIndex--;
                    for (int index = chosenShip.EachIndexOnOceanGrid.Count - 1; index >= 0; index--)
                    {
                        int[] shipSpaces = chosenShip.EachIndexOnOceanGrid[index];

                        int y = shipSpaces[0];
                        int x = shipSpaces[1];
                        playerOceanGrid[y, x] = '~';
                    }
                    chosenShip.EachIndexOnOceanGrid.Clear();
                    Console.Clear();
                    Console.WriteLine("\x1b[3J");                   // Fully clears the console.
                }
                else if (userInput.ToLower() == "yes" || userInput == "1")
                {
                    isValidChoice = true;
                    Console.Clear();
                    Console.WriteLine("\x1b[3J");                   // Fully clears the console.
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("That wasn't a valid input");
                    Console.ResetColor();
                }
            }
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
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("That is not a valid input.");
                    Console.ResetColor();
                }
            }
            return outputValue;
        }

        /// <summary>
        /// Choice of player that goes first.
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        public static void ChooseWhoGoesFirstInput(Player player1, Player player2)
        {
            bool isValidName = false;
            string? userinput;

            while (!isValidName)
            {
                userinput = Console.ReadLine();
                if (userinput != null)
                {
                    if (userinput.ToLower() == player1.name.ToLower())
                    {
                        isValidName = true;
                        player1.goesFirst = 1;
                    }
                    else if (userinput.ToLower() == player2.name.ToLower())
                    {
                        isValidName = true;
                        player2.goesFirst = 1;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("That is not a player name.");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Cannot input nothing.");
                }
                Console.ResetColor();
            }
        }

        //public static void GiveYourselfAName(Player player)
        //{
        //    Console.WriteLine("")
        //}
    }
}
