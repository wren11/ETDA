using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BotCore.PathFinding;
using BotCore.Actions;

namespace BotCore.Types
{
    public class MapObject : GameClient.RepeatableTimer, IDiscoverable, ISearchable, ITargetable, IWalkable
    {
        public MapObject()
        {
            Timer = new UpdateTimer(TimeSpan.FromMilliseconds(100));
        }

        public Direction Direction;

        internal bool IsCursed;

        public Position OldPosition;
        public EventHandler<List<PathSolver.PathFinderNode>> PathUpdated = delegate { };

        public ushort Sprite;

        public Stopwatch timer = new Stopwatch();

        public DateTime TimeSeen = DateTime.Now;

        public Position ServerPosition { get; set; }

        public int Serial { get; set; }

        public MapObjectType Type { get; set; }

        public int MovementSpeed { get; set; }

        public CurseInfo CurseInfo { get; set; }

        public FasInfo FasInfo { get; set; }

        public List<PathSolver.PathFinderNode> PathToMapObject { get; set; }

        public int TargetPriority { get; set; }

        public Func<MapObject, bool> CanTarget { get; set; }

        public void OnDiscovery(GameClient client)
        {
            Client = client;

            if (Client.InstalledComponents.Count(n => 
                n.ComponentID == Serial) == 0)
            {
                base.ComponentID = Serial;
                Client.InstalledComponents.Add(this);
            }

            if (Type == MapObjectType.NPC || Type == MapObjectType.Monster || Type == MapObjectType.Aisling ||
                this is Aisling)
                client.FieldMap.SetWall(ServerPosition);

            ReassignObj(client);
            SetTargetPriorties(client);
            OverrideTargetPriorties();
            UpdatePath(client);

            timer.Start();
        }

        public void OnRemoved(GameClient client)
        {
            Console.WriteLine("Entity: {0} was Removed.", Serial);
            client.FieldMap.SetPassable(ServerPosition);
            client.FieldMap.RemoveObject(this);
            PathToMapObject = null;

            var obj = client.InstalledComponents.FirstOrDefault
                (n => n.ComponentID == Serial);

            if (obj != null)
            {
                client.InstalledComponents.Remove(obj);
            }
        }

        public void OnPositionUpdated(GameClient client, Position oldPosition, Position newPosition)
        {
            if (Timer == null)
            {
                Timer = new UpdateTimer(TimeSpan.FromMilliseconds(1));
            }

            Timer.Reset();


            //lets time movement speed.
            if (timer.ElapsedMilliseconds > 0)
                MovementSpeed = (int) timer.ElapsedMilliseconds;


            Console.WriteLine("Entity Moved {0},{1} -> {2},{3}", oldPosition.X, oldPosition.Y, newPosition.X,
                newPosition.Y);

            timer.Restart();
            client.FieldMap.SetPassable(oldPosition);
            client.FieldMap.SetWall(newPosition);
            UpdatePath(client);

        }

        private void ReassignObj(GameClient client)
        {
            var obj = client.FieldMap.MapObjects.FirstOrDefault(i => i.Serial == Serial);
            if (obj == null)
                client.FieldMap.AddObject(this);
            else
            {
                if (obj is Aisling)
                {
                    if ((obj as Aisling).IsHidden && this is Aisling
                        && !(this as Aisling).IsHidden)
                    {
                        (obj as Aisling).IsHidden = false;
                    }
                }
            }
        }

        private void OverrideTargetPriorties()
        {
            if (Collections.TargetConditions.ContainsKey((short) Sprite))
            {
                CanTarget = Collections.TargetConditions[(short) Sprite].predicate;
                TargetPriority = Collections.TargetConditions[(short) Sprite].Priority;
            }
        }

        //here should be the default target prority.
        //only hard coded logic should be placed here.
        private void SetTargetPriorties(GameClient client)
        {
            if (Type == MapObjectType.Monster && CanTarget == null)
                // don't re-create it again if it's already defined.
                //as this can be defined by plugin settings.
            {
                switch (Sprite)
                {
                    case 1: // target is a wasp.
                    {
                        CanTarget = target => true; //return true, no special condition here.
                        TargetPriority = 1; //give it 1, making this sprite a higher priority.
                    }
                        break;
                    default:
                    {
                        CanTarget = target => true;
                        TargetPriority = 0; //give it 0 - nothing special.
                    }
                        break;
                }
            }
        }

        public void UpdatePath(GameClient client)
        {
            if (Type == MapObjectType.NPC || Type == MapObjectType.Monster || Type == MapObjectType.Aisling
                || this is Aisling
                && Serial != client.Attributes.Serial)
            {
                PathToMapObject = client.FieldMap.Search(client.Attributes.ClientPosition, ServerPosition);
                PathUpdated(client, PathToMapObject);
            }
        }

        public override void Pulse()
        {
            if (CurseInfo != null && CurseInfo.CurseElapsed)
                CurseInfo = null;

            //if (Type == MapObjectType.Monster || this is Aisling)
            //    GameActions.Say(Client, this, Serial + " (" + ServerPosition.X + "," + ServerPosition.Y + ")  " + (CurseInfo != null ? "Cursed" : "Not Cursed"));
        }

        public override void Update(TimeSpan tick)
        {
            Timer.Update(tick);

            if (Timer.Elapsed)
            {
                Pulse();
                Timer.Reset();
            }
        }

        public void OnAnimation(short animation, int to, int from)
        {
            switch ((Animation) animation)
            {
                //TODO: Add more Animation Handling here.
                case Animation.ardcradh:
                {
                    if (CurseInfo != null)
                        CurseInfo.Applied = DateTime.Now;

                    if (CurseInfo == null)
                    {
                        CurseInfo = new CurseInfo
                        {
                            Applied = DateTime.Now,
                            Type = CurseInfo.Curse.ardcradh,
                            Duration = 240000
                        };
                    }
                    IsCursed = true;
                }
                    break;
            }
        }

        public bool WalkTowards(int distance)
        {
            if (Client.FieldMap == null)
                return false;
            if (!Client.FieldMap.Ready)
                return false;

            UpdatePath(Client);

            if (PathToMapObject == null)
                return false;

            if (PathToMapObject?.Count == 0)
                return false;

            Direction dir = Direction.None;
            var PathNodes = PathToMapObject;

            if (PathNodes[1].X == Client.FieldMap.X() && PathNodes[1].Y == Client.FieldMap.Y() - 1)
                dir = Direction.North;
            else if (PathNodes[1].X == Client.FieldMap.X() && PathNodes[1].Y == Client.FieldMap.Y() + 1)
                dir = Direction.South;
            else if (PathNodes[1].X == Client.FieldMap.X() - 1 && PathNodes[1].Y == Client.FieldMap.Y())
                dir = Direction.West;
            else if (PathNodes[1].X == Client.FieldMap.X() + 1 && PathNodes[1].Y == Client.FieldMap.Y())
                dir = Direction.East;

            GameActions.Walk(Client, dir);
            return true;
        }
    }

    internal interface IWalkable
    {
        bool WalkTowards(int distance);
    }

    internal interface ISearchable
    {
        List<PathSolver.PathFinderNode> PathToMapObject { get; set; }
    }
}