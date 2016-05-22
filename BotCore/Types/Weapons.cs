using System.Collections.Generic;

namespace BotCore.Types
{
    public class Weapons : Dictionary<string, Weapons.WeaponTypes>
    {
        public enum WeaponClass
        {

        }

        public enum WeaponTypes
        {
            Warrior1h,
            Warrior2h,
            Rogue1h,
            RogueBow,
            Monk,
            Peasant,
            WarShield,
            Shield,
            Fishing,
            ParadiseFishing,
            YuleLog,
            Leprechaun,
            Insect
        }

        public Weapons() : base()
        {
            //Warrior1h
            this["Yowien Hatchet"] = WeaponTypes.Warrior1h;
            this["Crystal Blade"] = WeaponTypes.Warrior1h;
            this["Eclipse"] = WeaponTypes.Warrior1h;
            this["Master Falcata"] = WeaponTypes.Warrior1h;
            this["Master Saber"] = WeaponTypes.Warrior1h;

            //Warrior2h
            this["Empowered Hwarone Guandao"] = WeaponTypes.Warrior2h;
            this["Hwarone Guandao"] = WeaponTypes.Warrior2h;
            this["Andor Saber"] = WeaponTypes.Warrior2h;
            this["Blazed Veltain Sword"] = WeaponTypes.Warrior2h;
            this["Master Battle Axe"] = WeaponTypes.Warrior2h;
            this["Hy-brasyl Escalon"] = WeaponTypes.Warrior2h;
            this["Hy-brasyl Battle Axe"] = WeaponTypes.Warrior2h;

            //Rogue1h
            this["Andor Whip"] = WeaponTypes.Rogue1h;
            this["Master Kris"] = WeaponTypes.Rogue1h;
            this["Master Skean"] = WeaponTypes.Rogue1h;
            this["Scurvy Dagger"] = WeaponTypes.Rogue1h;
            this["Desert Skewer"] = WeaponTypes.Rogue1h;

            //RogueBow
            this["Empowered Yumi Bow"] = WeaponTypes.RogueBow;
            this["Yumi Bow"] = WeaponTypes.RogueBow;
            this["Andor Bow"] = WeaponTypes.RogueBow;
            this["Sen Bow"] = WeaponTypes.RogueBow;
            this["Jenwir Bow"] = WeaponTypes.RogueBow;
            this["Royal Bow"] = WeaponTypes.RogueBow;
            this["Wooden Bow"] = WeaponTypes.RogueBow;

            //Monk
            this["Yowien's Fist"] = WeaponTypes.Monk;
            this["Yowien's Claw"] = WeaponTypes.Monk;
            this["Obsidian"] = WeaponTypes.Monk;
            this["Nunchaku"] = WeaponTypes.Monk;
            this["Wolf Claw"] = WeaponTypes.Monk;

            //Peasant
            this["Stone Axe"] = WeaponTypes.Peasant;
            this["Scythe"] = WeaponTypes.Peasant;
            this["Wooden Club"] = WeaponTypes.Peasant;
            this["Dragon Scale Sword"] = WeaponTypes.Peasant;
            this["Hatchet"] = WeaponTypes.Peasant;

            //WarShield
            this["Great Yowien Shield"] = WeaponTypes.WarShield;
            this["Yowien Shield"] = WeaponTypes.WarShield;
            this["Talgonite Shield"] = WeaponTypes.WarShield;

            //Shield
            this["Andor Shield"] = WeaponTypes.Shield;
            this["Cursed Shield of Chadul"] = WeaponTypes.Shield;
            this["Captain's Shield"] = WeaponTypes.Shield;
            this["Cathonic Shield"] = WeaponTypes.Shield;

            //Fishing
            this["Fishing Rod"] = WeaponTypes.Fishing;

            //ParadiseFishing
            this["Island Fishing Rod"] = WeaponTypes.ParadiseFishing;

            //YuleLog
            this["Torbjorn's Axe"] = WeaponTypes.YuleLog;

            //Leprechaun
            this["Magical Net"] = WeaponTypes.Leprechaun;

            //Insect
            this["Insect Net"] = WeaponTypes.Insect;
        }
    }
}
