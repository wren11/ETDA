using BotCore.Types;
using System;
using System.Linq;

namespace BotCore.Components
{
    public class Magic : UpdateableComponent
    {
        private Spell[] spells = new Spell[90];

        public Spell[] Spells
        {
            get { return spells; }
            private set { spells = value; }
        }

        public Spell this[string SpellName]
        {
            get
            {
                return spells.FirstOrDefault(i => i != null
                && i.Name.StartsWith(SpellName));              
            }
        }

        public Magic()
        {
            Timer = new UpdateTimer(TimeSpan.FromMilliseconds(1.0));
        }



        public override void Update(TimeSpan tick)
        {
            Timer.Update(tick);

            if (Timer.Elapsed)
            {
                if (Client.Memory == null || !Client.Memory.IsRunning)
                    return;
                if (!IsInGame())
                    return;

                var ptr = Client.Memory.Read<int>((IntPtr)Client.Memory.Read<int>((IntPtr)0x00882E68, false) + 0x2CC, false) + 0x4DFA;
                ptr += 0x05;

                for (int i = 0; i < 90; i++)
                {
                    var val = Client.Memory.ReadString((IntPtr)ptr, false, 256);
                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        Spells[i] = new Spell(val, (byte)(i + 1), 0, 0);
                    }
                    else
                        Spells[i] = null;

                    ptr += 0x206;
                }

                Timer.Reset();
                base.Pulse();
            }
        }
    }
}
