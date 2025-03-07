using System;
using System.Collections.Generic;
namespace BattleShip_PSchmitt
{
    class Player
    {
        public char[,] playerOceanGrid;
        public char[,] playerTargetGrid;
        public List<Battleship> playerShipList;
        public string name = "";
        public bool IsAlive
        {
            get
            {
                return playerShipList.Count > 0;
            }
        }

        public Player()
        {
            playerShipList = CreateShips();                        // Default number of ships. Will act as health
            playerOceanGrid = GameGrid.CreateDefaultGrid();        // Deafult ocean grid. Will contain ships
            playerTargetGrid = GameGrid.CreateDefaultGrid();       // Default target grid. Will show shots taken
        }
        public Player(string playerName)
        {
            playerShipList = CreateShips();                        // Default number of ships. Will act as health
            playerOceanGrid = GameGrid.CreateDefaultGrid();        // Deafult ocean grid. Will contain ships
            playerTargetGrid = GameGrid.CreateDefaultGrid();       // Default target grid. Will show shots taken
            name = playerName;
        }

        /// <summary>
        /// Creation of each battleship
        /// </summary>
        /// <returns>An array of each batteship: amount of int spaces it takes up, an empty List of spaces it takes on a grid,
        /// and its string name</returns>
        public List<Battleship> CreateShips()
        {
            Battleship destroyerShip = new Battleship(2, spaces: [], "Destroyer", 'd');
            Battleship submarineShip = new Battleship(3, spaces: [], "Submarine", 's');
            Battleship cruiserShip = new Battleship(3, spaces: [], "Cruiser", 'c');
            Battleship battleShip = new Battleship(4, spaces: [], "Battleship", 'B');
            Battleship carrierShip = new Battleship(5, spaces: [], "Carrier", 'C');

            return [destroyerShip, submarineShip, cruiserShip, battleShip, carrierShip];
        }


        /// <summary>
        /// Used on the Main Menu. Checks if the player input is one of the main menu choices
        /// </summary>
        /// <returns></returns>
        public static string CheckMainMenuChoice()
        {
            bool isValidInput = false;
            string choiceString = "Invalid";

            while (!isValidInput)
            {
                string? playerInput = Console.ReadLine();
                if (playerInput != null && playerInput != "")
                {
                    foreach (BattleshipGame.MainMenuChoices choice in Enum.GetValues(typeof(BattleshipGame.MainMenuChoices)))   // Going through each Main Menu choice
                    {
                        choiceString = choice.ToString().Replace("_", " ");
                        int choiceInt = (int)choice;

                        if (choiceString == "Invalid")
                        {
                            continue;
                        }
                        else if (playerInput.ToLower() == choiceString.ToLower() || playerInput == choiceInt.ToString())        // If playerinput is one of the main menu
                        {                                                                                                       //      string or ints
                            isValidInput = true;
                            break;
                        }
                    }
                    if (!isValidInput)
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
            bool isValid = false;
            Battleship chosenShip = null;
            while (!isValid)                                                // Should always break when the ship is no longer null.
            {
                Console.Write("Choose a ship to place: ");
                string? playerInput = Console.ReadLine();

                if (playerInput != null && playerInput != "")
                {
                    for (int index = 1; index < player.playerShipList.Count + 1; index++)              // Going through the list of ships
                    {
                        if (playerInput.ToLower() == player.playerShipList[index - 1].name.ToLower() || playerInput == index.ToString())  // if player picked the correct a listed ship
                        {
                            isValid = true;
                            chosenShip = player.playerShipList[index - 1];                             // minus 1 index for off-by-one
                            break;
                        }
                    }

                    if (isValid)
                    {
                        if (chosenShip.EachIndexSpace.Count > 0)                            // if the ship has been chosen previously
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            isValid = false;
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
            bool isInputValid = false;
            int inputIndex = -1;
            while (!isInputValid)
            {
                Console.Write(message + ": ");
                string? userInput = Console.ReadLine();

                if (userInput != null && userInput != string.Empty)
                {
                    for (int index = 1; index <= 10; index++)           // going through the numbers 1 - 10
                    {
                        if (userInput == index.ToString())
                        {
                            isInputValid = true;
                            inputIndex = index;
                            break;
                        }
                    }
                    if (!isInputValid)
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
            return inputIndex - 1;                                      // minus one to deal with off-by-one
        }

        /// <summary>
        /// checks if player input is a valid direction
        /// </summary>
        /// <param name="directionsList"></param>
        /// <returns></returns>
        public static int ChooseDirectionToPlaceShip(string[] directionsList)
        {
            bool isValid = false;
            string? userInput = "";
            int chosenDirection = -1;
            while (!isValid)
            {
                Console.Write("Choose a direction to place the ship: ");
                userInput = Console.ReadLine();
                if (userInput != null)
                {
                    for (int index = 1; index < directionsList.Length + 1; index++)
                    {
                        if (userInput.ToLower() == directionsList[index - 1].ToLower())
                        {
                            isValid = true;
                            chosenDirection = index - 1;
                            break;
                        }
                        else if (userInput == index.ToString())
                        {
                            isValid = true;
                            chosenDirection = index - 1;
                            break;
                        }
                    }
                    if (!isValid)
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
        /// <param name="index"></param>
        /// <param name="playerGrid"></param>
        /// <returns></returns>
        public static char[,] CheckRedoGrid(ref Battleship chosenShip, ref int index, char[,] playerGrid, int direction)
        {
            bool isValid = false;
            string? userInput = "";
            Console.WriteLine("You have placed " + chosenShip.name + " onto the grid.");
            Console.WriteLine("Are you happy with this placement?");
            Console.WriteLine("1) Yes");
            Console.WriteLine("2) No");

            while (!isValid)
            {
                userInput = Console.ReadLine();

                if (userInput.ToLower() == "no" || userInput == "2")
                {
                    isValid = true;
                    index--;
                    for (int i = chosenShip.EachIndexSpace.Count - 1; i >= 0; i--)
                    {
                        int[] shipSpaces = chosenShip.EachIndexSpace[i];
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
                    isValid = true;
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
