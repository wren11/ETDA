using BotCore.States.BotStates;
using System;

namespace BotCore.States
{
    [Serializable]
    [StateAttribute(Author: "Jimmy", Desc: "Will try to keep beag cradh on")]
    public class CheckIfBC : GameState
    {
        public override bool NeedToRun
        {
            get
            {
                if ((Client.SpellBar.Contains(83)))
                {
                    return false;
                }
                else if ((Client.SpellBar.Contains(84)))
                {
                    return false;
                }
                else if ((Client.SpellBar.Contains(133)))
                {
                    return false;
                }
                else if ((!Client.SpellBar.Contains(5)))
                {
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
                Client.Utilities.CastSpell("beag cradh", Client as Client);
                Client.TransitionTo(this, Elapsed);
            }
        }
    }
}