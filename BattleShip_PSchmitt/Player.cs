using System;
using System.Collections.Generic;
namespace BattleShip_PSchmitt
{
    class Player : GameGrid
    {
        public char[,] _playerOceanGrid;
        public char[,] _playerTargetGrid;
        public Battleship[] _playerShipList;
        public Player()
        {
            _playerShipList = new Battleship[5];            // Default number of ships. Will act as health
            _playerOceanGrid = CreateDefaultGrid();         // Deafult ocean grid, will contain ships
            _playerTargetGrid = CreateDefaultGrid();        // Default target grid, will show shots 
        }
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
    }
}
