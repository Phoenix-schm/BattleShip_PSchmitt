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
            bool isPlaying = true;

            foreach (string row in titleScreen)
            {
                Console.WriteLine(row);
            }
            Console.WriteLine("Press any key to start...");
            Console.ReadKey();
            Console.WriteLine();

            //start game here
            while (isPlaying)
            { 
                Console.WriteLine("Choose your game:");
                DisplayMainMenuChoices();
                string playerInput = Player.CheckMainMenuChoice();

                switch(playerInput)
                {
                    case "quit":
                        isPlaying = false; 
                        break;
                    case "player vs cpu":
                        PlayPlayerVsCPU();
                        break;
                    case "player vs player":
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
        static void PlayPlayerVsCPU()
        {
            Player player = new Player();
            Player cpu = new Player();

            Console.WriteLine("Howdy Player! ");
            player._playerOceanGrid[0, 0] = 'H';                // test
            player.DisplayOceanGrid(player._playerOceanGrid);
        }
    }
}
