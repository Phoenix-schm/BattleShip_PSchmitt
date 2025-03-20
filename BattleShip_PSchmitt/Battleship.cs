namespace BattleShip_PSchmitt
{
    class Battleship
    {
        private int _shipLength;
        private int _takesUpSpaces;
        public string name = "";
        private char _displayNuetral;
        private char _displayWhenHit;
        public bool IsHit   // use for CPU ai. if there's a ship that's been hit and is still floating, then zone in on it
        {
            get
            {
                return ShipLength < _takesUpSpaces && IsStillFloating;
            }
        }
        public bool IsStillFloating
        {
            get
            {
                return ShipLength > 0;
            }
        }
        public int ShipLength
        {
            get { return _shipLength; }
            set { _shipLength = Math.Max(0, value); }
        }

        public int takesUpSpaces
        {
            get { return _takesUpSpaces; }
            set { _takesUpSpaces = value; }
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
        public Battleship(int length, string shipName, char displayOnGrid, char displayWhenHit)
        {
            ShipLength = length;
            name = shipName;
            _displayNuetral = displayOnGrid;
            _displayWhenHit = displayWhenHit;
        }
    }
}
