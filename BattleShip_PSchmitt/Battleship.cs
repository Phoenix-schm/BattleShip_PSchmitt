using System;
using System.Collections.Generic;
namespace BattleShip_PSchmitt
{
    class Battleship
    {
        private int _shipLength;
        private List<int[]> _eachIndexSpace = [];
        public string name = "";
        private char _displayNuetral;
        private char _displayWhenHit;
        public char displayWhenSunk = 'N';
        public bool IsHit   // use for CPU ai. if there's a ship that's been hit and is still floating, then zone in on it
        {
            get
            {
                return _shipLength < _eachIndexSpace.Count && IsStillFloating;
            }
        }
        public bool IsStillFloating
        {
            get
            {
                return _shipLength > 0;
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
        public char DisplayNuetral
        {
            get { return _displayNuetral; }
        }
        public char DisplayWhenHit
        {
            get { return _displayWhenHit; }
        }
        public Battleship()
        {
            _shipLength = 0;
            name = "ship";
        }
        public Battleship(int length, List<int[]> spaces, string shipName, char displayOnGrid, char displayWhenHit)
        {
            _shipLength = length;
            name = shipName;
            _eachIndexSpace = spaces;
            _displayNuetral = displayOnGrid;
            _displayWhenHit = displayWhenHit;
        }
    }
}
