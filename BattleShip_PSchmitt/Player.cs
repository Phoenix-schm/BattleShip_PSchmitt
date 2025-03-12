using System;
using System.Collections.Generic;
namespace BattleShip_PSchmitt
{
    class Player
    {
        public enum Directions
        {
            Invalid,
            Up,
            Down,
            Left,
            Right
        }
        public char targetSunkDisplay = 'N';
        public char targetMissDisplay = 'M';
        public char targetHitDisplay = 'H';

        public char[,] oceanGrid;
        public char[,] targetGrid;
        public List<Battleship> shipList;
        public string name = "Player";
        public int[]? previousShot;
        public int goesFirst = 0;
        public bool IsAlive
        {
            get
            {
                return shipList.Count > 0;
            }
        }

        public Player()
        {
            shipList = CreateShips();                        // Default number of ships. Will act as health
            oceanGrid = GameGrid.CreateDefaultGrid();        // Deafult ocean grid. Will contain ships
            targetGrid = GameGrid.CreateDefaultGrid();       // Default target grid. Will show shots taken
        }
        public Player(string playerName)
        {
            shipList = CreateShips();                        // Default number of ships. Will act as health
            oceanGrid = GameGrid.CreateDefaultGrid();        // Deafult ocean grid. Will contain ships
            targetGrid = GameGrid.CreateDefaultGrid();       // Default target grid. Will show shots taken
            name = playerName;
        }

        /// <summary>
        /// Creation of each battleship
        /// </summary>
        /// <returns>An array of each batteship: amount of int spaces it takes up, an empty List of spaces it takes on a grid,
        /// and its string name</returns>
        public static List<Battleship> CreateShips()
        {
            Battleship destroyerShip = new Battleship(2, spaces: [], "Destroyer", 'd', 'Z');
            Battleship submarineShip = new Battleship(3, spaces: [], "Submarine", 's', 'Y');
            Battleship cruiserShip = new Battleship(3, spaces: [], "Cruiser", 'c', 'X');
            Battleship battleShip = new Battleship(4, spaces: [], "Battleship", 'B', 'W');
            Battleship carrierShip = new Battleship(5, spaces: [], "Carrier", 'C', 'V');

            return [destroyerShip, submarineShip, cruiserShip, battleShip, carrierShip];
        }
    }
}
