using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotCore.States
{
    class CheckIfFas : GameState
    {
        public override bool NeedToRun
        {
            get
            {
                return (Client.SpellBar.Contains(119));
            }
        }

        public override int Priority
        {
            get
            {
                return 103;
            }
        }

        public override void Run(TimeSpan Elapsed)
        {
            //refas when fas runs
        }
    }
}
