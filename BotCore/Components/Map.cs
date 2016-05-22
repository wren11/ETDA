using System;
using System.IO;
using BotCore.Types;
using System.Linq;
using System.Collections.Generic;
using BotCore.PathFinding;

namespace BotCore.Components
{

    public class Map : UpdateableComponent
    {
        public static byte[] sotp = File.ReadAllBytes("sotp.dat");

        public int[,] Grid
        {
            get
            {
                return grid;
            }
        }

        private int[,] grid;
        private short Width, Height, Number;

        public override string ToString()
        {
            string result = "";
            for (short i = 0; i < Height; i++)
            {
                for (short j = 0; j < Width; j++)
                {
                    if (j == X() && i == Y())
                    {
                        result += "x";
                    }
                    else
                        result += IsWall(j, i) ? "1" : "0";
                }
                result += Environment.NewLine;
            }
            return result;
        }

        public int this[short x, short y]
        {
            get
            {
                return grid[x, y];
            }
            set
            {
                if (x != 0 && y != 0)
                {
                    grid[x, y] = value;
                }
            }
        }

        public int this[Position p]
        {
            get
            {
                return this[p.X, p.Y];
            }
            set
            {
                this[p.X, p.Y] = value;
            }
        }

        public List<Position> GetNearByTiles(short x, short y, double radius)
        {
            List<Position> nearbyPtsSet = new List<Position>();
            double innerBound = radius * (Math.Sqrt(2.0) / 2.0);
            double radiusSq = radius * radius;

            for (short j = 0; j < Height; j++)
            {
                for (short i = 0; i < Width; i++)
                {
                    var xDist = Math.Abs(x - i);
                    var yDist = Math.Abs(y - j);

                    if (xDist > radius || yDist > radius)
                        continue;
                    if (xDist > innerBound || yDist > innerBound)
                        continue;
                    if (IsWall(i, j))
                        continue;
                    if (i == x && j == y)
                        continue;

                    if (new Position(x, y).DistanceFrom(new Position(i, j)) < radiusSq)
                        nearbyPtsSet.Add(new Position(i, j));
                }
            }

            return nearbyPtsSet;
        }
      
        public short MapNumber()
        {
            if (_memory == null || !_memory.IsRunning || !IsInGame())
                return 0;

            var ptr = _memory.Read<int>((IntPtr)0x00882E68, false) + 0x26C;
            var num = _memory.Read<short>((IntPtr)ptr, false);

            return num;
        }

        public short MapWidth()
        {
            if (_memory == null || !_memory.IsRunning || !IsInGame())
                return 0;

            var ptr = _memory.Read<int>((IntPtr)0x00882E68, false) + 0x1C4;
            var num = _memory.Read<short>((IntPtr)ptr, false);

            return num;
        }

        public short MapHeight()
        {
            if (_memory == null || !_memory.IsRunning || !IsInGame())
                return 0;

            var ptr = _memory.Read<int>((IntPtr)0x00882E68, false) + 0x1C8;
            var num = _memory.Read<short>((IntPtr)ptr, false);

            return num;
        }


        public short X()
        {
            if (_memory == null || !_memory.IsRunning || !IsInGame())
                return 0;

            var ptr = _memory.Read<int>((IntPtr)0x00882E68, false) + 0x23C;
            var xcoord = _memory.Read<short>((IntPtr)ptr, false);
            return xcoord;
        }

        private MapObject[] _MapObjects = new MapObject[0];
        public bool Ready;

        public MapObject[] MapObjects
        {
            get
            {
                List<MapObject> copy;
                lock (_MapObjects)
                {
                    copy = new List<MapObject>(_MapObjects);
                }
                return copy.ToArray();
            }
        }

        public MapObject GetObject(short x, short y)
        {
            return GetObject(i => i != null
            && i.ServerPosition != null
            && i.ServerPosition.X == x
            && i.ServerPosition.Y == x);
        }

        public MapObject GetObject(Position p)
        {
            return GetObject(p.X, p.Y);
        }

        public MapObject GetObject(Func<MapObject, bool> predicate)
        {
            return MapObjects.FirstOrDefault(i => i != null && predicate(i));
        }

        public bool ObjectExists(MapObject obj)
        {
            return GetObject(i => i.Serial == obj.Serial) != null;
        }

        public void AddObject(MapObject obj)
        {
            if (!ObjectExists(obj))
            {
                lock (_MapObjects)
                {
                    Array.Resize(ref _MapObjects, _MapObjects.Length + 1);
                    _MapObjects[_MapObjects.Length - 1] = obj;
                }
            }
        }

        public void RemoveObject(MapObject obj)
        {
            for (int i = 0; i < _MapObjects.Length; i++)
            {
                if (_MapObjects[i].Serial == obj.Serial)
                {
                    RemoveObject(ref _MapObjects, i);
                }
            }
        }

        private void RemoveObject(ref MapObject[] arr, int index)
        {
            lock (_MapObjects)
            {
                for (int a = index; a < arr.Length - 1; a++)
                    arr[a] = arr[a + 1];

                Array.Resize(ref arr, arr.Length - 1);
            }
        }

        public short Y()
        {
            if (_memory == null || !_memory.IsRunning || !IsInGame())
                return 0;

            var ptr = _memory.Read<int>((IntPtr)0x00882E68, false) + 0x238;
            var xcoord = _memory.Read<short>((IntPtr)ptr, false);
            return xcoord;
        }

        public void Init(short Width, short Height, short Number)
        {
            grid = null;

            int w = Width;
            int h = Height;
            int[,] m_grid = new int[w, h];

            unsafe
            {
                fixed (int* ptr = m_grid)
                {
                    for (int i = 0; i < w * h; ++i)
                        ptr[i] = 0;
                }
            }

            grid = m_grid;

            this.Width = Width;
            this.Height = Height;
            this.Number = Number;

            Console.WriteLine("Map Init {0}, {1}", Width, Height);
        }

        public static bool Parse(short lWall, short rWall)
        {
            if (lWall == 0 && rWall == 0)
                return false;
            if (lWall == 0)
                return sotp[rWall - 1] == 0x0F;
            if (rWall == 0)
                return sotp[lWall - 1] == 0x0F;
            return
                sotp[lWall - 1] == 0x0F && sotp[rWall - 1] == 0x0F;
        }

        public bool IsWall(short x, short y)
        {
            if (x < Grid.GetLength(0) && y < Grid.GetLength(1))
                return Grid[x, y] == 1;

            throw new mapException("Not Loaded");
        }

        public void ProcessMap(Stream stream)
        {
            var reader = new BinaryReader(stream);

            for (short y = 0; y < Height; y++)
            {
                for (short x = 0; x < Width; x++)
                {
                    reader.BaseStream.Seek(2, SeekOrigin.Current);
                    if (Parse(reader.ReadInt16(), reader.ReadInt16()))
                        SetWall(x, y);
                    else
                        SetPassable(x, y);
                }
            }

            reader.Close();
            stream.Close();

            Console.WriteLine("Map Process {0}, {1}", Width, Height);
        }

        public void OnMapLoaded(Stream data, short number, short width, short height)
        {
            if (grid.GetLength(0) == 0 || grid.GetLength(0) != width)
            {
                lock (_MapObjects)
                {
                    Array.Clear(_MapObjects, 0, _MapObjects.Length);
                    _MapObjects = new MapObject[0];
                }

                Init(width, height, number);
                ProcessMap(data);
                data.Dispose();

                if (Client.IsInGame())
                   (Client as Client).OnClientStateUpdated(true);

                Console.WriteLine("Map Handled {0}, {1}", Width, Height);
            }
        }


        private void SetTile(int value, short x, short y)
        {
            var w = grid.GetLength(0);
            var h = grid.GetLength(1);

            if (x > w || x < w)
                return;
            if (y > h || y < h)
                return;

            this[x, y] = value;

        }

        public void SetPassable(short x, short y)
        {
            SetTile(0, x, y);
        }

        public void SetPassable(Position p)
        {
            SetTile(0, p.X, p.Y);
        }


        public void SetWall(Position p)
        {
            SetWall(p.X, p.Y);
        }       

        public void SetWall(short x, short y)
        {
            SetTile(1, x, y);
        }

        public Map()
        {
            Timer  = new UpdateTimer(TimeSpan.FromMilliseconds(100.0));
            Width  = 0;
            Height = 0;
            Number = 0;
        }

        public override void Update(TimeSpan tick)
        {
            Timer.Update(tick);

            if (Timer.Elapsed)
            {
                OnUpdate();

                Timer.Reset();
            }
        }

        private void OnUpdate()
        {
            Client.Attributes.ClientPosition = new Position(X(), Y());

            RemoveOutofSight();
            AddOnSight();
        }

        private void AddOnSight()
        {
            lock (_MapObjects)
            {
                for (int i = 0; i < _MapObjects.Length; i++)
                {
                    if (_MapObjects[i % _MapObjects.Length].ServerPosition.DistanceFrom(new Position(X(), Y())) >= 12)
                        _MapObjects[i % _MapObjects.Length].OnDiscovery(Client);
                }
            }
        }

        private void RemoveOutofSight()
        {
            lock (_MapObjects)
            {
                for (int i = 0; i < _MapObjects.Length; i++)
                {
                    if (_MapObjects[i % _MapObjects.Length].ServerPosition.DistanceFrom(new Position(X(), Y())) >= 12)
                        _MapObjects[i % _MapObjects.Length].OnRemoved(Client);
                }
            }            
        }

        public List<PathSolver.PathFinderNode> Search(Position Start, Position End)
        {
            lock (_MapObjects)
            {
                if (grid.GetLength(0) == 0 || grid.GetLength(1) == 0)
                    return null;

                if (Client.MapLoaded)
                {
                    var usingpath = PathSolver.FindPath(grid, Start, End);
                    return usingpath;
                };

                return null;
            }
        }

        public bool CanUseSkills { get; set; }

        public bool CanCastSpells { get; set; }
    }    
}
