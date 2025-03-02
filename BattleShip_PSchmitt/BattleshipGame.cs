namespace BattleShip_PSchmitt
{
    class BattleshipGame
    {
        public enum MainMenuChoices
        {
            Invalid,
            Player_Vs_CPU,
            Player_Vs_Player,
            Quit
        }
        static void Main(string[] args)
        {
            string[] titleScreen =
            {
                " ____        _   _   _           _     _       ",
                "| __ )  __ _| |_| |_| | ___  ___| |__ (_)_ __  ",
                "|  _ \\ / _` | __| __| |/ _ \\/ __| '_ \\| | '_ \\ ",
                "| |_) | (_| | |_| |_| |  __/\\__ \\ | | | | |_) |",
                "|____/ \\__,_|\\__|\\__|_|\\___||___/_| |_|_| .__/ ",
                "                                        |_|    \r\n",
            };

            //foreach (string row in titleScreen)
            //{
            //    Console.WriteLine(row);
            //}
            //Console.WriteLine("Press any key to start...");
            //Console.ReadKey();
            //Console.Clear();

            //start game here
            bool isPlaying = true;
            while (isPlaying)
            { 
                Console.WriteLine("Choose your game:");
                DisplayMainMenuChoices();
                string playerInput = Player.CheckMainMenuChoice();

                switch(playerInput)                                         // Player choice of which to play
                {
                    case "quit":
                        isPlaying = false; 
                        break;
                    case "player vs cpu":
                        PlayPlayerVsCPU();
                        break;
                    case "player vs player":
                        break;
                    default:
                        Console.WriteLine("This shouldn't be accessible.");
                        break;
                }
            }
        }
        static void DisplayMainMenuChoices()
        {
            foreach(MainMenuChoices choice in Enum.GetValues(typeof(MainMenuChoices)))
            {
                string choiceString = choice.ToString().Replace("_", " ");
                if (choiceString == "Invalid")
                {
                    continue;
                }
                else
                {
                    Console.WriteLine((int)choice + ") " + choiceString);
                }
            }
        }

        /// <summary>
        /// Running through the game of player vs computer. First to sink all battleships wins
        /// </summary>
        static void PlayPlayerVsCPU()
        {
            Player player = new Player();
            Player cpu = new Player();

            Console.WriteLine("Howdy Player! Time to make your grid");
            player._playerOceanGrid = CreateOceanGrid(player, player._playerOceanGrid, player._playerShipList);

            player._playerShipList = player.CreateShips();

            player.DisplayOceanGrid(player._playerOceanGrid);

        }

        /// <summary>
        /// Displays the list of ships
        /// </summary>
        //static void DisplayShipList(Battleship[] shipList)
        //{
        //    Console.WriteLine("Your choice of battleships: ");
        //    foreach(Battleship ship in shipList)
        //    {
        //        Console.WriteLine((int)ship + ") " + ship.ToString());
        //    }
        //}

        static char[,] CreateOceanGrid(Player player, char[,] currentOceanGrid, List<Battleship> shipList)
        {
            string[] directionalPlacement = { "Vertical Up", "Vertical Down", "Horizontal Left", "Horizontal Right" };

            bool isValid = false;
            for (int i = 0; i < shipList.Count; i++)
            {
                do
                {
                    Console.WriteLine("Current Player Grid:");
                    player.DisplayOceanGrid(currentOceanGrid);

                    Console.WriteLine("You have " + shipList.Count + " ships left to place.");
                    foreach (Battleship displayShip in shipList)
                    {
                        Console.WriteLine($"{displayShip.name}");
                    }
                    Console.WriteLine();

                    Battleship chosenShip = Player.ChooseShipToPlace(player);
                    int chosenDirection = Player.ChooseDirectionToPlaceShip(directionalPlacement);
                    int y_Coordinate = Player.CheckInputAxisIsValid("Choose an y coordinate to place the ship.");
                    int x_Coordinate = Player.CheckInputAxisIsValid("Choose an x coodrinate to place the ship.");

                    currentOceanGrid = player.PlaceShipsOnOceanGrid(currentOceanGrid, chosenShip, chosenDirection, [y_Coordinate, x_Coordinate], ref isValid);
                    if (isValid)
                    {
                        shipList.Remove(chosenShip);
                    }
                } while (!isValid);
            }
            return currentOceanGrid;
        }
        static void DisplayRules()
        {

        }
    }
}
