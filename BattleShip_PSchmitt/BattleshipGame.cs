using System.Diagnostics;
using System.Numerics;

namespace BattleShip_PSchmitt
{
    class BattleshipGame
    {
        static void Main(string[] args)
        {
            string[] mainMenuChoices = { "Invalid", "Player Vs CPU", "Player Vs Player", "Tutorial", "Quit" };
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
            bool isPlayingBattleship = true;
            while (isPlayingBattleship)
            {
                Console.WriteLine("Choose your game:");
                DisplayMainMenuChoices(mainMenuChoices);
                string playerInput = PlayerInput.CheckMainMenuChoice(mainMenuChoices);
                bool isPlayingShortGame = true;

                switch (playerInput)                                         // Player choice of which to play
                {
                    case "quit":
                        isPlayingBattleship = false;
                        break;
                    case "player vs cpu":
                        while (isPlayingShortGame)
                        {
                            Console.Clear();
                            isPlayingShortGame = Play_PlayerVsCPU();
                        }
                        break;
                    case "player vs player":
                        Play_PlayerVsPlayer();
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
        static void DisplayMainMenuChoices(string[] mainMenuChoices)
        {
            for (int index = 0; index < mainMenuChoices.Length; index++)
            {
                if (mainMenuChoices[index] == "Invalid")
                {
                    continue;
                }
                else
                {
                    Console.WriteLine(index + ") " + mainMenuChoices[index]);
                }
            }
        }

        /// <summary>
        /// Running through the game of player vs computer. First to sink all opponent battleships wins
        /// </summary>
        static bool Play_PlayerVsCPU()
        {
            // Initialize player variables
            Player player = new Player("Player");
            CPU cpu = new CPU();
            cpu.name = "CPU";

            Random rand = new Random();
            int shotsTaken = 0;

            Console.WriteLine("Howdy Player! Time to make your grid");
            //player.playerOceanGrid = CreateOceanGrid(player);
            player.playerOceanGrid = CPU.CreateCPUoceanGrid(player, rand);
            cpu.playerOceanGrid = CPU.CreateCPUoceanGrid(cpu, rand);

            Console.WriteLine("Now for battle!");
            //GameGrid.DisplayPlayerGrids(cpu);
            string cpuMessage = "";
            string playerMessage = "";

            while (player.IsAlive && cpu.IsAlive)                                                   // While both players have all their ships on the board.
            {
                GameGrid.DisplayPlayerGrids(player);
                DisplayPreviousShot(player, playerMessage, ConsoleColor.Cyan);
                DisplayPreviousShot(cpu, cpuMessage, ConsoleColor.Red);

                int[] userCoordinates = TargetGrid.ReturnValidUserCoordinates(player);

                playerMessage = TargetGrid.PlaceShotsOnTargetGrid(player, cpu, userCoordinates[0], userCoordinates[1]);     // Player shoots, returns the shot message
                shotsTaken++;

                if (cpu.IsAlive)
                {
                    cpuMessage = CPU.ChooseRandomShot(cpu, player, rand);                                                   // CPU shoots, returns the shot message
                }

                Console.Clear();
                Console.WriteLine("\x1b[3J");
            }

            if (player.IsAlive)
            {
                Console.WriteLine("Congrats " + player.name + " you are victorious!");
                Console.WriteLine("You shot: " + shotsTaken + " times.");
            }
            else
            {
                Console.WriteLine("Congrats " + cpu.name + " you are victorious");
            }

            return PlayerInput.PlayAgainInput();
        }

        static void Play_PlayerVsPlayer()
        {
            Player player1 = new Player("Player 1");
            Player player2 = new Player("Player 2");

            Random random = new Random();
            int shotsTaken = 0;

            Console.WriteLine("Alright " + player1.name + ", make your grid.");
            player1.playerOceanGrid = CreateOceanGrid(player1);
            Console.WriteLine(player2.name + " it's your turn to make a grid.");
            player2.playerOceanGrid = CreateOceanGrid(player2);

            Console.WriteLine("Time to choose who goes first.");
            Console.Write("Write the name of the player that goes first: ");
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
            Battleship? chosenShip = null;
            int chosenDirection = -1;

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
                        playerOceanGrid = PlayerInput.CheckRedoGridInput(ref chosenShip, ref shipCountIndex, playerOceanGrid, chosenDirection);
                        Console.WriteLine("Current Player Grid:");
                        OceanGrid.DisplayOceanGrid(playerOceanGrid);
                    }

                    Console.WriteLine("You have " + (playerShipList.Count - shipCountIndex) + " ships left to place.");
                    DisplayShipsForPlacement(playerShipList);
                    Console.WriteLine();

                    chosenShip = PlayerInput.ChooseShipToPlace(player);

                    int userY = PlayerInput.CheckInputNumIsOnGrid("Choose a y coordinate to place the ship");
                    int userX = PlayerInput.CheckInputNumIsOnGrid("Choose an x coodrinate to place the ship");

                    Console.WriteLine("Placement Directions: ");
                    for (int directionIndex = 0; directionIndex < directionList.Length; directionIndex++)
                    {
                        Console.WriteLine((directionIndex + 1) + ") " + directionList[directionIndex]);
                    }
                    Console.WriteLine();
                    chosenDirection = PlayerInput.ChooseDirectionToPlaceShip(directionList);

                    playerOceanGrid = OceanGrid.PlaceShipsOnOceanGrid(playerOceanGrid, chosenShip, chosenDirection, [userY, userX], ref isValidCoordinates);

                    doneOnce = 1;
                    Console.Clear();
                    Console.WriteLine("\x1b[3J");                   // Fully clears the console.
                } while (!isValidCoordinates);
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
                if (shipList[index].EachIndexOnOceanGrid.Count > 0)         // if the ship has already been placed onto the grid
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(index + 1 + ") " + shipList[index].name + " = " + shipList[index].ShipLength + " spaces");
                }
                else
                {
                    Console.WriteLine(index + 1 + ") " + shipList[index].name + " = " + shipList[index].ShipLength + " spaces");
                }
                Console.ResetColor();
            }
        }
        static void DisplayPreviousShot(Player displayPlayer, string targetMessage, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            if (displayPlayer.previousShot != null)
            {
                Console.WriteLine(displayPlayer.name + " Turn:");
                Console.WriteLine("----------------------");
                Console.WriteLine(displayPlayer.name + " shoots coordinate " + (displayPlayer.previousShot[0] + 1) + "," + (displayPlayer.previousShot[1] + 1) + ".");
                Console.WriteLine(targetMessage);
                Console.WriteLine();
            }
            Console.ResetColor();
        }

        static void DisplayRules()
        {
            Player tutorial = new Player();
            Console.WriteLine("The rules of battleship are simple");

        }
    }
}
