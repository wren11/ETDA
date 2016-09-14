using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BotCore.PathFinding;
using BotCore.Types;

namespace BotCore.Components
{
    public class Map : UpdateableComponent
    {
        public static byte[] sotp = File.ReadAllBytes("sotp.dat");

        private MapObject[] _mapObjects = new MapObject[0];

        public bool Ready;
        short Width, Height;

        public Map()
        {
            Timer = new UpdateTimer(TimeSpan.FromMilliseconds(1.0));
            Width = 0;
            Height = 0;
        }

        public int[,] Grid { get; private set; }

        public int this[short x, short y]
        {
            get { return Grid[x, y]; }
            set
            {
                if (x != 0 && y != 0)
                {
                    Grid[x, y] = value;
                }
            }
        }

        public int this[Position p]
        {
            get { return this[p.X, p.Y]; }
            set { this[p.X, p.Y] = value; }
        }

        public MapObject[] MapObjects
        {
            get
            {
                List<MapObject> copy;
                lock (_mapObjects)
                {
                    copy = new List<MapObject>(_mapObjects);
                }
                return copy.ToArray();
            }
        }

        public bool CanUseSkills { get; set; }

        public bool CanCastSpells { get; set; }

        public override string ToString()
        {
            var result = "";
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

        public List<Position> GetNearByTiles(short x, short y, double radius)
        {
            var nearbyPtsSet = new List<Position>();
            var innerBound = radius*(Math.Sqrt(2.0)/2.0);
            var radiusSq = radius*radius;

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

            var ptr = _memory.Read<int>((IntPtr) 0x00882E68, false) + 0x26C;
            var num = _memory.Read<short>((IntPtr) ptr, false);

            return num;
        }

        public short MapWidth()
        {
            if (_memory == null || !_memory.IsRunning || !IsInGame())
                return 0;

            var ptr = _memory.Read<int>((IntPtr) 0x00882E68, false) + 0x1C4;
            var num = _memory.Read<short>((IntPtr) ptr, false);

            return num;
        }

        public short MapHeight()
        {
            if (_memory == null || !_memory.IsRunning || !IsInGame())
                return 0;

            var ptr = _memory.Read<int>((IntPtr) 0x00882E68, false) + 0x1C8;
            var num = _memory.Read<short>((IntPtr) ptr, false);

            return num;
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
                lock (_mapObjects)
                {
                    Array.Resize(ref _mapObjects, _mapObjects.Length + 1);
                    _mapObjects[_mapObjects.Length - 1] = obj;
                }
            }
        }

        public void RemoveObject(MapObject obj)
        {
            for (var i = 0; i < _mapObjects.Length; i++)
            {
                if (_mapObjects[i].Serial == obj.Serial)
                {
                    RemoveObject(ref _mapObjects, i);
                }
            }
        }

        private void RemoveObject(ref MapObject[] arr, int index)
        {
            lock (_mapObjects)
            {
                for (var a = index; a < arr.Length - 1; a++)
                    arr[a] = arr[a + 1];

                Array.Resize(ref arr, arr.Length - 1);
            }
        }

        public short X()
        {
            if (_memory == null || !_memory.IsRunning || !IsInGame())
                return 0;

            var ptr = _memory.Read<int>((IntPtr)0x00882E68, false) + 0x23C;
            var xcoord = _memory.Read<short>((IntPtr)ptr, false);
            return xcoord;
        }

        public short Y()
        {
            if (_memory == null || !_memory.IsRunning || !IsInGame())
                return 0;

            var ptr = _memory.Read<int>((IntPtr) 0x00882E68, false) + 0x238;
            var xcoord = _memory.Read<short>((IntPtr) ptr, false);
            return xcoord;
        }

        public void Init(short Width, short Height, short Number)
        {
            Grid = null;

            int w = Width;
            int h = Height;
            var m_grid = new int[w, h];

            unsafe
            {
                fixed (int* ptr = m_grid)
                {
                    for (var i = 0; i < w*h; ++i)
                        ptr[i] = 0;
                }
            }

            Grid = m_grid;

            this.Width = Width;
            this.Height = Height;

            Console.WriteLine($"Map Init {Width}, {Height}");
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
            if (Grid.GetLength(0) != 0 && Grid.GetLength(0) == width)
                return;

            lock (_mapObjects)
            {
                Array.Clear(_mapObjects, 0, _mapObjects.Length);
                _mapObjects = new MapObject[0];
            }

            Init(width, height, number);
            ProcessMap(data);
            data.Dispose();

            if (Client.IsInGame())
                ((Client) Client).OnClientStateUpdated(true);

            Console.WriteLine("Map Handled {0}, {1}", Width, Height);
        }


        private void SetTile(int value, short x, short y)
        {
            var w = Grid.GetLength(0);
            var h = Grid.GetLength(1);

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

            //TODO: these need reworking!
            //RemoveOutofSight();
            //AddOnSight();
        }

        private void AddOnSight()
        {
            lock (_mapObjects)
            {
                for (var i = 0; i < _mapObjects.Length; i++)
                {
                    if (_mapObjects[i%_mapObjects.Length].ServerPosition.DistanceFrom(new Position(X(), Y())) < 9)
                        _mapObjects[i%_mapObjects.Length].OnDiscovery(Client);
                }
            }
        }

        private void RemoveOutofSight()
        {
            lock (_mapObjects)
            {
                for (var i = 0; i < _mapObjects.Length; i++)
                {
                    if (_mapObjects[i%_mapObjects.Length].ServerPosition.DistanceFrom(new Position(X(), Y())) > 9)
                        _mapObjects[i%_mapObjects.Length].OnRemoved(Client);
                }
            }
        }

        public List<PathSolver.PathFinderNode> Search(Position Start, Position End)
        {
            lock (_mapObjects)
            {
                if (Grid.GetLength(0) == 0 || Grid.GetLength(1) == 0)
                    return null;

                if (!Client.MapLoaded)
                    return null;

                var usingpath = PathSolver.FindPath(Grid, Start, End);
                return usingpath;
            }
        }
    }
}