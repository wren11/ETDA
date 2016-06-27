using BotCore.States.BotStates;
using BotCore.Types;
using System;

namespace BotCore.States
{
    [StateAttribute(Author: "Dean", Desc: "This was created to heal.")]
    [StateMetaInfo(Version: "1.0", DateUpdated: "10/04/2016")]
    public class CheckAttributes : GameState
    {
        public bool use_fas_spiorad { get; set; }

        public override bool NeedToRun
        {
            get
            {
                if (Client.SpellBar.Contains(26))
                {
                    return false;
                }
                if (!use_fas_spiorad)
                {
                    return Client.Attributes.CurrentHP() < Client.Attributes.MaximumHP() * 0.4;
                }
                else
                {
                    if (Client.Attributes.CurrentHP() > Client.Attributes.MaximumHP() * 0.4
                        && Client.Attributes.CurrentMP() < Client.Attributes.MaximumMP() * 0.2
                        && !Client.SpellBar.Contains((short)SpellBar.slan))
                    {
                        return true;
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

                if (use_fas_spiorad)
                    Client.Utilities.CastSpell("fas spiorad", Client as Client);
                else
                {
                    Client.Utilities.CastSpell("ard ioc", Client as Client);
                }
                Client.TransitionTo(this, Elapsed);
            }

        }
    }
}
