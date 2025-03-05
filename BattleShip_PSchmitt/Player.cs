using System;
using System.Collections.Generic;
namespace BattleShip_PSchmitt
{
    class Player : GameGrid
    {
        public char[,] _playerOceanGrid;
        public char[,] _playerTargetGrid;
        public List<Battleship> _playerShipList;
        public string _name = "";
        public Player()
        {
            _playerShipList = CreateShips();                // Default number of ships. Will act as health
            _playerOceanGrid = CreateDefaultGrid();         // Deafult ocean grid. Will contain ships
            _playerTargetGrid = CreateDefaultGrid();        // Default target grid. Will show shots taken
        }
        public Player(string name)
        {
            _playerShipList = CreateShips();                // Default number of ships. Will act as health
            _playerOceanGrid = CreateDefaultGrid();         // Deafult ocean grid. Will contain ships
            _playerTargetGrid = CreateDefaultGrid();        // Default target grid. Will show shots taken
            _name = name;
        }

        /// <summary>
        /// Creation of each battleship
        /// </summary>
        /// <returns>An array of each batteship: amount of int spaces it takes up, an empty List of spaces it takes on a grid,
        /// and its string name</returns>
        public List<Battleship> CreateShips()
        {
            Battleship destroyerShip = new Battleship(2, spaces: [], "Destroyer", 'D');
            Battleship submarineShip = new Battleship(3, spaces: [], "Submarine", 'C');
            Battleship cruiserShip = new Battleship(3, spaces: [], "Cruiser", 'B');
            Battleship battleShip = new Battleship(4, spaces: [], "Battleship", 'A');
            Battleship carrierShip = new Battleship(5, spaces: [], "Carrier", 'S');

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
                    foreach (BattleshipGame.MainMenuChoices choice in Enum.GetValues(typeof(BattleshipGame.MainMenuChoices)))
                    {
                        choiceString = choice.ToString().Replace("_", " ");
                        int choiceInt = (int)choice;

                        if (choiceString == "Invalid")
                        {
                            continue;
                        }
                        else if (playerInput.ToLower() == choiceString.ToLower())
                        {
                            isValidInput = true;
                            break;
                        }
                        else if (playerInput == choiceInt.ToString())
                        {
                            isValidInput = true;
                            break;
                        }
                    }
                    if (!isValidInput)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("That was not a valid input");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Cannot input nothing");
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
                    for (int index = 1; index < player._playerShipList.Count + 1; index++)              // Going through the list of ships
                    {
                        if (playerInput.ToLower() == player._playerShipList[index - 1].name.ToLower())  // if player picked the correct string name
                        {
                            isValid = true;
                            chosenShip = player._playerShipList[index - 1];                             // minus 1 index for off-by-one
                            break;
                        }
                        else if (playerInput == index.ToString())                                       // if the player picked the correct index
                        {
                            isValid = true;
                            chosenShip = player._playerShipList[index - 1];                             // minus 1 index for obo
                        }
                    }

                    if (isValid)
                    {
                        if (chosenShip.eachIndexSpace.Count > 0)                    // if the ship has been chosen previously
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
        public static char[,] CheckRedoGrid(ref Battleship chosenShip, ref int index, char[,] playerGrid)
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
                    for (int i = chosenShip.eachIndexSpace.Count - 1; i >= 0; i--)
                    {
                        int[] shipSpaces = chosenShip.eachIndexSpace[i];
                        int y = shipSpaces[0];
                        int x = shipSpaces[1];
                        playerGrid[y, x] = '~';
                    }
                    chosenShip.eachIndexSpace.Clear();
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

        public static int[] RandomNumberList(int maxIndexOfList, int maxNumberInList, Random rand)
        {
            int[] randomNumList = new int[maxIndexOfList];
            int index = 0;
            int useZeroOnce = 0;

            while (index < randomNumList.Length)
            {
                int newNum = rand.Next(0, maxNumberInList);
                if (newNum == 0 && useZeroOnce == 0)
                {
                    randomNumList[index] = newNum;
                    useZeroOnce++;
                    index++;
                }
                else if (randomNumList.Contains(newNum))
                {
                    continue;
                }
                else
                {
                    randomNumList[index] = newNum;
                    index++;
                }
            }
            return randomNumList;
        }

        public static List<Battleship> RemoveShipFromList(List<Battleship> shipList, Battleship chosenShip)
        {
            shipList.Remove(chosenShip);
            return shipList;
        }
    }
}
