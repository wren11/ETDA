using BotCore.Actions;
using BotCore.PathFinding;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BotCore.Types
{
    public class MapObject : GameClient.RepeatableTimer, IDiscoverable, ISearchable, ITargetable
    {
        public EventHandler<List<PathSolver.PathFinderNode>> PathUpdated = delegate { };
        
        public ushort Sprite;

        public Direction Direction;

        public Position OldPosition;

        public DateTime TimeSeen = DateTime.Now;

        internal bool isCursed;

        public Position ServerPosition { get; set; }

        public int Serial { get; set; }
 
        public MapObjectType Type { get; set; }

        public List<PathSolver.PathFinderNode> PathToMapObject { get; set; }

        public Stopwatch timer = new Stopwatch();

        public int MovementSpeed { get; set; }

        public MapObject()
        {
            Timer = new UpdateTimer(TimeSpan.FromMilliseconds(1000));
        }

        public void OnDiscovery(GameClient client)
        {
            Client = client;

            if (Type == MapObjectType.NPC || Type == MapObjectType.Monster || Type == MapObjectType.Aisling)
                client.FieldMap.SetWall(ServerPosition);

            ReassignObj(client);
            SetTargetPriorties(client);
            OverrideTargetPriorties();
            UpdatePath(client);

            timer.Start();
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
                    if ((obj as Aisling).IsHidden && (this is Aisling)
                        && !(this as Aisling).IsHidden)
                    {
                        (obj as Aisling).IsHidden = false;
                    }
                }
                else
                {
                    obj = this;
                }
            }
        }

        private void OverrideTargetPriorties()
        {
            if (Collections.TargetConditions.ContainsKey((short)Sprite))
            {
                CanTarget = Collections.TargetConditions[(short)Sprite].predicate;
                TargetPriority = Collections.TargetConditions[(short)Sprite].Priority;
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
                            CanTarget = (target) => true; //return true, no special condition here.
                            TargetPriority = 1; //give it 1, making this sprite a higher priority.
                        }
                        break;
                    default:
                        {
                            CanTarget = (target) => true;
                            TargetPriority = 0; //give it 0 - nothing special.
                        }
                        break;
                }
            }
        }

        public void UpdatePath(GameClient client)
        {
            if (Type == MapObjectType.NPC || Type == MapObjectType.Monster || Type == MapObjectType.Aisling
                && Serial != client.Attributes.Serial)
            {
                PathToMapObject = client.FieldMap.Search(client.Attributes.ClientPosition, ServerPosition);
                PathUpdated(client, PathToMapObject);
            }
        }

        public void OnRemoved(GameClient client)
        {
            client.FieldMap.SetPassable(ServerPosition);
            client.FieldMap.RemoveObject(this);
            PathToMapObject = null;
        }

        public void OnPositionUpdated(GameClient client, Position oldPosition, Position newPosition)
        {
            //lets time movement speed.
            if (timer.ElapsedMilliseconds > 0)
                MovementSpeed = (int)timer.ElapsedMilliseconds;

            timer.Restart();
            client.FieldMap.SetPassable(oldPosition);
            client.FieldMap.SetWall(newPosition);
            UpdatePath(client);
        }

        public int TargetPriority { get; set; }

        public Func<MapObject, bool> CanTarget { get; set; }

        public CurseInfo CurseInfo { get; set; }

        public FasInfo FasInfo { get; set; }

        public override void Pulse()
        {
            if (CurseInfo != null)
            {
                GameActions.Say(Client, this, (CurseInfo.Duration - (int)((DateTime.Now - CurseInfo.Applied).TotalMilliseconds)).ToString());
            }
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
    }

    internal interface ISearchable
    {
        List<PathSolver.PathFinderNode> PathToMapObject { get; set; }
    }
}