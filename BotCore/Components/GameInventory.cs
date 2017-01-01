using BotCore.Shared.Memory;
using BotCore.Types;
using System;
using System.Collections.Generic;

namespace BotCore.Components
{
    public class Inventory : UpdateableComponent
    {
        private InventoryItem[] _items = new InventoryItem[59];

        public InventoryItem[] Items
        {
            get
            {
                var copy = new List<InventoryItem>();
                lock (_items)
                {
                    copy = new List<InventoryItem>(_items);
                }
                return copy.ToArray();
            }
        }

        public Inventory()
        {
            Timer = new UpdateTimer(TimeSpan.FromMilliseconds(1.0));
        }

        public override void Update(TimeSpan tick)
        {
            Timer.Update(tick);

            if (Timer.Elapsed)
            {
                Timer.Reset();

                if (!IsInGame())
                    return;
                if (_memory == null || !_memory.IsRunning)
                    return;

                Pulse();
            }
        }

        public InventoryItem this[byte slot]
        {
            get { return _items[slot]; }
            set { _items[slot] = value; }
        }

        public InventoryItem GetItem(byte slot)
        {
            return this[slot];
        }

        
        public override void Pulse()
        {
            if (!IsInGame())
                return;

            var inventoryptr = _memory.Read<int>((IntPtr)_memory.Read<int>((IntPtr)DAStaticPointers.ObjectBase, false) + 0x2CC, false) + 0x1092;
            inventoryptr += 0x05;

            _items = new InventoryItem[59];

            for (int i = 0; i < 59; i++)
            {
                var val = _memory.ReadString((IntPtr)inventoryptr, false, 256);
                if (!string.IsNullOrWhiteSpace(val))
                    _items[i] = new InventoryItem(val, (byte)i);
                else
                    _items[i] = null;

                inventoryptr += 0x10B - 0x05;
            }
            base.Pulse();
        }
    }
}
