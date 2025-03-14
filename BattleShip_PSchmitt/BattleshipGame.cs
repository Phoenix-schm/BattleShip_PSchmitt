namespace BattleShip_PSchmitt
{
    class BattleshipGame
    {
        // used in PlayerVsPlayer()
        int winningPlayer = 0;
        int losingPlayer = 1;

        static void Main(string[] args)
        {
            string[] mainMenuChoices = { "Invalid", "Player Vs CPU", "Player Vs Player", "Quit" };
            string[] titleScreen =
            {
                " ____        _   _   _           _     _       ",
                "| __ )  __ _| |_| |_| | ___  ___| |__ (_)_ __  ",
                "|  _ \\ / _` | __| __| |/ _ \\/ __| '_ \\| | '_ \\ ",
                "| |_) | (_| | |_| |_| |  __/\\__ \\ | | | | |_) |",
                "|____/ \\__,_|\\__|\\__|_|\\___||___/_| |_|_| .__/ ",
                "                                        |_|    \r\n",
            };

            foreach (string row in titleScreen)
            {
                Console.WriteLine(row);
            }
            DisplayMessageAndClear("Press any key to continue...");

            //start game here
            bool isPlayingBattleship = true;
            while (isPlayingBattleship)
            {
                Console.WriteLine("Choose your game:");
                DisplayMainMenuChoices(mainMenuChoices);
                string playerInput = PlayerInput.CheckMainMenuChoice(mainMenuChoices);
                bool isPlayingRound = true;

                switch (playerInput)                                         // Player choice of what to play
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
                    default:
                        Console.WriteLine("This shouldn't be accessible.");
                        break;
                }
                Console.Clear();
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
        /// <returns>Boolean for whether the player wants to try again.</returns>
        static bool Play_PlayerVsCPU()
        {
            // Initialize player variables
            HumanPlayer player = new HumanPlayer("Player");
            CPUPlayer cpuPlayer = new CPUPlayer("CPU");

            Console.WriteLine("Howdy! Time to make your grid.");
            CreateOceanGrid(player);
            //CPU.CreateCPUoceanGrid(player, rand);
            CPUPlayer.CreateCPUoceanGrid(cpuPlayer);

            BasePlayer[] playerOrder = [player, cpuPlayer];

            PlayBattleshipGame(playerOrder);

            if (player.IsAlive)
            {
                Console.WriteLine("Congrats! you are victorious!");
                Console.WriteLine("You finished the game with " + player.shotsTaken + " shots.");
            }
            else
            {
                Console.WriteLine("You lost against the computer. You weren't up to the challenge.");
                Console.WriteLine("You finished the game with " + player.shotsTaken + " shots.");
            }
            DisplayMessageAndClear("Press any key to continue...");

            return PlayerInput.PlayAgainInput();
        }

        /// <summary>
        /// Going through Battleship with two players
        /// </summary>
        /// <returns>Boolean for if players want to try again.</returns>
        static bool Play_PlayerVsPlayer()
        {
            BattleshipGame game = new BattleshipGame();
            HumanPlayer player1 = new HumanPlayer("Player 1");
            HumanPlayer player2 = new HumanPlayer("Player 2");

            // Initialize Player grids, one at a time.
            DisplayMessageAndClear("Alright " + player1.name + ", make your grid. \nTake your place at the computer and press any key to continue...");
            //CreateOceanGrid(player1);
            CPUPlayer.CreateCPUoceanGrid(player1);

            DisplayMessageAndClear(player2.name + ", it's your turn to make a grid. \nTake your place at the computer and press any key to continue...");
            //CreateOceanGrid(player2);
            CPUPlayer.CreateCPUoceanGrid(player2);

            Console.WriteLine("Time to choose who goes first.");
            Console.WriteLine("Player names:");
            Console.WriteLine(player1.name);
            Console.WriteLine(player2.name);
            Console.Write("Write the name of the player that goes first: ");

            BasePlayer[] playerOrder = PlayerInput.ChooseWhoGoesFirstInput(player1, player2);

            DisplayMessageAndClear("Have " + playerOrder[0].name + " take command of the computer. \nPress any key when you're ready to begin...");
            playerOrder = PlayBattleshipGame(playerOrder);

            Console.WriteLine("Congratulations " + playerOrder[game.winningPlayer].name + " you beat " + playerOrder[game.losingPlayer].name + ".");
            Console.WriteLine(playerOrder[game.winningPlayer].name + " finished the game with " + playerOrder[game.winningPlayer].shotsTaken + " shots.");

            DisplayMessageAndClear("Press any key to continue...");
            return PlayerInput.PlayAgainInput();
        }

        static BasePlayer[] PlayBattleshipGame(BasePlayer[] playerOrder)
        {
            Random random = new Random();
            int currentPlayer = 0;
            int nextPlayer = 1;
            string shotMessage = "";

            while (playerOrder[currentPlayer].IsAlive && playerOrder[nextPlayer].IsAlive)
            {
                if (playerOrder[currentPlayer] is HumanPlayer)
                {
                    shotMessage = HumanPlayer.PlayerTurn(playerOrder[currentPlayer], playerOrder[nextPlayer], shotMessage);
                    if (playerOrder[nextPlayer].IsAlive && playerOrder[nextPlayer] is not CPUPlayer)    // Occurs in Player Vs Player
                    {
                        DisplayMessageAndClear("Press any key to continue...");
                        DisplayMessageAndClear("Switch player to " + playerOrder[nextPlayer].name + ". Press any key when ready to continue...");
                    }
                    else if (playerOrder[nextPlayer].IsAlive)                                           // Occuts in Player Vs CPU
                    {
                        DisplayMessageAndClear("Press any key to continue...");
                    }
                }
                else
                {
                    shotMessage = CPUPlayer.CPUPlayerTurn((CPUPlayer)playerOrder[currentPlayer], playerOrder[nextPlayer], random);     // CPU shoots, returns the shot message
                }

                if (currentPlayer == playerOrder.Length - 1)    // Switch between who the current player is
                {
                    currentPlayer = 0;
                    nextPlayer = 1;
                }
                else
                {
                    currentPlayer = 1;
                    nextPlayer = 0;
                }
            }

            BasePlayer[] whoWon = new BasePlayer[2];
            foreach (BasePlayer player in playerOrder)
            {
                if (player.IsAlive)
                {
                    whoWon[0] = player;
                }
                else
                {
                    whoWon[1] = player;
                }
            }

            return whoWon;
        }

        /// <summary>
        /// Has player place all battleships onto the grid
        /// </summary>
        /// <param name="player">Current player placing onto the grid.</param>
        /// <returns>Returns the updated grid.</returns>
        static char[,] CreateOceanGrid(BasePlayer player)
        {
            List<Battleship> playerShipList = player.shipList;
            Battleship? chosenShip = null;

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
                        OceanGrid.DisplayOceanGrid(player);
                    }
                    else                                                                    // Shows the grid at least once
                    {
                        Console.WriteLine("Current Player Grid:");
                        OceanGrid.DisplayOceanGrid(player);
                    }

                    Console.WriteLine("You have " + (playerShipList.Count - shipCountIndex) + " ships left to place.");
                    DisplayShipsForPlacement(playerShipList);
                    Console.WriteLine();

                    chosenShip = PlayerInput.ChooseShipToPlace(player);

                    int userY = PlayerInput.CheckInputNumIsOnGrid("Choose a y coordinate to place the ship");
                    int userX = PlayerInput.CheckInputNumIsOnGrid("Choose an x coodrinate to place the ship");

                    int directionListIndex = 0;
                    Console.WriteLine("Placement Directions: ");
                    foreach (BasePlayer.DirectionList direction in Enum.GetValues(typeof(BasePlayer.DirectionList)))
                    {
                        if (direction == BasePlayer.DirectionList.Invalid)
                        {
                            continue;
                        }
                        else
                        {
                            directionListIndex++;
                            Console.WriteLine(directionListIndex + ") " + direction);
                        }
                    }
                    Console.WriteLine();
                    BasePlayer.DirectionList chosenDirection = PlayerInput.ChooseDirectionToPlaceShip();

                    OceanGrid.PlaceShipOnOceanGrid(player, chosenShip, chosenDirection, [userY, userX], ref isValidCoordinates);

                    doneOnce = 1;
                    FullyClearConsole();

                    if (isValidCoordinates)                         // If the player correctly placed the ship, check if they want to undo
                    {
                        Console.WriteLine("Current Player Grid:");
                        OceanGrid.DisplayOceanGrid(player);
                        PlayerInput.CheckRedoGridInput(ref chosenShip, ref shipCountIndex, player.oceanGrid);
                    }
                } while (!isValidCoordinates);
            }
            return player.oceanGrid;
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

        /// <summary>
        /// Displays a message before waiting for a key press and then clearing the console. 
        /// </summary>
        /// <param name="message">A string that will be displayed. Should usually request the player press any key.</param>
        static void DisplayMessageAndClear(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(message);
            Console.ReadKey();
            FullyClearConsole();
        }
        public static void FullyClearConsole()
        {
            Console.ResetColor();
            Console.Clear();
            Console.WriteLine("\x1b[3J");       // Fully clears the console
            Console.Clear();                    // Previous line adds a WriteLine()
        }
    }
}
