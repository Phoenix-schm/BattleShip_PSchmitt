namespace BattleShip_PSchmitt
{
    class BattleshipGame
    {
        public enum MainMenuChoices
        {
            Invalid,
            Player_Vs_CPU,
            Player_Vs_Player,
            Tutorial,
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

                switch (playerInput)                                         // Player choice of which to play
                {
                    case "quit":
                        isPlaying = false;
                        break;
                    case "player vs cpu":
                        Console.Clear();
                        Play_PlayerVsCPU();
                        break;
                    case "player vs player":
                        break;
                    case "tutorial":
                        break;
                    default:
                        Console.WriteLine("This shouldn't be accessible.");
                        break;
                }
            }
        }
        /// <summary>
        /// Displays enum MainMenuChoices with their associated number
        /// </summary>
        static void DisplayMainMenuChoices()
        {
            foreach (MainMenuChoices choice in Enum.GetValues(typeof(MainMenuChoices)))
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
        static void Play_PlayerVsCPU()
        {
            Player player = new Player("Player");
            Player cpu = new Player("CPU");
            Random rand = new Random();

            Console.WriteLine("Howdy Player! Time to make your grid");
            cpu.playerOceanGrid = CPU.CreateCPUoceanGrid(cpu, rand);
            // player.playerOceanGrid = CreateOceanGrid(player);
            player.playerOceanGrid = CPU.CreateCPUoceanGrid(player, rand);

            Console.WriteLine("Now for battle!");
            GameGrid.DisplayPlayerGrids(player);
            GameGrid.DisplayPlayerGrids(cpu);
            while (player.isAlive && cpu.isAlive)
            {
                int yCoord = Player.CheckInputNumIsOnGrid("Choose a y coordinate to shoot");
                int xCoord = Player.CheckInputNumIsOnGrid("Choose an x coordinate to shoot");

                TargetGrid.PlaceShotsOnTargetGrid(player, cpu, yCoord, xCoord);
                GameGrid.DisplayPlayerGrids(player);

                CPU.ChooseRandomShot(cpu, player, rand);
            }


        }

        /// <summary>
        /// Has player place all battleships onto the grid
        /// </summary>
        /// <param name="player">Current player placing onto the grid.</param>
        /// <returns>Returns the updated grid.</returns>
        static char[,] CreateOceanGrid(Player player)
        {
            char[,] playerOceanGrid = player.playerOceanGrid;
            List<Battleship> playerShipList = player.playerShipList;
            Battleship chosenShip = null;

            string[] directionList = { "Up", "Down", "Left", "Right" };
            bool isValidCoordinates = false;
            int doneOnce = 0;

            for (int shipCountIndex = 0; shipCountIndex < playerShipList.Count; shipCountIndex++)     // Going through the number of ships in the ship list
            {
                do
                {
                    if (doneOnce == 1 && !isValidCoordinates)                               // If the previous coordinates weren't valid
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("That was not a valid coordinate");
                        Console.ResetColor();
                        Console.WriteLine("Current Player Grid:");
                        OceanGrid.DisplayOceanGrid(playerOceanGrid);
                    }
                    else                                                                    // Shows the grid at least once
                    {
                        Console.WriteLine("Current Player Grid:");
                        OceanGrid.DisplayOceanGrid(playerOceanGrid);
                    }

                    if (doneOnce == 1 && isValidCoordinates)                               // If the player correctly placed the ship, check if they want to undo
                    {
                        playerOceanGrid = Player.CheckRedoGrid(ref chosenShip, ref shipCountIndex, playerOceanGrid);
                        Console.WriteLine("Current Player Grid:");
                        OceanGrid.DisplayOceanGrid(playerOceanGrid);
                    }

                    Console.WriteLine("You have " + (playerShipList.Count - shipCountIndex) + " ships left to place.");
                    DisplayShipsForPlacement(playerShipList);
                    Console.WriteLine();

                    chosenShip = Player.ChooseShipToPlace(player);

                    int userY = Player.CheckInputNumIsOnGrid("Choose a y coordinate to place the ship");
                    int userX = Player.CheckInputNumIsOnGrid("Choose an x coodrinate to place the ship");   

                    Console.WriteLine("Placement Directions: ");
                    for (int directionIndex = 0; directionIndex < directionList.Length; directionIndex++)
                    {
                        Console.WriteLine((directionIndex + 1) + ") " + directionList[directionIndex]);
                    }
                    Console.WriteLine();
                    int chosenDirection = Player.ChooseDirectionToPlaceShip(directionList);

                    playerOceanGrid = OceanGrid.PlaceShipsOnOceanGrid(playerOceanGrid, chosenShip, chosenDirection, [userY, userX], ref isValidCoordinates);

                    doneOnce = 1;
                    Console.Clear();
                    Console.WriteLine("\x1b[3J");                   // Fully clears the console. Somehow. Without it, the console only clears what is directly seen
                } while (!isValidCoordinates);                      //      and the player can scroll up to see previous outputs
            }
            return playerOceanGrid;
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
                if (shipList[index].eachIndexSpace.Count > 0)               // if the ship has already been placed onto the grid
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(index + 1 + ") " + shipList[index].name + " = " + shipList[index].shipLength + " spaces");
                }
                else
                {
                    Console.WriteLine(index + 1 + ") " + shipList[index].name + " = " + shipList[index].shipLength + " spaces");
                }
                Console.ResetColor();
            }
        }
        static void DisplayRules()
        {
            Player tutorial = new Player();
            Console.WriteLine("The rules of battleship are simple");

        }
    }
}
