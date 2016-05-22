using System;

namespace BotCore.Types
{
    public class Position : MathUtilties
    {
        public short X, Y;

        public int DistanceFrom(Position other)
        {
            return DistanceFrom(other.X, other.Y);
        }

        public int DistanceFrom(short X, short Y)
        {
            double XDiff = Math.Abs(X - this.X);
            double YDiff = Math.Abs(Y - this.Y);
            return (int)(XDiff > YDiff ? XDiff : YDiff);
        }

        public Position(short x, short y)
        {
            X = x;
            Y = y;
        }
        public Position(byte x, byte y) : this((short)x, (short)y) { }
        public Position(int x, int y) : this((short)x, (short)y) { }
        public Position() : this(0, 0) { }
    }
}
