namespace BattleShip_PSchmitt
{
    class BattleshipGame
    {
        public enum MainMenuChoices
        {
            Invalid,
            Plaver_Vs_CPU,
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
            }





            List<int[]> ints = new List<int[]>();
            Battleship smallBattleship = new Battleship((int)BattleshipList.Destroyer, ints, BattleshipList.Destroyer.ToString());
            Console.WriteLine(smallBattleship.name);

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
    }
}
