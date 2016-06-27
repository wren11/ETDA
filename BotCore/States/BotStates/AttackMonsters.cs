using BotCore.States.BotStates;
using BotCore.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BotCore.States
{
    [StateAttribute(Author: "Dean", Desc: "Will attack monsters within the radius of 9")]
    public class AttackMonsters : GameState
    {
        public List<MapObject> Targets = new List<MapObject>();

        private string m_nocurseattack = "mor pian na dion"; 
        [Description("Attack With"), Category("If Not Cursed")]
        public string NoCurseAttack
        {
            get { return m_nocurseattack; }
            set { m_nocurseattack = value; }
        }

        private string m_attack = "Keeter 12";
        [Description("Attack With"), Category("If Cursed")]
        public string Attack
        {
            get { return m_attack; }
            set { m_attack = value; }
        }


        private bool m_HavePathRequired;
        [Description("Attack Only if we have a path to it."), Category("Curse Conditions")]
        public bool DontCurseInvalidPath
        {
            get { return m_HavePathRequired; }
            set { m_HavePathRequired = value; }
        }

        private int m_CastingDistance = 9;
        [Description("Attack Only if we target is within X tiles"), Category("Casting Conditions")]
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
                    Targets = new List<MapObject>(copy.OrderBy
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
                    var Cursed = (obj.CurseInfo != null && !obj.CurseInfo.CurseElapsed);

                    Client?.Utilities?.CastSpell(Cursed ? m_attack : m_nocurseattack, obj);
                    break;
                }

                Client.TransitionTo(this, Elapsed);
            }
        }
    }
}
