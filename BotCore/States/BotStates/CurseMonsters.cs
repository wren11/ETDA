using BotCore.States.BotStates;
using BotCore.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BotCore.States
{
    [StateAttribute(Author: "Dean", Desc: "Will ard cradh if monster is within the radius of 9")]
    public class CurseMonsters : GameState
    {
        public List<MapObject> Targets = new List<MapObject>();

        private string m_curse = "ard cradh"; //by default we will assume we have ard cradh
        [Description("What Curse will we use?"), Category("Curse Spell Used")]
        public string UsingCurse
        {
            get { return m_curse; }
            set { m_curse = value; }
        }

        private bool m_HavePathRequired;
        [Description("Curse Only if we have a path to it."), Category("Curse Conditions")]
        public bool DontCurseInvalidPath
        {
            get { return m_HavePathRequired; }
            set { m_HavePathRequired = value; }
        }

        private int m_CastingDistance = 9;
        [Description("Curse Only if we target is within X tiles"), Category("Casting Conditions")]
        public int CastingDistance
        {
            get { return m_CastingDistance; }
            set { m_CastingDistance = value; }
        }

        public override bool NeedToRun
        {
            get
            {
                var objects = Client.ObjectSearcher.RetrieveMonsterTargets(i => Client
                .Attributes.ServerPosition.DistanceFrom(i.ServerPosition) < m_CastingDistance);

                foreach (var obj in objects)
                {
                    if (obj.CurseInfo != null
                        && obj.CurseInfo.CurseElapsed)
                        obj.CurseInfo = null;
                }

                
                    var copy = new List<MapObject>();
                    lock (objects)
                {
                    //we copy memory here deliberatly!
                    copy = new List<MapObject>(objects);
                    Targets = new List<MapObject>(copy.Where(i => i.CurseInfo == null).OrderBy
                        (i => Client.Attributes.ServerPosition.DistanceFrom(i.ServerPosition)));
                    if (m_HavePathRequired)
                        Targets = Targets.Where(i => i.PathToMapObject != null && i.PathToMapObject.Count > 0).ToList();

                    if (Targets.Count > 0)
                        return true;
                }

                return false;
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

                foreach (var obj in Targets)
                {
                    var ReCurse = (obj.CurseInfo != null && obj.CurseInfo.CurseElapsed);
                    if (ReCurse)
                        obj.CurseInfo = null;

                    if (obj.CurseInfo == null)
                    {
                        Client.Utilities.CastSpell(m_curse, obj);
                        break;
                    }
                }

                Client.TransitionTo(this, Elapsed);
            }
        }
    }
}
