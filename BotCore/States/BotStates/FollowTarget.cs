using BotCore.Actions;
using BotCore.Components;
using BotCore.States.BotStates;
using BotCore.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading;

namespace BotCore.States
{
    public class LeaderCollection : CollectionBase
    {
        public FollowTarget.Leader this[int index]
        {
            get { return (FollowTarget.Leader)List[index]; }
        }
        public void Add(FollowTarget.Leader emp)
        {
            List.Add(emp);
        }
        public void Remove(FollowTarget.Leader emp)
        {
            List.Remove(emp);
        }
    }

    public class FollowCollectionEditor : CollectionEditor
    {
        public FollowCollectionEditor(Type type)
            : base(type)
        {
        }

        protected override string GetDisplayText(object value)
        {
            FollowTarget.Leader item = new FollowTarget.Leader();
            item = (FollowTarget.Leader)value;
            
            return base.GetDisplayText(string.Format("{0}, {1}", item.Name,
                item.Serial));
        }
    }

    [State(Author: "Dean", Desc: "Will follow a target at a distance specified.")]
    public class FollowTarget : GameState
    {
        [Editor(typeof(FollowCollectionEditor),
        typeof(System.Drawing.Design.UITypeEditor))]
        public List<Leader> Leaders { get; set; }

        public override void InitState()
        {
            Leaders = new List<Leader>();

            Leaders.AddRange(Client.OtherClients.Select(i => new Leader() {
                Name = i.Attributes.PlayerName,
                Serial = i.Attributes.Serial,
                Client = i.Client
            }));
        }

        public Leader m_target = null;

        public class Leader
        {
            public string Name { get; set; }

            public int Serial { get; set; }

            [Browsable(false)]
            public GameClient Client { get; set; }
        }

        private int m_Followinstance = 2;
        [Description("Trail Distance"), Category("Following Conditions")]
        public int Distance
        {
            get { return m_Followinstance; }
            set { m_Followinstance = value; }
        }

        public override bool NeedToRun
        {
            get
            {

                var closest = (from v in Leaders
                               where v.Client.IsInGame() && v.Name != Client.Attributes.PlayerName
                               orderby Client.Attributes.ServerPosition.DistanceFrom(v.Client.Attributes.ServerPosition)
                               select v).FirstOrDefault();



                if (Client.FieldMap != null && Client.IsInGame()
                    && Client.MapLoaded && CanWalk())
                {

                    if (Client.ObjectSearcher != null)
                    {
                        m_target = closest;

                        if (m_target != null)
                        {
                            if (Client.Attributes?
                                .ServerPosition?
                                .DistanceFrom(m_target?.Client.Attributes.ServerPosition) > Distance)
                                return true;
                        }
                    }

                    return false;
                }
                else
                {
                    return false;
                }
            }
            set
            {

            }
        }

        public override int Priority { get; set; }

        public bool IsSurrounded(Position loc)
        {
            return false;
        }

        Position lastlocation;
        DateTime laststep;

        public bool CanWalk()
        {
            return (!Client.SpellBar.Contains((short)SpellBar.palsy)
                || Client.SpellBar.Contains((short)SpellBar.skulled)
                || Client.SpellBar.Contains((short)SpellBar.pramh));
        }

        public void Follow(int distance, List<PathFinding.PathSolver.PathFinderNode> array
            , PlayerAttributes player, MapObject mapobj)
        {
            int num = 0;

            if (Client.FieldMap != null && Client.MapLoaded)
            {
                if (player != null && !IsSurrounded(player.ServerPosition))
                {
                    if (Client.Attributes.ServerPosition.DistanceFrom(player.ServerPosition) > distance)
                    {

                        num = distance;
                        lastlocation = player.ServerPosition;

                        for (int i = 1; i < array.Count - num; i++)
                        {
                            if (IsSurrounded(player.ServerPosition))
                            {
                                break;
                            }
                            if (player.ServerPosition.DistanceFrom(Client.Attributes.ServerPosition) < num)
                            {
                                break;
                            }
                            if (!CanWalk())
                            {
                                break;
                            }

                            if (player == null)
                            {
                                break;
                            }
                            if (num == 0)
                            {
                                break;
                            }

                            var block = Client.FieldMap.Grid[array[i].X, array[i].Y];
                            if (block == 1)
                            {
                                break;
                            }
                            if (Math.Abs(Client.Attributes.ServerPosition.X - array[i].X) +
                                Math.Abs(Client.Attributes.ServerPosition.Y - array[i].Y) != 1)
                            {
                                break;
                            }
                            Direction direction = Client.Attributes.ServerPosition - new Position(array[i].X, array[i].Y);
                            if (direction == Direction.None)
                            {
                                break;
                            }
                            GameActions.Walk(Client, direction);
                            laststep = DateTime.UtcNow;

                            Thread.Sleep(20);
                        }
                    }
                }
            }
        }

        public override void Run(TimeSpan Elapsed)
        {
            if (Enabled && !InTransition)
            {
                InTransition = true;

                if (m_target != null)
                {
                    var player = m_target.Client.Attributes;
                    var mapobj = Client.ObjectSearcher.RetreivePlayerTarget(i => i.Serial == player.Serial);

                    if (mapobj != null)
                    {
                        mapobj.UpdatePath(Client);
                        var path = mapobj.PathToMapObject;

                        if (path != null && path.Count > 0)
                        {
                            var pos = new Position(path[1].X, path[1].Y);
                            var abx = pos.X - Client.Attributes.ServerPosition.X;
                            var aby = pos.Y - Client.Attributes.ServerPosition.Y;

                            if (abx == 1 && aby == 0)
                            {
                                GameActions.Walk(Client, Direction.East);
                            }
                            else if (abx == 0 && aby == -1)
                            {
                                GameActions.Walk(Client, Direction.North);
                            }
                            else if (abx == -1 && aby == 0)
                            {
                                GameActions.Walk(Client, Direction.West);
                            }
                            else
                            {
                                GameActions.Walk(Client, Direction.South);
                            }
                        }

                        Follow(Distance, path, player, mapobj);
                    }
                }
            }

            Client.TransitionTo(this, Elapsed);

        }
    }
}
