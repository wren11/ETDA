using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotCore.States
{
    class CheckedifPramhed : GameState
    {
        public override bool NeedToRun
        {
            get
            {
                return (Client.SpellBar.Contains(90));
            }
        }

        public override int Priority
        {
            get
            {
                return 101;
            }
        }

        public override void Run(TimeSpan Elapsed)
        {
            //if pramhed, check if there's bard in grp. if true, bard use wake scroll
        }
    }
}
