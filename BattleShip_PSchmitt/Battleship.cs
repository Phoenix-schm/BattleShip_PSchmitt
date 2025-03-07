using System;
using System.Collections.Generic;
namespace BattleShip_PSchmitt
{
    class Battleship
    {
        private int _shipLength;
        private List<int[]> _eachIndexSpace = [];
        public string name = "";
        private char _display;
        public bool stillOnGameBoard
        {
            get 
            {
                return _eachIndexSpace.Count > 0;
            }
        }
        public int shipLength
        {
            get { return _shipLength; }
            set { _shipLength = Math.Max(0, value); }
        }
        public List<int[]> eachIndexSpace
        {
            get { return _eachIndexSpace; }
        }
        public char display
        {
            get { return _display; }
        }
        public Battleship()
        {
            _shipLength = 0;
            name = "ship";
        }
        public Battleship(int length, List<int[]> spaces, string shipName, char displayOnGrid)
        {
            _shipLength = length;
            name = shipName;
            _eachIndexSpace = spaces;
            _display = displayOnGrid;
        }

        public void RemoveShip(int y, int x)
        {

        }
    }
}
