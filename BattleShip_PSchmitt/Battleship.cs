using System;
using System.Collections.Generic;
namespace BattleShip_PSchmitt
{
    public enum BattleshipList
    {
        Destroyer = 2,
        Submarine = 3,
        Cruiser = 3,
        Battleship = 4,
        Carrier = 5
    }
    class Battleship
    {
        private int _shipLength;
        private List<int[]> _takeUpSpaces = [];
        public string name = "";
        public int shipLength
        {
            get{ return _shipLength; }
            set{ _shipLength = Math.Max(0, value); }
        }
        public List<int[]> takeUpSpaces
        {
            get { return _takeUpSpaces; }
        }
        public Battleship()
        {
            _shipLength = 0;
            name = "ship";
        }
        public Battleship(int length, List<int[]> spaces, string shipName)
        {
            _shipLength = length;
            name = shipName;
            _takeUpSpaces = spaces;
        }
        public List<int[]> AddTakeUpSpace(int y, int x)
        {
            _takeUpSpaces.Add([y, x]);
            return _takeUpSpaces;
        }

        public List<int[]> RemoveTakeUpSpace(int y, int x)
        {
            _takeUpSpaces.Remove([y, x]);
            return _takeUpSpaces;
        }



    }
}
