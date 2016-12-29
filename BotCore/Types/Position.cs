using BotCore.Actions;
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

        public bool IsNearby(Position pos)
        {
            return pos.DistanceFrom(X, Y) <= 1;
        }
        public bool WithinSquare(Position loc, int num)
        {
            return Math.Abs(X - loc.X) <= num && Math.Abs(Y - loc.Y) <= num;
        }

        public static Position operator +(Position a, Direction b)
        {
            var location = new Position(a.X, a.Y);
            switch (b)
            {
                case Direction.North:
                    location.Y--;
                    return location;
                case Direction.East:
                    location.X++;
                    return location;
                case Direction.South:
                    location.Y++;
                    return location;
                case Direction.West:
                    location.X--;
                    return location;
            }
            return location;
        }

        public static Direction operator -(Position a, Position b)
        {
            if ((a.X == b.X) && (a.Y == (b.Y + 1)))
                return Direction.North;
            if ((a.X == b.X) && (a.Y == (b.Y - 1)))
                return Direction.South;
            if ((a.X == (b.X + 1)) && (a.Y == b.Y))
                return Direction.West;
            if ((a.X == (b.X - 1)) && (a.Y == b.Y))
                return Direction.East;

            return Direction.None;
        }

        public bool IsNextTo(Position pos)
        {
            if (X == pos.X && Y + 1 == pos.Y)
            {
                return true;
            }
            if (X == pos.X && Y - 1 == pos.Y)
            {
                return true;
            }
            if (X == pos.X + 1 && Y == pos.Y)
            {
                return true;
            }
            if (X == pos.X - 1 && Y == pos.Y)
            {
                return true;
            }

            return false;
        }

    }
}
