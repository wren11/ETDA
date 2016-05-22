using System.Collections.Generic;


namespace BotCore.Types
{

    public class Item
    {

        public string classType;
        public int itemSlot;
        public int itemMod;
        public enum itemType
        {
            Weapon,
            Shield,
            Necklace,
            Belt,
            Armor
        }
        public enum itemMods
        {
            AllOneLines,
            Force1Lines,
            Minus1Lines,
            Minus2Lines,
            Minus3Lines,
            Minus4Lines,
            CurseZeroLines,
            WeaponSkills,
            MonkSkills,
            ArcherShots,
            LightEle,
            DarkEle
        }

        public Item()
        {
            classType = "Peasant";
        }

        public Item(string CharClass, itemType slot, itemMods itemMod)
        {
           
        }


    }

    public class Peasant : Dictionary<string, Item.itemMods>
    {
        
        public Peasant() : base()
        {
            this["Staff of Brilliance"] = Item.itemMods.Force1Lines;
            this["Staff of Ages"] = Item.itemMods.Force1Lines;
            this["Stone Axe"] = Item.itemMods.WeaponSkills;

            this["Lumen Amulet"] = Item.itemMods.LightEle;
            this["Thief's Dark Necklace"] = Item.itemMods.DarkEle;
        }
    }

    public class Warrior : Peasant
    {
        public Warrior() : base()
        { 
            this["Yowien Hachet"] = Item.itemMods.WeaponSkills;
        }
    }

    public class Rogue : Peasant
    {
        public Rogue() : base()
        {
            this["Empowered Yumi Bow"] = Item.itemMods.ArcherShots;
        }
    }

    public class Wizard : Peasant
    {
        public Wizard() : base()
        {
            this["Yowien Tree Staff"] = Item.itemMods.AllOneLines;
            this["Andor Staff"] = Item.itemMods.Minus2Lines;
        }
    }

    public class Priest : Peasant
    {
        public Priest() : base()
        { 
            this["Brute's Quill"] = Item.itemMods.AllOneLines;
            this["Holy Hy-brasyl Baton"] = Item.itemMods.AllOneLines;
            this["Assassin's Cross"] = Item.itemMods.Minus2Lines;
            this["Holy Diana"] = Item.itemMods.Minus2Lines;
        }
    }

    public class Monk : Peasant
    {
        public Monk() : base()
        {
            this["Yowien's Fist"] = Item.itemMods.MonkSkills;
        }
    }

}
