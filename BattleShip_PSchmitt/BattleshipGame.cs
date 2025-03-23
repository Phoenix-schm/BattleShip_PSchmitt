namespace BattleShip_PSchmitt
{
    public enum DirectionList
    {
        Invalid,
        Up,
        Down,
        Left,
        Right
    }
    class BattleshipGame
    {
        // used in PlayerVsPlayer()
        public int winningPlayer = 0;
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
            
            DisplayMessageAndClear("Tip: For best experience, resize the console vertically to be bigger. \nPress any key to continue...");

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

            // Create player grids
            Console.WriteLine("Time to place your ships.");
            player.oceanGrid = TesterGrid(player);
            //HumanPlayer.CreateOceanGrid(player);
            CPUPlayer.CreateCPUoceanGrid(cpuPlayer);

            BasePlayer[] playerOrder = [player, cpuPlayer];

            Play_BattleshipGame(playerOrder);

            if (player.IsAlive)
            {
                Console.WriteLine("Congrats! You are victorious!");
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
            HumanPlayer.CreateOceanGrid(player1);

            DisplayMessageAndClear(player2.name + ", it's your turn to make a grid. \nTake your place at the computer and press any key to continue...");
            HumanPlayer.CreateOceanGrid(player2);

            // Initialize play order
            BasePlayer[] playerOrder = PlayerInput.ChooseWhoGoesFirstInput(player1, player2);

            // Play battleship
            DisplayMessageAndClear("Have " + playerOrder[0].name + " take command of the computer. \nPress any key when you're ready to begin...");
            playerOrder = Play_BattleshipGame(playerOrder);

            // Display winner
            Console.WriteLine("Congratulations " + playerOrder[game.winningPlayer].name + ", you beat " + playerOrder[game.losingPlayer].name + ".");
            Console.WriteLine(playerOrder[game.winningPlayer].name + " finished the game with " + playerOrder[game.winningPlayer].shotsTaken + " shots.");

            DisplayMessageAndClear("Press any key to continue...");
            return PlayerInput.PlayAgainInput();
        }

        enum PlayerOrder
        {
            FirstPlayer = 0,
            SecondPlayer = 1,
        }

        /// <summary>
        /// Plays the battleship game with each player taking turns \
        /// </summary>
        /// <param name="playerOrder">The order of who goes first and second when playing</param>
        /// <returns>An array of who won, in order of winner then loser.</returns>
        static BasePlayer[] Play_BattleshipGame(BasePlayer[] playerOrder)
        {
            BattleshipGame game = new BattleshipGame();
            Random random = new Random();
            string shotMessage = "";

            // initializes the player order 
            int currentPlayer = (int)PlayerOrder.FirstPlayer;
            int nextPlayer = (int)PlayerOrder.SecondPlayer;

            while (playerOrder[currentPlayer].IsAlive && playerOrder[nextPlayer].IsAlive)
            {
                if (playerOrder[currentPlayer] is HumanPlayer)
                {
                    shotMessage = HumanPlayer.HumanPlayerTurn((HumanPlayer)playerOrder[currentPlayer], playerOrder[nextPlayer], shotMessage);

                    if (playerOrder[nextPlayer].IsAlive)
                    {
                        DisplayMessageAndClear("Press any key to continue...");     // offers a buffer to see the results of their shot
                        if (playerOrder[nextPlayer] is not CPUPlayer)    // Occurs in Player Vs Player, offers a buffer to switch between players
                        {
                            DisplayMessageAndClear("Switch player to " + playerOrder[nextPlayer].name + ". Press any key when ready to continue...");
                        }
                    }
                }
                else
                {
                    shotMessage = CPUPlayer.CPUPlayerTurn((CPUPlayer)playerOrder[currentPlayer], playerOrder[nextPlayer], random);     // CPU shoots, returns the shot message
                }

                if (currentPlayer == (int)PlayerOrder.FirstPlayer)    // Switch between who the current player and next player is
                {
                    currentPlayer = (int)PlayerOrder.SecondPlayer;
                    nextPlayer = (int)PlayerOrder.FirstPlayer;
                }
                else
                {
                    currentPlayer = (int)PlayerOrder.FirstPlayer;
                    nextPlayer = (int)PlayerOrder.SecondPlayer;
                }
            }

            BasePlayer[] whoWon = new BasePlayer[playerOrder.Length];
            foreach (BasePlayer player in playerOrder)      // Creates an array of the winning results in order of winner to loser
            {
                if (player.IsAlive)
                {
                    whoWon[game.winningPlayer] = player;
                }
                else
                {
                    whoWon[game.losingPlayer] = player;
                }
            }

            return whoWon;
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

        /// <summary>
        /// Fully clears the console of all elements
        /// </summary>
        public static void FullyClearConsole()
        {
            Console.ResetColor();
            Console.Clear();
            Console.WriteLine("\x1b[3J");       // Fully clears the console
            Console.Clear();                    // Previous line adds a WriteLine(), clear again
        }

        /// <summary>
        /// An ready made tester grid for debugging the CPU ai
        /// </summary>
        /// <param name="player">player being modified</param>
        /// <returns>Grid filled with player ships</returns>
        static char[,] TesterGrid(HumanPlayer player)
        {
            
            char[,] oceanGrid = player.oceanGrid;
            List<Battleship> shipList = player.shipList;
            bool canshipBePlaced = false;

            foreach (Battleship ship in shipList)
            {
                if (ship.name == "Submarine")
                {
                    oceanGrid = OceanGrid.PlaceShipOnOceanGrid(player, ship, DirectionList.Right, [0, 4], ref canshipBePlaced);
                }
                else if (ship.name == "Carrier")
                {
                    oceanGrid = OceanGrid.PlaceShipOnOceanGrid(player, ship, DirectionList.Down, [5, 0], ref canshipBePlaced);
                }
                else if (ship.name == "Cruiser")
                {
                    oceanGrid = OceanGrid.PlaceShipOnOceanGrid(player, ship, DirectionList.Right, [9, 4], ref canshipBePlaced);
                }
                else if (ship.name == "Battleship")
                {
                    oceanGrid = OceanGrid.PlaceShipOnOceanGrid(player, ship, DirectionList.Up, [5, 9], ref canshipBePlaced);
                }
                else if (ship.name == "Destroyer")
                {
                    oceanGrid = OceanGrid.PlaceShipOnOceanGrid(player, ship, DirectionList.Right, [4, 4], ref canshipBePlaced);
                }
            }

            return oceanGrid;
        }
    }
}
