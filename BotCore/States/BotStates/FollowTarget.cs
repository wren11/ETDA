using BotCore.Actions;
using BotCore.States.BotStates;
using BotCore.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;

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

            Leaders.AddRange(Client.OtherClients.Select(i => new Leader()
            {
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
                    && Client.MapLoaded && Client.Utilities.CanWalk())
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


        public override void Run(TimeSpan Elapsed)
        {
            if (Enabled && !InTransition)
            {
                InTransition = true;

                if (m_target != null)
                {
                    var mapobj = Client.ObjectSearcher.RetreivePlayerTarget(i =>
                        i?.Serial == m_target?.Serial);

                    if (mapobj != null)
                    {
                        mapobj.UpdatePath(Client);
                        var path = mapobj.PathToMapObject;

                        if (path != null && path.Count > Distance)
                        {
                            ComputeStep(path, mapobj);
                        }
                    }
                }
            }
            Client.TransitionTo(this, Elapsed);
        }

        private void ComputeDirection(List<PathFinding.PathSolver.PathFinderNode> path, MapObject obj, int idx = 0)
        {
            var pos = new Position(path[idx].X, path[idx].Y);
            var abx = pos.X - Client.Attributes.ServerPosition.X;
            var aby = pos.Y - Client.Attributes.ServerPosition.Y;

            if (abx == (idx + 1) && aby == 0)
                GameActions.Walk(Client, Direction.East);
            else if (abx == 0 && aby == -(idx + 1))
                GameActions.Walk(Client, Direction.North);
            else if (abx == -(idx + 1) && aby == 0)
                GameActions.Walk(Client, Direction.West);
            else if (abx == 0 && aby == (idx + 1))
                GameActions.Walk(Client, Direction.South);
        }

        private void ComputeStep(List<PathFinding.PathSolver.PathFinderNode> path, MapObject obj)
        {
            var pos = new Position(path[Distance].X, path[Distance].Y);
            var abx = pos.X - Client.Attributes.ServerPosition.X;
            var aby = pos.Y - Client.Attributes.ServerPosition.Y;

            if (abx == Distance && aby == 0)
                GameActions.Walk(Client, Direction.East);
            else if ((abx == 0 && aby == -Distance)
                || (abx == 1 && aby == -1)
                || (abx == -1 && aby == -1))
                GameActions.Walk(Client, Direction.North);
            else if (abx == -Distance && aby == 0)
                GameActions.Walk(Client, Direction.West);
            else if ((abx == 0 && aby == Distance)
                || (abx == 1 && aby == 1)
                || (abx == -1 && aby == 1))
                GameActions.Walk(Client, Direction.South);
        }
    }
}
