namespace BattleShip_PSchmitt
{
    class BasePlayer
    {

        // Target shot displays (used in both ocean grid and target grid)
        public char targetSunkDisplay = 'N';
        public char targetMissDisplay = 'M';
        public char targetHitDisplay = 'H';

        // Important initializations
        public char[,] oceanGrid;
        public char[,] targetGrid;
        public List<Battleship> shipList;
        public string name = "Player";
        public int[]? previousShot;

        // Misc use in game
        public int shotsTaken;

        public bool IsAlive
        {
            get
            {
                return shipList.Count > 0;
            }
        }

        public BasePlayer()
        {
            shipList = CreateShips();                        // Default number of ships. Will act as health
            oceanGrid = BaseGrid.CreateDefaultGrid();        // Deafult ocean grid. Will contain ships
            targetGrid = BaseGrid.CreateDefaultGrid();       // Default target grid. Will show shots taken
        }

        /// <summary>
        /// Creation of each battleship
        /// </summary>
        /// <returns>An array of each batteship: amount of int spaces it takes up, an empty List of spaces it takes on a grid,
        /// and its string name</returns>
        static List<Battleship> CreateShips()
        {
            Battleship destroyerShip = new Battleship(2, "Destroyer", 'd', 'Z');
            Battleship submarineShip = new Battleship(3, "Submarine", 's', 'Y');
            Battleship cruiserShip = new Battleship(3, "Cruiser", 'c', 'X');
            Battleship battleShip = new Battleship(4, "Battleship", 'B', 'W');
            Battleship carrierShip = new Battleship(5, "Carrier", 'C', 'V');

            return [destroyerShip, submarineShip, cruiserShip, battleShip, carrierShip];
        }
    }
}
