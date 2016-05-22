using System.Collections.Generic;

namespace BotCore.Types
{
    public class Spell
    {
        public string Name { get; set; }
        public byte TargetType { get; set; }
        public byte UnequipedLines { get; set; }
        public byte EquipedLines { get; set; }
        public byte Slot { get; set; }
        public StaffTable OptimalStaff { get; set; }
        public string CastName { get; set; }

        public Spell(string name, byte slot, byte targettype, byte baselines)
        {
            Name = name;
            Slot = slot;
            TargetType = targettype;
            UnequipedLines = baselines;
        }
    }
}