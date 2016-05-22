using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotCore.States
{
    class CheckifSkulled : GameState
    {
        public override bool NeedToRun
        {
            get
            {
                return (Client.SpellBar.Contains(89));
            }
        }

        public override int Priority
        {
            get
            {
                return 105;
            }
        }

        public override void Run(TimeSpan Elapsed)
        {
            //if skulled wait for others to red or log off
        }
    }
}
