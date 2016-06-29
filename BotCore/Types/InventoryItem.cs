using System;

namespace BotCore.Types
{
    public class InventoryItem
    {
        public string ItemName;
        public byte Slot;

        public InventoryItem(string itemName, byte slot)
        {
            ItemName = itemName;
            Slot = slot;
        }

    }
}
