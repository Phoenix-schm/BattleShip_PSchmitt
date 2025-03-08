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
                bool isPlayingRound = true;

                switch (playerInput)                                         // Player choice of which to play
                {
                    case "quit":
                        isPlayingBattleship = false;
                        break;
                    case "player vs cpu":
                        while (isPlayingRound)
                        {
                            Console.Clear();
                            isPlayingRound = Play_PlayerVsCPU();
                        }
                        break;
                    case "player vs player":
                        while (isPlayingRound)
                        {
                            Console.Clear();
                            isPlayingRound = Play_PlayerVsPlayer();
                        }
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
                DisplayShotTakenMessage(player, playerMessage, ConsoleColor.Cyan);
                DisplayShotTakenMessage(cpu, cpuMessage, ConsoleColor.Red);

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

        static bool Play_PlayerVsPlayer()
        {
            Player player1 = new Player("Player 1");
            Player player2 = new Player("Player 2");

            Random random = new Random();

            // Initialize Player grids, one at a time.
            Console.WriteLine("Alright " + player1.name + ", make your grid.");
            //player1.playerOceanGrid = CreateOceanGrid(player1);
            player1.playerOceanGrid = CPU.CreateCPUoceanGrid(player1, random);

            Console.WriteLine(player2.name + " it's your turn to make a grid.");
            //player2.playerOceanGrid = CreateOceanGrid(player2);
            player2.playerOceanGrid = CPU.CreateCPUoceanGrid(player2, random);

            Console.WriteLine("Time to choose who goes first.");
            Console.Write("Write the name of the player that goes first: ");
            PlayerInput.ChooseWhoGoesFirstInput(player1, player2);

            Player firstPlayer;
            Player secondPlayer;
            if (player1.goesFirst == 1)
            {
                firstPlayer = player1;
                secondPlayer = player2;
            }
            else
            {
                firstPlayer = player2;
                secondPlayer = player1;
            }

            int firstShotsTaken = 0;
            int secondShotsTaken = 0;
            string firstPlayerMessage;
            string secondPlayerMessage = "";
            int[] userCoordinates;

            while(player1.IsAlive && player2.IsAlive)
            {
                GameGrid.DisplayPlayerGrids(firstPlayer);
                DisplayShotTakenMessage(secondPlayer, secondPlayerMessage, ConsoleColor.Red);       // Displays the shot message of the previous secondPlayer shot

                userCoordinates = TargetGrid.ReturnValidUserCoordinates(firstPlayer);
                firstPlayerMessage = TargetGrid.PlaceShotsOnTargetGrid(firstPlayer, secondPlayer, userCoordinates[0], userCoordinates[1]);
                Console.Clear();
                Console.WriteLine("\x1b[3J");

                GameGrid.DisplayPlayerGrids(firstPlayer);
                DisplayShotTakenMessage(firstPlayer, firstPlayerMessage, ConsoleColor.Cyan);        // Displays the shot message of the current firstPlayer shot
                firstShotsTaken++;

                if (secondPlayer.IsAlive)
                {
                    DisplayMessageAndClear("Press any key to continue...");
                    DisplayMessageAndClear("Switch Player! Press any key to continue...");

                    GameGrid.DisplayPlayerGrids(secondPlayer);
                    DisplayShotTakenMessage(firstPlayer, firstPlayerMessage, ConsoleColor.Red);     // Displays the shot message of the previous firstPlayer shot

                    userCoordinates = TargetGrid.ReturnValidUserCoordinates(secondPlayer);
                    secondPlayerMessage = TargetGrid.PlaceShotsOnTargetGrid(secondPlayer, firstPlayer, userCoordinates[0], userCoordinates[1]);
                    Console.Clear();
                    Console.WriteLine("\x1b[3J");

                    GameGrid.DisplayPlayerGrids(secondPlayer);
                    DisplayShotTakenMessage(secondPlayer, secondPlayerMessage, ConsoleColor.Cyan);  // Displays the shot message of the current secondPlayer shot
                    secondShotsTaken++;
                }

                if (firstPlayer.IsAlive && secondPlayer.IsAlive)
                {
                    DisplayMessageAndClear("Press any key to continue...");
                    DisplayMessageAndClear("Switch Player! Press any key to continue...");
                }
            }

            if (firstPlayer.IsAlive)
            {
                Console.WriteLine("Congratulations " + firstPlayer.name + " you beat " + secondPlayer.name + ".");
                Console.WriteLine(firstPlayer.name + " finished the game with " + firstShotsTaken + " shots.");
            }
            else
            {
                Console.WriteLine("Congratulations " + secondPlayer.name + " you beat " + firstPlayer.name + ".");
                Console.WriteLine(secondPlayer.name + " finished the game with " + secondShotsTaken + " shots.");
            }

            DisplayMessageAndClear("Press any key to continue...");
            return PlayerInput.PlayAgainInput();
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
        static void DisplayShotTakenMessage(Player displayPlayer, string targetMessage, ConsoleColor color)
        {
            if (displayPlayer.previousShot != null)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(displayPlayer.name + " Turn:");
                Console.WriteLine("----------------------");
                Console.WriteLine(displayPlayer.name + " shoots coordinate " + (displayPlayer.previousShot[0] + 1) + "," + (displayPlayer.previousShot[1] + 1) + ".");
                Console.WriteLine(targetMessage);
                Console.WriteLine();
            }
            Console.ResetColor();
        }

        static void DisplayMessageAndClear(string message)
        {
            Console.Write(message);
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("\x1b[3J");
        }

        static void DisplayRules()
        {
            Player tutorial = new Player();
            Console.WriteLine("The rules of battleship are simple");

        }
    }
}
