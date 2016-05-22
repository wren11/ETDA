using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotCore.States
{
    class CheckifPosion : GameState
    {
        public override bool NeedToRun
        {
            get
            {
                return (Client.SpellBar.Contains(35));
            }
        }

        public override int Priority
        {
            get
            {
                return 106;
            }
        }

        public override void Run(TimeSpan Elapsed)
        {
            //Cast ao posion
        }
    }
}
