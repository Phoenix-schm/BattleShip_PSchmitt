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
        public bool IsStillFloating
        {
            get
            {
                return _eachIndexSpace.Count > 0;
            }
        }
        public int ShipLength
        {
            get { return _shipLength; }
            set { _shipLength = Math.Max(0, value); }
        }
        public List<int[]> EachIndexOnOceanGrid
        {
            get { return _eachIndexSpace; }
        }
        public char Display
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
    }
}
