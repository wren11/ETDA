using BotCore.States.BotStates;
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
                            //determine if should follow?
                            if (Client.Attributes?
                                .ServerPosition?
                                .DistanceFrom(m_target?.Client.Attributes.ServerPosition) > Distance
                                && m_target?.Client.FieldMap.MapNumber() == Client.FieldMap.MapNumber())
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

        public Random rnd = new Random();


        public override void Run(TimeSpan Elapsed)
        {
            if (Enabled && !InTransition)
            {
                InTransition = true;

                if (m_target != null)
                {
                    var path =
                        Client.FieldMap.Search(Client.Attributes.ServerPosition,
                        m_target.Client.Attributes?.ServerPosition);

                    if (path != null)
                    {
                        Client.Utilities.ComputeStep(path, Distance);
                    }
                }
            }
            Client.TransitionTo(this, Elapsed);
        }
    }
}
