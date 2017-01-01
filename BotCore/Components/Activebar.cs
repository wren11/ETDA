using BotCore.Interop;
using BotCore.Shared.Memory;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace BotCore.Components
{
    public class Activebar : UpdateableComponent
    {
        public int MemoryPointer { get; set; }
        private List<byte> m_active = new List<byte>();

        public List<byte> ActiveIcons
        {
            get
            {
                List<byte> copy;
                lock (m_active)
                    copy = new List<byte>(m_active);

                return copy;
            }
        }

        public Activebar()
        {
            Timer = new UpdateTimer(TimeSpan.FromMilliseconds(1.0));
            HardReset();
        }

        public void Reset()
        {
            if (Client != null)
            {
                m_active = new List<byte>();
                Client.SpellBar = new List<short>();
            }
        }

        public void HardReset()
        {
            if (Client == null || !Client.Memory.IsRunning)
                return;

            MemoryPointer = 0;
            Reset();
        }

        public override void Pulse()
        {

            if (!IsInGame())
                return;



            if (MemoryPointer == 0)
            {
                var FunctionSearch = new MemoryPatternSearcher(Client);
                var pointer = FunctionSearch.FindMemoryRegion((int)DAStaticPointers.ActiveBar);
                if (pointer != null && pointer > 0)
                    MemoryPointer = (int)pointer;
            }

            Reset();

            if (MemoryPointer > 0)
            {
                for (byte i = 0; i < 10; i++)
                {
                    var n = MemoryPointer + i * 0x02 + 0x190;
                    var Icon = Client.Memory.Read<byte>((IntPtr)n, false);

                    if (Icon < 255)
                    {
                        m_active.Add(Icon);
                    }
                }
            }

            var copy = new List<short>();
            lock (Client.SpellBar)
                copy = new List<short>(Client.SpellBar);

            foreach (var icon in copy)
            {
                if (!ActiveIcons.Contains((byte)icon))
                    Client.SpellBar.Remove((byte)icon);
            }
            foreach (var icon in ActiveIcons)
            {
                if (!Client.SpellBar.Contains(icon))
                    Client.SpellBar.Add(icon);
            }
            base.Pulse();
        }

        public override void Update(TimeSpan tick)
        {
            Timer.Update(tick);

            if (Timer.Elapsed)
            {
                Pulse();
                Timer.Reset();
            }
        }
    }
}
