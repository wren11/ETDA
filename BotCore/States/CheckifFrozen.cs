using BotCore.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotCore.States
{
    class CheckifFrozen : GameState
    {

        public override int Priority
        {
            get
            {
                return 104;
            }
        }

        public override bool NeedToRun
        {
            get
            {
                return (Client.SpellBar.Contains(90) || Client.SpellBar.Contains(0x61));
            }
        }

        public override void Run(TimeSpan Elapsed)
        {
            //do something properly when we are frozen
            Console.WriteLine("I'm Frozen");
            Client.TransitionTo(this);
     
        }
    }
}
