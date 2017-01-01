using BotCore.States.BotStates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;

namespace BotCore.States
{

    public class DebuffCollection : CollectionBase
    {
        public RemoveDebuff.Debuff this[int index]
        {
            get { return (RemoveDebuff.Debuff)List[index]; }
        }
        public void Add(RemoveDebuff.Debuff emp)
        {
            List.Add(emp);
        }
        public void Remove(RemoveDebuff.Debuff emp)
        {
            List.Remove(emp);
        }
    }

    public class DebuffCollectionEditor : CollectionEditor
    {
        public DebuffCollectionEditor(Type type)
            : base(type)
        {
        }

        protected override string GetDisplayText(object value)
        {
            RemoveDebuff.Debuff item = new RemoveDebuff.Debuff();
            item = (RemoveDebuff.Debuff)value;

            return base.GetDisplayText(string.Format("{0}, {1}", item.Icon,
                item.Name));
        }
    }

    [StateAttribute(Author: "Huy", Desc: "Will remove debuffs")]
    public class RemoveDebuff : GameState
    {
        [Editor(typeof(DebuffCollectionEditor),
        typeof(System.Drawing.Design.UITypeEditor))]
        [Category("Add/Remove Debuffs")]
        [DisplayName("Debuffs")]
        [Description("A collection of debuffs the bot will remove.")]
        public List<Debuff> Debuffs { get; set; }

        string m_spell;

        public class Debuff
        {
            [Category("Debuff")]
            [DisplayName("Debuff Icon")]
            [Description("The Icon to Add.")]
            public short Icon { get; set; }

            [Category("Debuff")]
            [DisplayName("Debuff Name")]
            [Description("The Name of the spell that will remove the debuff.")]
            public string Name { get; set; }
        }

        public override void InitState()
        {
            Debuffs = new List<Debuff>();
            Debuffs.Add(new Debuff() { Icon = 50, Name = "ao suain" });
            Debuffs.Add(new Debuff() { Icon = 35, Name = "ao puinsein" });
            Debuffs.Add(new Debuff() { Icon = 84, Name = "ao ard cradh" });
            Debuffs.Add(new Debuff() { Icon = 83, Name = "ao mor cradh" });
            Debuffs.Add(new Debuff() { Icon = 82, Name = "ao cradh" });
            Debuffs.Add(new Debuff() { Icon = 03, Name = "ao dall" });
            Debuffs.Add(new Debuff() { Icon = 05, Name = "ao beag cradh" });

        }

        public override bool NeedToRun
        {
            get
            {
                Client.Active.Pulse();

                if (InTransition)
                    return false;

                if (Client.SpellBar.Contains(89))
                {
                    return false;
                }
                else
                {
                    for (int i = 0; i < Debuffs.Count; i++)
                    {
                        if (Client.SpellBar.Contains(Debuffs[i].Icon))
                        {
                            m_spell = Debuffs[i].Name;
                            return true;
                        }
                    }
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

                if (!string.IsNullOrWhiteSpace(m_spell) && Client.Utilities.HaveSpell(m_spell))
                {
                    Client.Utilities.CastSpell(m_spell, Client as Client);
                    NeedToRun = false;
                }
                Client.TransitionTo(this, Elapsed);
            }
        }
    }
}
