using BotCore.States.BotStates;
using System;

namespace BotCore.States
{
    [StateAttribute(Author: "Dean", Desc: "This was created to heal.")]
    [StateMetaInfo(Version: "1.0", DateUpdated: "10/04/2016")]
    public class CheckAttributes : GameState
    {
        public override bool NeedToRun
        {
            get
            {
                if (Client.SpellBar.Contains(26))
                { 
                    return false;
                }
                return Client.Attributes.CurrentHP() < Client.Attributes.MaximumHP() * 0.4;
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

                Client.Utilities.CastSpell("ard ioc", Client as Client);
                Client.TransitionTo(this, Elapsed);
            }

        }
    }
}
