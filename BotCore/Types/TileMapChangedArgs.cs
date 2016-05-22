using System;

namespace BotCore
{
    public class TileMapChangedArgs : EventArgs
    {
        public short X, Y;
        public int PreviousValue;
        public int NewValue;

        public TileMapChangedArgs(short x, short y, int previous, int newvalue)
        {
            X = x;
            Y = y;
            PreviousValue = previous;
            NewValue = newvalue;
        }
    }
}
