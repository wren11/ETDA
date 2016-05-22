using BotCore.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotCore.States
{
    class CheckifAited : GameState
    {

        public override bool NeedToRun
        {
            get
            {
                return (Client.SpellBar.Contains(11));
            }
        }

        public override int Priority
        {
            get
            {
                return 102;
            }
        }

        public override void Run(TimeSpan Elapsed)
        {
            //reaite when aite runs
        }
    }
}
