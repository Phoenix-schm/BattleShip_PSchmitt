using System;
using System.Collections.Generic;
namespace BattleShip_PSchmitt
{
    class Player : GameGrid
    {
        public char[,] _playerOceanGrid;
        public char[,] _playerTargetGrid;
        public List<Battleship> _playerShipList;
        public Player()
        {
            _playerShipList = CreateShips();                // Default number of ships. Will act as health
            _playerOceanGrid = CreateDefaultGrid();         // Deafult ocean grid. Will contain ships
            _playerTargetGrid = CreateDefaultGrid();        // Default target grid. Will show shots taken
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
            while (!isValid)
            {
                Console.WriteLine("Which ship qould you like to place?");
                string? playerInput = Console.ReadLine();

                if (playerInput != null && playerInput != "")
                {
                    foreach (Battleship ship in player._playerShipList)
                    {
                        if (playerInput.ToLower() == ship.name.ToLower())
                        {
                            isValid = true;
                            chosenShip = ship;
                            break;
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

        public static List<Battleship> RemoveShipFromList(List<Battleship> shipList, Battleship chosenShip)
        {
            shipList.Remove(chosenShip);
            return shipList;
        }
    }
}
