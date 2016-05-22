using System;

namespace BotCore.Components
{
    public class GameEquipment : UpdateableComponent
    {
        private EquipmentItems[] m_equipment = new EquipmentItems[18];

        public EquipmentItems[] Items
        {
            get { return m_equipment; }
            private set { m_equipment = value; }
        }

        public enum EquipmentSlot : byte
        {
            Weapon = 1,
            Armor = 2,
            Shield = 3,
            Helmet = 4,
            Earring = 5,
            Necklace = 6,
            LeftRing = 7,
            RightRing = 8,
            LeftGauntlet = 9,
            RightGauntlet = 10,
            Belt = 11,
            Greaves = 12,
            Boots = 13,
            Accessory1 = 14,
            Overcoat = 15,
            Hat = 16,
            Accessory2 = 17,
            Accessory3 = 18
        }

        public class EquipmentItems
        {
            public EquipmentSlot Slot;
            public string Name;

            public EquipmentItems(string name, EquipmentSlot slot)
            {
                Name = name;
                Slot = slot;
            }
        }

        public GameEquipment()
        {
            Timer = new UpdateTimer(TimeSpan.FromMilliseconds(50.0));
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

        //0045118C

        public string CurrentWeaponName()
        {
            if (_memory == null || !_memory.IsRunning)
                return string.Empty;

            if (!IsInGame())
                return string.Empty;

            var ptr = _memory.Read<int>((IntPtr)0x085118C, false);
            ptr += 0x588;
            ptr = _memory.Read<int>((IntPtr)ptr, false);
            ptr += 0x670;
            var result =  _memory.ReadString((IntPtr)ptr, false, 40);

            return result;
        }

        public override void Pulse()
        {
            if (!IsInGame())
                return;
            if (_memory == null || !_memory.IsRunning)
                return;

            Array.Clear(m_equipment, 0, m_equipment.Length);

            try
            {

                var x = CurrentWeaponName();
                var ptr = _memory.Read<int>((IntPtr)0x06FC914, false) + 0x1152;
                for (int i = 0; i < 12; i++)
                {
                    var val = _memory.ReadString((IntPtr)ptr, false, 256);
                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        m_equipment[i] = new EquipmentItems(val, (EquipmentSlot)i);
                    }
                    else
                        m_equipment[i] = null;
                    ptr += 0x80;
                }
            }
            catch
            {

            }
        }
    }
}
