using System;
using System.Collections.Generic;
namespace BattleShip_PSchmitt
{
    class Battleship
    {
        private int _shipLength;
        private List<int[]> _eachIndexSpace = [];
        public string name = "";
        public char _display = ' ';
        public int shipLength
        {
            get { return _shipLength; }
            set { _shipLength = Math.Max(0, value); }
        }
        public List<int[]> eachIndexSpace
        {
            get { return _eachIndexSpace; }
        }
        public Battleship()
        {
            _shipLength = 0;
            name = "ship";
        }
        public Battleship(int length, List<int[]> spaces, string shipName, char display)
        {
            _shipLength = length;
            name = shipName;
            _eachIndexSpace = spaces;
            _display = display;
        }
        public List<int[]> AddTakeUpSpace(int y, int x)
        {
            _eachIndexSpace.Add([y, x]);
            return _eachIndexSpace;
        }

        public List<int[]> RemoveTakeUpSpace(int y, int x)
        {
            _eachIndexSpace.Remove([y, x]);
            return _eachIndexSpace;
        }
    }
}
