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
        /// Used on the Main Menu. Checks if the player input is one of the main menu choices
        /// </summary>
        /// <returns></returns>
        public static string CheckMainMenuChoice()
        {
            bool isValidInput = false;
            string choiceString = "Invalid";

            while(!isValidInput)
            {
                string? playerInput = Console.ReadLine();
                if (playerInput != null && playerInput != "")
                {
                    foreach(BattleshipGame.MainMenuChoices choice in Enum.GetValues(typeof(BattleshipGame.MainMenuChoices)))
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
                }
                else
                {
                    Console.WriteLine("Cannot input nothing");
                }
            }
            return choiceString.ToLower();
        }

        public static Battleship ChooseShipToPlace(Player player)
        {
            bool isValid = false;
            Battleship chosenShip = null;
            while (!isValid)                                                // Should always break when the ship is no longer null.
            {
                Console.WriteLine("Which ship qould you like to place?");
                string? playerInput = Console.ReadLine();

                if (playerInput != null && playerInput != "")
                {
                    for (int index = 1; index < player._playerShipList.Count + 1; index++)
                    {
                        if (playerInput.ToLower() == player._playerShipList[index - 1].name.ToLower())
                        {
                            isValid = true;
                            chosenShip = player._playerShipList[index - 1];
                            break;
                        }
                        else if (playerInput == index.ToString())
                        {
                            isValid = true;
                            chosenShip = player._playerShipList[index - 1];
                        }
                    }

                    if (isValid)
                    {
                        if (chosenShip.eachIndexSpace.Count > 0)                    // if the ship has been chosen previously
                        {
                            isValid = false;
                            Console.WriteLine("You've already picked that ship before.");
                        }
                    }

                    if (chosenShip == null)
                    {
                        Console.WriteLine("That isn't a ship you can choose.");
                    }
                }
                else
                {
                    Console.WriteLine("Cannot input nothing");
                }
            }
            return chosenShip;
        }

        public static int CheckInputAxisIsValid(string message)
        {
            bool isInputValid = false;
            int inputIndex = -1;
            while (!isInputValid)
            {
                Console.Write(message + ": ");
                string? userInput = Console.ReadLine(); 
                for (int index = 1; index <= 10; index++)
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
                    Console.WriteLine("Those are invalid coordinates");
                }
            }
            return inputIndex - 1;
        }

        public static int ChooseDirectionToPlaceShip(string[] directionsList)
        {
            bool isValid = false;
            string? userInput = "";
            int chosenDirection = -1;
            while (!isValid)
            {
                Console.WriteLine("Directions to place: ");
                for (int i = 0; i < directionsList.Length; i++)
                {
                    Console.WriteLine((i + 1) + ") " + directionsList[i]);
                }
                Console.WriteLine();
                Console.Write("Choose a direction to place the ship: ");
                userInput = Console.ReadLine();
                if (userInput != null)
                {
                    for (int index = 1;  index < directionsList.Length + 1; index++)
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
                }
            }
            return chosenDirection;
        }

        public static List<Battleship> RemoveShipFromList(List<Battleship> shipList, Battleship chosenShip)
        {
            shipList.Remove(chosenShip);
            return shipList;
        }
    }
}
