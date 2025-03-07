using System;
using System.Collections.Generic;
namespace BattleShip_PSchmitt
{
    class Player
    {
        public char[,] playerOceanGrid;
        public char[,] playerTargetGrid;
        public List<Battleship> playerShipList;
        public string name = "";
        public int[] previousShot;
        public bool IsAlive
        {
            get
            {
                return playerShipList.Count > 0;
            }
        }

        public Player()
        {
            playerShipList = CreateShips();                        // Default number of ships. Will act as health
            playerOceanGrid = GameGrid.CreateDefaultGrid();        // Deafult ocean grid. Will contain ships
            playerTargetGrid = GameGrid.CreateDefaultGrid();       // Default target grid. Will show shots taken
        }
        public Player(string playerName)
        {
            playerShipList = CreateShips();                        // Default number of ships. Will act as health
            playerOceanGrid = GameGrid.CreateDefaultGrid();        // Deafult ocean grid. Will contain ships
            playerTargetGrid = GameGrid.CreateDefaultGrid();       // Default target grid. Will show shots taken
            name = playerName;
        }

        /// <summary>
        /// Creation of each battleship
        /// </summary>
        /// <returns>An array of each batteship: amount of int spaces it takes up, an empty List of spaces it takes on a grid,
        /// and its string name</returns>
        public List<Battleship> CreateShips()
        {
            Battleship destroyerShip = new Battleship(2, spaces: [], "Destroyer", 'd');
            Battleship submarineShip = new Battleship(3, spaces: [], "Submarine", 's');
            Battleship cruiserShip = new Battleship(3, spaces: [], "Cruiser", 'c');
            Battleship battleShip = new Battleship(4, spaces: [], "Battleship", 'B');
            Battleship carrierShip = new Battleship(5, spaces: [], "Carrier", 'C');

            return [destroyerShip, submarineShip, cruiserShip, battleShip, carrierShip];
        }
    }
}
