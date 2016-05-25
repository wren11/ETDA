using BotCore.Types;
using System.Collections.Generic;

namespace BotCore.PathFinding
{
    public class RecursionSolver
    {
        protected Client client { get; private set; }

        bool[,] Visited { get; set; }
        bool[,] Path { get; set; }

        public Position Start, End;

        public RecursionSolver(Client client)
        {
            this.client = client;

            Prepare(client);
        }

        private void Prepare(Client client)
        {
            var h = client.FieldMap.MapHeight();
            var w = client.FieldMap.MapWidth();

            Path = new bool[w, h];
            Visited = new bool[w, h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Visited[x, y] = false;
                    Path[x, y] = false;
                }
            }
        }

        public ISet<Position> Solve(Client client, Position startPoint, Position endPoint)
        {
            Prepare(client);

            Start = startPoint;
            End   = endPoint;

            var success = Solve((byte)Start.X, (byte)Start.Y);
            if (!success)
                return null;

            var h = client.FieldMap.MapHeight();
            var w = client.FieldMap.MapWidth();

            var resultSet = new HashSet<Position>();

            for (byte y = 0; y < h; y++)
            {
                for (byte x = 0; x < w; x++)
                {
                    if (Path[x, y])
                    {
                        resultSet.Add(new Position(x, y));
                    }
                }
            }
            return resultSet;
        }

        public bool IsPositionValid(Position startPoint)
        {
            var H = client.FieldMap.MapHeight();
            var W = client.FieldMap.MapWidth();

            if (H == 0 && W == 0)
                return false;

            if (startPoint.X < 0 || startPoint.X > W - 1 || startPoint.Y < 0 || startPoint.Y > H - 1)
                return false;

            return true;
        }


        private bool Solve(byte x, byte y)
        {
            if (!IsPositionValid(new Position(x, y)))
                return false;

            if ((x == End.X) && (y == End.Y))
            {
                Path[x, y] = true;
                return true;
            }

            if (Visited[x, y])
                return false;

            Visited[x, y] = true;

            var currentPoint = new Position(x, y);
            var h = client.FieldMap.MapHeight();
            var w = client.FieldMap.MapWidth();

            if (x != 0)
            {
                var nextPoint = new Position(x - 1, y);
                if (Solve((byte)(x - 1), y) && !client.FieldMap.IsWall((byte)(x - 1), y))
                {
                    Path[x, y] = true;
                    return true;
                }
            }
            if (x != w - 1)
            {
                var nextPoint = new Position(x + 1, y);
                if (Solve((byte)(x + 1), y) && !client.FieldMap.IsWall((byte)(x + 1), y))
                {
                    Path[x, y] = true;
                    return true;
                }
            }
            if (y != 0)
            {
                var nextPoint = new Position(x, y - 1);
                if (Solve(x, (byte)(y - 1)) && !client.FieldMap.IsWall(x, (byte)(y - 1)))
                {
                    Path[x, y] = true;
                    return true;
                }
            }
            if (y != h - 1)
            {
                var nextPoint = new Position(x, y + 1);
                if (Solve(x, (byte)(y + 1)) && !client.FieldMap.IsWall(x, (byte)(y + 1)))
                {
                    Path[x, y] = true;
                    return true;
                }
            }

            return false;
        }
    }
}
