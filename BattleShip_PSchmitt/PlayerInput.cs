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
            string choiceString = "Invalid";

            while (!isValidChoice)
            {
                string? playerInput = Console.ReadLine();
                if (playerInput != null && playerInput != "")
                {
                    for (int index = 0;  index < mainMenuChoices.Length; index++)
                    {
                        choiceString = mainMenuChoices[index].ToString().Replace("_", " ");

                        if (choiceString == "Invalid")
                        {
                            continue;
                        }
                        else if (playerInput.ToLower() == choiceString.ToLower() || playerInput == index.ToString())        // If playerinput is one of the main menu
                        {                                                                                                       //      string or ints
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
            return choiceString.ToLower();
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
                    for (int index = 1; index < player.playerShipList.Count + 1; index++)              // Going through the list of ships
                    {
                        if (playerInput.ToLower() == player.playerShipList[index - 1].name.ToLower() || playerInput == index.ToString())  // if player picked the correct a listed ship
                        {
                            isValidShip = true;
                            chosenShip = player.playerShipList[index - 1];                             // minus 1 index for off-by-one
                            break;
                        }
                    }

                    if (isValidShip)
                    {
                        if (chosenShip.EachIndexSpace.Count > 0)                            // if the ship has been chosen previously
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
        /// <param name="directionsList"></param>
        /// <returns></returns>
        public static int ChooseDirectionToPlaceShip(string[] directionsList)
        {
            bool isValidDirection = false;
            string? playerInput = "";
            int chosenDirection = -1;

            while (!isValidDirection)
            {
                Console.Write("Choose a direction to place the ship: ");
                playerInput = Console.ReadLine();
                if (playerInput != null)
                {
                    for (int index = 1; index < directionsList.Length + 1; index++)
                    {
                        if (playerInput.ToLower() == directionsList[index - 1].ToLower() || playerInput == index.ToString()) // if playerInput = direction string or number
                        {
                            isValidDirection = true;
                            chosenDirection = index - 1;
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
        /// Checks if the player wishes to undo a chosenShip placement on the board.
        /// </summary>
        /// <param name="chosenShip"></param>
        /// <param name="totalShipIndex">Adjusts whether there is another ship to be added to the board.</param>
        /// <param name="playerGrid"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static char[,] CheckRedoGrid(ref Battleship chosenShip, ref int totalShipIndex, char[,] playerGrid, int direction)
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
                    for (int index = chosenShip.EachIndexSpace.Count - 1; index >= 0; index--)
                    {
                        int[] shipSpaces = chosenShip.EachIndexSpace[index];
                        if (direction == 0 || direction == 1)
                        {
                            Array.Reverse(shipSpaces);
                        }

                        int y = shipSpaces[0];
                        int x = shipSpaces[1];
                        playerGrid[y, x] = '~';
                    }
                    chosenShip.EachIndexSpace.Clear();
                    Console.Clear();
                }
                else if (userInput.ToLower() == "yes" || userInput == "1")
                {
                    isValidChoice = true;
                    Console.Clear();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("That wasn't a valid input");
                    Console.ResetColor();
                }
            }
            return playerGrid;
        }
    }
}
