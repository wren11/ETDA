using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using BotCore.Types;
using BotCore.Components;
using BotCore.PathFinding;
using System.Net;

namespace BotCore
{
    public static class Collections
    {
        public static string DAPATH = Environment.CurrentDirectory;
        public static Form ParentForm { get; set; }

        public static Dictionary<int, Client> AttachedClients = new Dictionary<int, Client>();
        public static Dictionary<short, TargetCondition> TargetConditions = new Dictionary<short, TargetCondition>();
        public static Dictionary<string, BaseSpellInformation> BaseSpells = new Dictionary<string, BaseSpellInformation>();
        public static Dictionary<string, BaseSkillInformation> BaseSkills = new Dictionary<string, BaseSkillInformation>();
        public static Dictionary<string, StaffTable> BaseStaffs = new Dictionary<string, StaffTable>();
        public static List<string> RandomWords = new List<string>();

        static Collections()
        {
            LoadBaseStaves();
            LoadBaseSpells();
            LoadBaseSkills();
        }

        #region Skills
        private static void LoadBaseSkills()
        {
            BaseSkills["Assail"] = new BaseSkillInformation("Assail", "All", "Assail", 0);
            BaseSkills["Assault"] = new BaseSkillInformation("Assault", "Warrior", "Assail", 0);
            BaseSkills["Rescue"] = new BaseSkillInformation("Rescue", "Warrior", "none", 7);
            BaseSkills["Melee Lore"] = new BaseSkillInformation("Melee Lore", "Warrior", "Lore", 12);
            BaseSkills["ao beag suain"] = new BaseSkillInformation("ao beag suain", "Warrior", "none", 7);
            BaseSkills["Clobber"] = new BaseSkillInformation("Clobber", "Warrior", "Assail", 0);
            BaseSkills["Long Strike"] = new BaseSkillInformation("Long Strike", "Warrior", "Assail", 0);
            BaseSkills["Combat Senses"] = new BaseSkillInformation("Combat Senses", "Warrior", "Senses", 7);
            BaseSkills["Armor Lore"] = new BaseSkillInformation("Armor Lore", "Warrior", "Lore", 12);
            BaseSkills["Missile Lore"] = new BaseSkillInformation("Missile Lore", "Warrior", "Lore", 12);
            BaseSkills["Wind Blade"] = new BaseSkillInformation("Wind Blade", "Warrior", "blade", 7);
            BaseSkills["Mend Weapon"] = new BaseSkillInformation("Mend Weapon", "Warrior", "Mend", 225);
            BaseSkills["Tailor"] = new BaseSkillInformation("Tailor", "Warrior", "Mend", 475);
            BaseSkills["Wallop"] = new BaseSkillInformation("Wallop", "Warrior", "Assail", 0);
            BaseSkills["Two-handed Attack"] = new BaseSkillInformation("Two-handed Attack", "Warrior", "none", 0);
            BaseSkills["beag suain"] = new BaseSkillInformation("beag suain", "Warrior", "stun", 15);
            BaseSkills["Crasher"] = new BaseSkillInformation("Crasher", "Warrior", "Crasher", 28);
            BaseSkills["Charge"] = new BaseSkillInformation("Charge", "Warrior", "Charge", 30);
            BaseSkills["Trash"] = new BaseSkillInformation("Trash", "Warrior", "Assail", 0);
            BaseSkills["Cyclone Blade"] = new BaseSkillInformation("Cyclone Blade", "Warrior", "Blade", 15);
            BaseSkills["Mad Soul"] = new BaseSkillInformation("Mad Soul", "Warrior", "Purewar", 27);
            BaseSkills["Sacrifice"] = new BaseSkillInformation("Sacrifice", "Warrior", "Purewar", 27);
            BaseSkills["Perfect Defense"] = new BaseSkillInformation("Perfect Defense", "Warrior", "none", 22);
            BaseSkills["Sneak Attack"] = new BaseSkillInformation("Sneak Attack", "Warrior", "Ambush2", 11);
            BaseSkills["Paralyze Force"] = new BaseSkillInformation("Paralyze Force", "Warrior", "Stun", 24);
            BaseSkills["Sever"] = new BaseSkillInformation("Sever", "Warrior", "Blade", 37);
            BaseSkills["Auto Hemloch"] = new BaseSkillInformation("Auto Hemloch", "Warrior", "Hemloch", 45);
            BaseSkills["Frost Strike"] = new BaseSkillInformation("Frost Strike", "Warrior", "none", 3);
            BaseSkills["Ground Stomp"] = new BaseSkillInformation("Ground Stomp", "Warrior", "Stun", 27);
            BaseSkills["Lullaby Punch"] = new BaseSkillInformation("Lullaby Punch", "Warrior", "WFF", 20);
            BaseSkills["Cyclone Kick"] = new BaseSkillInformation("Cyclone Kick", "Warrior", "Blade", 15);
            BaseSkills["Spinning Kelberoth Strike"] = new BaseSkillInformation("Spinning Kelberoth Strike", "Warrior", "Kelb", 30);
            BaseSkills["Thrust Attack 1"] = new BaseSkillInformation("Thrust Attack 1", "Warrior", "TA", 0);
            BaseSkills["Thrust Attack 2"] = new BaseSkillInformation("Thrust Attack 2", "Warrior", "TA", 0);
            BaseSkills["Thrust Attack 3"] = new BaseSkillInformation("Thrust Attack 3", "Warrior", "TA", 0);
            BaseSkills["Thrust Attack 4"] = new BaseSkillInformation("Thrust Attack 4", "Warrior", "TA", 0);
            BaseSkills["Thrust Attack 5"] = new BaseSkillInformation("Thrust Attack 5", "Warrior", "TA", 0);
            BaseSkills["Thrust Attack 6"] = new BaseSkillInformation("Thrust Attack 6", "Warrior", "TA", 0);
            BaseSkills["Intimidate"] = new BaseSkillInformation("Intimidate", "Warrior", "none", 15);
            BaseSkills["Strikedown 1"] = new BaseSkillInformation("Strikedown 1", "Warrior", "Blade", 7);
            BaseSkills["Strikedown 2"] = new BaseSkillInformation("Strikedown 2", "Warrior", "Blade", 7);
            BaseSkills["Strikedown 3"] = new BaseSkillInformation("Strikedown 3", "Warrior", "Blade", 7);
            BaseSkills["Strikedown 4"] = new BaseSkillInformation("Strikedown 4", "Warrior", "Blade", 7);
            BaseSkills["Strikedown 5"] = new BaseSkillInformation("Strikedown 5", "Warrior", "Blade", 7);
            BaseSkills["Strikedown 6"] = new BaseSkillInformation("Strikedown 6", "Warrior", "Blade", 7);
            BaseSkills["Strikedown 7"] = new BaseSkillInformation("Strikedown 7", "Warrior", "Blade", 7);
            BaseSkills["Strikedown 8"] = new BaseSkillInformation("Strikedown 8", "Warrior", "Blade", 7);
            BaseSkills["Dune Swipe 1"] = new BaseSkillInformation("Dune Swipe 1", "Warrior", "Blade", 12);
            BaseSkills["Dune Swipe 2"] = new BaseSkillInformation("Dune Swipe 2", "Warrior", "Blade", 12);
            BaseSkills["Dune Swipe 3"] = new BaseSkillInformation("Dune Swipe 3", "Warrior", "Blade", 12);
            BaseSkills["Dune Swipe 4"] = new BaseSkillInformation("Dune Swipe 4", "Warrior", "Blade", 12);
            BaseSkills["Dune Swipe 5"] = new BaseSkillInformation("Dune Swipe 5", "Warrior", "Blade", 12);
            BaseSkills["Dune Swipe 6"] = new BaseSkillInformation("Dune Swipe 6", "Warrior", "Blade", 12);
            BaseSkills["Dune Swipe 7"] = new BaseSkillInformation("Dune Swipe 7", "Warrior", "Blade", 12);
            BaseSkills["Wheel Kick 1"] = new BaseSkillInformation("Wheel Kick 1", "Warrior", "Blade", 28);
            BaseSkills["Wheel Kick 2"] = new BaseSkillInformation("Wheel Kick 2", "Warrior", "Blade", 28);
            BaseSkills["Wheel Kick 3"] = new BaseSkillInformation("Wheel Kick 3", "Warrior", "Blade", 28);
            BaseSkills["Wheel Kick 4"] = new BaseSkillInformation("Wheel Kick 4", "Warrior", "Blade", 28);
            BaseSkills["Wheel Kick 5"] = new BaseSkillInformation("Wheel Kick 5", "Warrior", "Blade", 28);
            BaseSkills["Wheel Kick 6"] = new BaseSkillInformation("Wheel Kick 6", "Warrior", "Blade", 28);
            BaseSkills["Wheel Kick 7"] = new BaseSkillInformation("Wheel Kick 7", "Warrior", "Blade", 28);
            BaseSkills["Magic Lore"] = new BaseSkillInformation("Magic Lore", "Wizard", "Lore", 12);
            BaseSkills["Potion Lore"] = new BaseSkillInformation("Potion Lore", "Wizard", "Lore", 12);
            BaseSkills["Mend Staff"] = new BaseSkillInformation("Mend Staff", "Wizard", "Mend", 225);
            BaseSkills["Wield Staff"] = new BaseSkillInformation("Wield Staff", "Wizard", "none", 0);
            BaseSkills["Analyze Item"] = new BaseSkillInformation("Analyze Item", "Wizard", "ID", 6);
            BaseSkills["Herbal Lore"] = new BaseSkillInformation("Herbal Lore", "Wizard", "Lore", 36);
            BaseSkills["Elemental Bless 1"] = new BaseSkillInformation("Elemental Bless 1", "Wizard", "EL", 0);
            BaseSkills["Elemental Bless 2"] = new BaseSkillInformation("Elemental Bless 2", "Wizard", "EL", 0);
            BaseSkills["Elemental Bless 3"] = new BaseSkillInformation("Elemental Bless 3", "Wizard", "EL", 0);
            BaseSkills["Elemental Bless 4"] = new BaseSkillInformation("Elemental Bless 4", "Wizard", "EL", 0);
            BaseSkills["Elemental Bless 5"] = new BaseSkillInformation("Elemental Bless 5", "Wizard", "EL", 0);
            BaseSkills["Food Lore"] = new BaseSkillInformation("Food Lore", "Priest", "Lore", 12);
            BaseSkills["Transferblood"] = new BaseSkillInformation("Transferblood", "Priest", "none", 2);
            BaseSkills["Perish Lore"] = new BaseSkillInformation("Perish Lore", "Priest", "ID", 12);
            BaseSkills["Herbal Lore"] = new BaseSkillInformation("Herbal Lore", "Priest", "Lore", 36);
            BaseSkills["Mind Hymn"] = new BaseSkillInformation("Mind Hymn", "Priest", "none", 180);
            BaseSkills["Instrumental 1"] = new BaseSkillInformation("Instrumental 1", "Priest", "Instrumental", 0);
            BaseSkills["Instrumental 2"] = new BaseSkillInformation("Instrumental 2", "Priest", "Instrumental", 0);
            BaseSkills["Instrumental 3"] = new BaseSkillInformation("Instrumental 3", "Priest", "Instrumental", 0);
            BaseSkills["Instrumental 4"] = new BaseSkillInformation("Instrumental 4", "Priest", "Instrumental", 0);
            BaseSkills["Instrumental 5"] = new BaseSkillInformation("Instrumental 5", "Priest", "Instrumental", 0);
            BaseSkills["Instrumental 6"] = new BaseSkillInformation("Instrumental 6", "Priest", "Instrumental", 0);
            BaseSkills["Instrumental Attack 1"] = new BaseSkillInformation("Instrumental Attack 1", "Priest", "IA", 12);
            BaseSkills["Instrumental Attack 2"] = new BaseSkillInformation("Instrumental Attack 2", "Priest", "IA", 12);
            BaseSkills["Instrumental Attack 3"] = new BaseSkillInformation("Instrumental Attack 3", "Priest", "IA", 12);
            BaseSkills["Instrumental Attack 4"] = new BaseSkillInformation("Instrumental Attack 4", "Priest", "IA", 12);
            BaseSkills["Instrumental Attack 5"] = new BaseSkillInformation("Instrumental Attack 5", "Priest", "IA", 12);
            BaseSkills["Instrumental Attack 6"] = new BaseSkillInformation("Instrumental Attack 6", "Priest", "IA", 12);
            BaseSkills["Instrumental Attack 7"] = new BaseSkillInformation("Instrumental Attack 7", "Priest", "IA", 12);
            BaseSkills["Instrumental Attack 8"] = new BaseSkillInformation("Instrumental Attack 8", "Priest", "IA", 12);
            BaseSkills["Instrumental Attack 9"] = new BaseSkillInformation("Instrumental Attack 9", "Priest", "IA", 12);
            BaseSkills["Kick"] = new BaseSkillInformation("Kick", "Monk", "Kick", 9);
            BaseSkills["Throw"] = new BaseSkillInformation("Throw", "Monk", "none", 45);
            BaseSkills["Ambush"] = new BaseSkillInformation("Ambush", "Monk", "Ambush1", 7);
            BaseSkills["High Kick"] = new BaseSkillInformation("High Kick", "Monk", "Kick", 7);
            BaseSkills["Double Punch"] = new BaseSkillInformation("Double Punch", "Monk", "Assail", 0);
            BaseSkills["Poison Punch"] = new BaseSkillInformation("Poison Punch", "Monk", "none", 7);
            BaseSkills["Martial Awareness"] = new BaseSkillInformation("Martial Awareness", "Monk", "Senses", 7);
            BaseSkills["Mantis Kick"] = new BaseSkillInformation("Mantis Kick", "Monk", "Kick", 23);
            BaseSkills["Eagle Strike"] = new BaseSkillInformation("Eagle Strike", "Monk", "Blade", 20);
            BaseSkills["Claw Fist"] = new BaseSkillInformation("Claw Fist", "Monk", "none", 27);
            BaseSkills["Wise Touch"] = new BaseSkillInformation("Wise Touch", "Monk", "ID", 22);
            BaseSkills["Lucky Hand"] = new BaseSkillInformation("Lucky Hand", "Monk", "Mend", 75);
            BaseSkills["Wolf Fang Fist"] = new BaseSkillInformation("Wolf Fang Fist", "Monk", "WFF", 32);
            BaseSkills["Triple Kick"] = new BaseSkillInformation("Triple Kick", "Monk", "Assail", 0);
            BaseSkills["WhirlWind Attack"] = new BaseSkillInformation("WhirlWind Attack", "Monk", "none", 35);
            BaseSkills["Draco Tail Kick"] = new BaseSkillInformation("Draco Tail Kick", "Monk", "Kick", 23);
            BaseSkills["Kelberoth Strike"] = new BaseSkillInformation("Kelberoth Strike", "Monk", "Kelb", 23);
            BaseSkills["Dark Spear"] = new BaseSkillInformation("Dark Spear", "Monk", "none", 7);
            BaseSkills["Eco Sense"] = new BaseSkillInformation("Eco Sense", "Monk", "Senses", 7);
            BaseSkills["Sting"] = new BaseSkillInformation("Sting", "Monk", "none", 7);
            BaseSkills["Animal Roar"] = new BaseSkillInformation("Animal Roar", "Monk", "Stun", 27);
            BaseSkills["Animal Feast"] = new BaseSkillInformation("Animal Feast", "Monk", "Crasher", 28);
            BaseSkills["Raging Attack"] = new BaseSkillInformation("Raging Attack", "Monk", "Kick", 33);
            BaseSkills["Puncture"] = new BaseSkillInformation("Puncture", "Monk", "none", 20);
            BaseSkills["Inner Beast 1"] = new BaseSkillInformation("Inner Beast 1", "Monk", "IB", 0);
            BaseSkills["Inner Beast 2"] = new BaseSkillInformation("Inner Beast 2", "Monk", "IB", 0);
            BaseSkills["Inner Beast 3"] = new BaseSkillInformation("Inner Beast 3", "Monk", "IB", 0);
            BaseSkills["Inner Beast 4"] = new BaseSkillInformation("Inner Beast 4", "Monk", "IB", 0);
            BaseSkills["Inner Beast 5"] = new BaseSkillInformation("Inner Beast 5", "Monk", "IB", 0);
            BaseSkills["Inner Beast 6"] = new BaseSkillInformation("Inner Beast 6", "Monk", "IB", 0);
            BaseSkills["Mass Strike 1"] = new BaseSkillInformation("Mass Strike 1", "Monk", "Kick", 16);
            BaseSkills["Mass Strike 2"] = new BaseSkillInformation("Mass Strike 2", "Monk", "Kick", 16);
            BaseSkills["Mass Strike 3"] = new BaseSkillInformation("Mass Strike 3", "Monk", "Kick", 16);
            BaseSkills["Mass Strike 4"] = new BaseSkillInformation("Mass Strike 4", "Monk", "Kick", 16);
            BaseSkills["Mass Strike 5"] = new BaseSkillInformation("Mass Strike 5", "Monk", "Kick", 16);
            BaseSkills["Mass Strike 6"] = new BaseSkillInformation("Mass Strike 6", "Monk", "Kick", 16);
            BaseSkills["Mass Strike 7"] = new BaseSkillInformation("Mass Strike 7", "Monk", "Kick", 16);
            BaseSkills["Mass Strike 8"] = new BaseSkillInformation("Mass Strike 8", "Monk", "Kick", 16);
            BaseSkills["Mass Strike 9"] = new BaseSkillInformation("Mass Strike 9", "Monk", "Kick", 16);
            BaseSkills["Double Rake 1"] = new BaseSkillInformation("Double Rake 1", "Monk", "Kick", 14);
            BaseSkills["Double Rake 2"] = new BaseSkillInformation("Double Rake 2", "Monk", "Kick", 14);
            BaseSkills["Double Rake 3"] = new BaseSkillInformation("Double Rake 3", "Monk", "Kick", 14);
            BaseSkills["Double Rake 4"] = new BaseSkillInformation("Double Rake 4", "Monk", "Kick", 14);
            BaseSkills["Double Rake 5"] = new BaseSkillInformation("Double Rake 5", "Monk", "Kick", 14);
            BaseSkills["Double Rake 6"] = new BaseSkillInformation("Double Rake 6", "Monk", "Kick", 14);
            BaseSkills["Double Rake 7"] = new BaseSkillInformation("Double Rake 7", "Monk", "Kick", 14);
            BaseSkills["Pounce 1"] = new BaseSkillInformation("Pounce 1", "Monk", "none", 6);
            BaseSkills["Pounce 2"] = new BaseSkillInformation("Pounce 1", "Monk", "none", 6);
            BaseSkills["Pounce 3"] = new BaseSkillInformation("Pounce 1", "Monk", "none", 6);
            BaseSkills["Pounce 4"] = new BaseSkillInformation("Pounce 1", "Monk", "none", 6);
            BaseSkills["Pounce 5"] = new BaseSkillInformation("Pounce 1", "Monk", "none", 6);
            BaseSkills["Pounce 6"] = new BaseSkillInformation("Pounce 1", "Monk", "none", 6);
            BaseSkills["Pounce 7"] = new BaseSkillInformation("Pounce 1", "Monk", "none", 6);
            BaseSkills["Beak Pierce 1"] = new BaseSkillInformation("Beak Pierce 1", "Monk", "none", 13);
            BaseSkills["Beak Pierce 2"] = new BaseSkillInformation("Beak Pierce 2", "Monk", "none", 13);
            BaseSkills["Beak Pierce 3"] = new BaseSkillInformation("Beak Pierce 3", "Monk", "none", 13);
            BaseSkills["Beak Pierce 4"] = new BaseSkillInformation("Beak Pierce 4", "Monk", "none", 13);
            BaseSkills["Beak Pierce 5"] = new BaseSkillInformation("Beak Pierce 5", "Monk", "none", 13);
            BaseSkills["Beak Pierce 6"] = new BaseSkillInformation("Beak Pierce 6", "Monk", "none", 13);
            BaseSkills["Beak Pierce 7"] = new BaseSkillInformation("Beak Pierce 7", "Monk", "none", 13);
            BaseSkills["Beak Pierce 8"] = new BaseSkillInformation("Beak Pierce 8", "Monk", "none", 13);
            BaseSkills["Sneak Flight 1"] = new BaseSkillInformation("Sneak Flight 1", "Monk", "none", 13);
            BaseSkills["Sneak Flight 2"] = new BaseSkillInformation("Sneak Flight 2", "Monk", "none", 13);
            BaseSkills["Sneak Flight 3"] = new BaseSkillInformation("Sneak Flight 3", "Monk", "none", 13);
            BaseSkills["Sneak Flight 4"] = new BaseSkillInformation("Sneak Flight 4", "Monk", "none", 13);
            BaseSkills["Sneak Flight 5"] = new BaseSkillInformation("Sneak Flight 5", "Monk", "none", 13);
            BaseSkills["Sneak Flight 6"] = new BaseSkillInformation("Sneak Flight 6", "Monk", "none", 13);
            BaseSkills["Sneak Flight 7"] = new BaseSkillInformation("Sneak Flight 7", "Monk", "none", 13);
            BaseSkills["Talon Kick 1"] = new BaseSkillInformation("Talon Kick 1", "Monk", "Kick", 5);
            BaseSkills["Talon Kick 2"] = new BaseSkillInformation("Talon Kick 1", "Monk", "Kick", 5);
            BaseSkills["Talon Kick 3"] = new BaseSkillInformation("Talon Kick 1", "Monk", "Kick", 5);
            BaseSkills["Talon Kick 4"] = new BaseSkillInformation("Talon Kick 1", "Monk", "Kick", 5);
            BaseSkills["Talon Kick 5"] = new BaseSkillInformation("Talon Kick 1", "Monk", "Kick", 5);
            BaseSkills["Tail Slam"] = new BaseSkillInformation("Tail Slam", "Monk", "none", 20);
            BaseSkills["Tail Sweep 1"] = new BaseSkillInformation("Tail Sweep 1", "Monk", "Kick", 12);
            BaseSkills["Tail Sweep 2"] = new BaseSkillInformation("Tail Sweep 2", "Monk", "Kick", 12);
            BaseSkills["Tail Sweep 3"] = new BaseSkillInformation("Tail Sweep 3", "Monk", "Kick", 12);
            BaseSkills["Tail Sweep 4"] = new BaseSkillInformation("Tail Sweep 4", "Monk", "Kick", 12);
            BaseSkills["Tail Sweep 5"] = new BaseSkillInformation("Tail Sweep 5", "Monk", "Kick", 12);
            BaseSkills["Tail Sweep 6"] = new BaseSkillInformation("Tail Sweep 6", "Monk", "Kick", 12);
            BaseSkills["Tail Sweep 7"] = new BaseSkillInformation("Tail Sweep 7", "Monk", "Kick", 12);
            BaseSkills["Tail Sweep 8"] = new BaseSkillInformation("Tail Sweep 8", "Monk", "Kick", 12);
            BaseSkills["Tail Sweep 9"] = new BaseSkillInformation("Tail Sweep 9", "Monk", "Kick", 12);
            BaseSkills["Claw Slash 1"] = new BaseSkillInformation("Claw Slash 1", "Monk", "none", 12);
            BaseSkills["Claw Slash 2"] = new BaseSkillInformation("Claw Slash 2", "Monk", "none", 12);
            BaseSkills["Claw Slash 3"] = new BaseSkillInformation("Claw Slash 3", "Monk", "none", 12);
            BaseSkills["Claw Slash 4"] = new BaseSkillInformation("Claw Slash 4", "Monk", "none", 12);
            BaseSkills["Claw Slash 5"] = new BaseSkillInformation("Claw Slash 5", "Monk", "none", 12);
            BaseSkills["Claw Slash 6"] = new BaseSkillInformation("Claw Slash 6", "Monk", "none", 12);
            BaseSkills["Claw Slash 7"] = new BaseSkillInformation("Claw Slash 7", "Monk", "none", 12);
            BaseSkills["Venom Attack 1"] = new BaseSkillInformation("Venom Attack 1", "Monk", "none", 10);
            BaseSkills["Unlock"] = new BaseSkillInformation("Unlock", "Rogue", "none", 7);
            BaseSkills["Mend Soori"] = new BaseSkillInformation("Mend Soori", "Rogue", "Mend", 225);
            BaseSkills["Amnesia"] = new BaseSkillInformation("Amnesia", "Rogue", "none", 7);
            BaseSkills["Mend Garment"] = new BaseSkillInformation("Mend Garment", "Rogue", "Mend", 225);
            BaseSkills["Appraise"] = new BaseSkillInformation("Appraise", "Rogue", "ID", 12);
            BaseSkills["Study Creature"] = new BaseSkillInformation("Study Creature", "Rogue", "Senses", 7);
            BaseSkills["Throw Surigum"] = new BaseSkillInformation("Throw Surigum", "Rogue", "none", 0);
            BaseSkills["Hairstyle"] = new BaseSkillInformation("Hairstyle", "Rogue", "none", 0);
            BaseSkills["Throw Smoke Bomb"] = new BaseSkillInformation("Throw Smoke Bomb", "Rogue", "none", 7);
            BaseSkills["Stab and Twist"] = new BaseSkillInformation("Stab and Twist", "Rogue", "none", 0);
            BaseSkills["Evaluate"] = new BaseSkillInformation("Evaluate", "Rogue", "ID", 12);
            BaseSkills["Peek"] = new BaseSkillInformation("Peek", "Rogue", "none", 15);
            BaseSkills["Assassin Strike"] = new BaseSkillInformation("Assassin Strike", "Rogue", "none", 0);
            BaseSkills["Midnight Slash"] = new BaseSkillInformation("Midnight Slash", "Rogue", "Assail", 0);
            BaseSkills["Shadow Figure"] = new BaseSkillInformation("Shadow Figure", "Rogue", "Ambush2", 0);
            BaseSkills["Stab Twice"] = new BaseSkillInformation("Stab Twice", "Rogue", "none", 0);
            BaseSkills["Shadow Strike"] = new BaseSkillInformation("Shadow Strike", "Rogue", "Ambush2", 30);
            BaseSkills["Birthday Suit"] = new BaseSkillInformation("Birthday Suit", "Rogue", "none", 60);
            BaseSkills["Incapacitate"] = new BaseSkillInformation("Incapacitate", "Rogue", "none", 60);
            BaseSkills["Precision Shot"] = new BaseSkillInformation("Precision Shot", "Rogue", "Kelb", 30);
            BaseSkills["Special Arrow Attack"] = new BaseSkillInformation("Special Arrow Attack", "Rogue", "none", 0);
            BaseSkills["Archery 1"] = new BaseSkillInformation("Archer 1", "Rogue", "Archer", 0);
            BaseSkills["Archery 2"] = new BaseSkillInformation("Archer 2", "Rogue", "Archer", 0);
            BaseSkills["Archery 3"] = new BaseSkillInformation("Archer 3", "Rogue", "Archer", 0);
            BaseSkills["Archery 4"] = new BaseSkillInformation("Archer 4", "Rogue", "Archer", 0);
            BaseSkills["Archery 5"] = new BaseSkillInformation("Archer 5", "Rogue", "Archer", 0);
            BaseSkills["Archery 6"] = new BaseSkillInformation("Archer 6", "Rogue", "Archer", 0);
            BaseSkills["Arrow Shot 1"] = new BaseSkillInformation("Arrow Shot 1", "Rogue", "ArrowShot", 0);
            BaseSkills["Arrow Shot 2"] = new BaseSkillInformation("Arrow Shot 2", "Rogue", "ArrowShot", 0);
            BaseSkills["Arrow Shot 3"] = new BaseSkillInformation("Arrow Shot 3", "Rogue", "ArrowShot", 0);
            BaseSkills["Arrow Shot 4"] = new BaseSkillInformation("Arrow Shot 4", "Rogue", "ArrowShot", 0);
            BaseSkills["Arrow Shot 5"] = new BaseSkillInformation("Arrow Shot 5", "Rogue", "ArrowShot", 0);
            BaseSkills["Arrow Shot 6"] = new BaseSkillInformation("Arrow Shot 6", "Rogue", "ArrowShot", 0);
            BaseSkills["Arrow Shot 7"] = new BaseSkillInformation("Arrow Shot 7", "Rogue", "ArrowShot", 0);
            BaseSkills["Arrow Shot 8"] = new BaseSkillInformation("Arrow Shot 8", "Rogue", "ArrowShot", 0);
            BaseSkills["Arrow Shot 9"] = new BaseSkillInformation("Arrow Shot 9", "Rogue", "ArrowShot", 0);
            BaseSkills["Rear Strike 1"] = new BaseSkillInformation("Rear Strike 1", "Rogue", "RS", 15);
            BaseSkills["Rear Strike 2"] = new BaseSkillInformation("Rear Strike 2", "Rogue", "RS", 15);
            BaseSkills["Rear Strike 3"] = new BaseSkillInformation("Rear Strike 3", "Rogue", "RS", 15);
            BaseSkills["Rear Strike 4"] = new BaseSkillInformation("Rear Strike 4", "Rogue", "RS", 15);
            BaseSkills["Rear Strike 5"] = new BaseSkillInformation("Rear Strike 5", "Rogue", "RS", 15);
        }
        #endregion

        #region Staves
        private static void LoadBaseStaves()
        {
            BaseStaffs["Holy Apollo"] = new StaffTable() { Level = 19, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 2 } };
            BaseStaffs["Holy Diana"] = new StaffTable() { Level = 19, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Decrease, Scope = SpellScope.All, Value = 2 } };
            BaseStaffs["Holy Gaea"] = new StaffTable() { Level = 19, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 2 } };
            BaseStaffs["Holy Hermes"] = new StaffTable() { Level = 19, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 0, Name = "ioc" } };
            BaseStaffs["Holy Kronos"] = new StaffTable() { Level = 19, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Magus Apollo"] = new StaffTable() { Level = 19, Class = "Wizard", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 3 } };
            BaseStaffs["Magus Ares"] = new StaffTable() { Level = 19, Class = "Wizard", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1, Name = "cradh" } };
            BaseStaffs["Magus Diana"] = new StaffTable() { Level = 19, Class = "Wizard", Modifer = new SpellModifiers() { Action = ActionModifier.Decrease, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Magus Gaea"] = new StaffTable() { Level = 19, Class = "Wizard", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 4 } };
            BaseStaffs["Magus Kronos"] = new StaffTable() { Level = 19, Class = "Wizard", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Holy Zeus"] = new StaffTable() { Level = 19, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 2 } };
            BaseStaffs["Assassin's Cross"] = new StaffTable() { Id = 162, Level = 99, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Decrease, Scope = SpellScope.All, Value = 2 } };
            BaseStaffs["Andor Staff"] = new StaffTable() { Id = 153, Level = 99, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Decrease, Scope = SpellScope.All, Value = 2 } };
            BaseStaffs["Veltain Staff"] = new StaffTable() { Level = 99, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Decrease, Scope = SpellScope.All, Value = 2 } };
            BaseStaffs["Empowered Holy Gnarl"] = new StaffTable() { Id = 87, Level = 99, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.Single, Value = 0, Name = "ard cradh" } };
            BaseStaffs["Empowered Magus Orb"] = new StaffTable() { Level = 99, Class = "Wizard", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Enchanted Hy-Brasyl Gnarl"] = new StaffTable() { Level = 99, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.Single, Value = 1, Name = "ard cradh" } };
            BaseStaffs["Enchanted Magus Orb"] = new StaffTable() { Level = 99, Class = "Wizard", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Magus Hy-Brasyl Gnarl"] = new StaffTable() { Level = 99, Class = "Wizard", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 2 } };
            BaseStaffs["Magus Hy-Brasyl Orb"] = new StaffTable() { Level = 99, Class = "Wizard", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 2 } };
            BaseStaffs["Master Celestial Staff"] = new StaffTable() { Level = 99, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Staff of Ages"] = new StaffTable() { Level = 99, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Staff of Brilliance"] = new StaffTable() { Level = 99, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Sphere"] = new StaffTable() { Level = 99, Class = "Wizard", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Wooden Harp"] = new StaffTable() { Level = 99, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["ShaineSphere"] = new StaffTable() { Level = 99, Class = "Wizard", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Maron Sphere"] = new StaffTable() { Level = 99, Class = "Wizard", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Rosewood Harp"] = new StaffTable() { Level = 99, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Chernol Sphere"] = new StaffTable() { Level = 99, Class = "Wizard", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Ironwood Harp"] = new StaffTable() { Level = 99, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Hwarone Lute"] = new StaffTable() { Level = 99, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Serphant Sphere"] = new StaffTable() { Level = 99, Class = "Wizard", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Empowered Hwarone Lute"] = new StaffTable() { Level = 99, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Empowered Serphant Sphere"] = new StaffTable() { Level = 99, Class = "Wizard", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Glimmering Wand"] = new StaffTable() { Level = 99, Class = "Wizard", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Holy Hy-brasyl Baton"] = new StaffTable() { Id = 6, Level = 99, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            //BaseStaffs["Brute's Quill1"] = new StaffTable() { Id = 239, Level = 99, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Yowien Tree Staff"] = new StaffTable() { Id = 30, Level = 99, Class = "Wizard", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Goldberry Harp"] = new StaffTable() { Level = 99, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };
            BaseStaffs["Master Divine Staff"] = new StaffTable() { Id = 110, Level = 99, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Decrease, Scope = SpellScope.AllExcept, Name = "cradh", Value = 3 } };
            BaseStaffs["Divinities Staff"] = new StaffTable() { Id = 161, Level = 99, Class = "Bard", Modifer = new SpellModifiers() { Action = ActionModifier.Decrease, Scope = SpellScope.All, Value = 2 } };
            BaseStaffs["Dragon Emberwood Staff"] = new StaffTable() {Id = 11,  Level = 99, Class = "Priest", Modifer = new SpellModifiers() { Action = ActionModifier.Set, Scope = SpellScope.All, Value = 1 } };

        }
        #endregion

        #region Spells
        private static void LoadBaseSpells()
        {
            BaseSpells["ao ard cradh"] = new BaseSpellInformation("ao ard cradh", "Priest", "ao", 120, 1);
            BaseSpells["ao beag cradh"] = new BaseSpellInformation("ao beag cradh", "Priest", "ao", 30, 1);
            BaseSpells["ao cradh"] = new BaseSpellInformation("ao cradh", "Priest", "ao", 60, 1);
            BaseSpells["ao mor cradh"] = new BaseSpellInformation("ao mor cradh", "Priest", "ao", 90, 1);
            BaseSpells["ao dall"] = new BaseSpellInformation("ao dall", "All", "ao", 30, 1);
            BaseSpells["ao puinsein"] = new BaseSpellInformation("ao puinsein", "All", "ao", 30, 1);
            BaseSpells["ao suain"] = new BaseSpellInformation("ao suain", "Priest", "ao", 30, 1);
            BaseSpells["ard athar"] = new BaseSpellInformation("ard athar", "Wizard", "athar", 2530, 4);
            BaseSpells["ard athar nadur"] = new BaseSpellInformation("ard athar nadur", "Wizard", "athar", 5, 0);
            BaseSpells["ard creag nadur"] = new BaseSpellInformation("ard creag nadur", "Wizard", "creag nadur", 5, 0);
            BaseSpells["ard cradh"] = new BaseSpellInformation("ard cradh", "Priest", "cradh", 500, 3);
            BaseSpells["ard creag"] = new BaseSpellInformation("ard creag", "Wizard", "creag", 2530, 4);
            BaseSpells["ard fas nadur"] = new BaseSpellInformation("ard fas nadur", "Wizard", "Fas nadur", 5000, 4);
            BaseSpells["ard ioc"] = new BaseSpellInformation("ard ioc", "Priest", "ioc", 800, 2);
            BaseSpells["ard naomh aite"] = new BaseSpellInformation("ard naomh aite", "Priest", "aite", 600, 4);
            BaseSpells["ard puinneag spiorad"] = new BaseSpellInformation("ard puinneag spiorad", "Wizard", "puinneag spiorad", 5, 0);
            BaseSpells["ard sal"] = new BaseSpellInformation("ard sal", "Wizard", "sal", 2530, 4);
            BaseSpells["ard sal nadur"] = new BaseSpellInformation("ard sal nadur", "Wizard", "sal nadur", 5, 0);
            BaseSpells["ard srad"] = new BaseSpellInformation("ard srad", "Wizard", "srad", 2530, 4);
            BaseSpells["ard srad nadur"] = new BaseSpellInformation("ard srad nadur", "Wizard", "srad nadur", 5, 0);
            BaseSpells["armachd"] = new BaseSpellInformation("armachd", "Priest", "none", 30, 2);
            BaseSpells["athar"] = new BaseSpellInformation("athar", "Wizard", "athar", 200, 2);
            BaseSpells["athar gar"] = new BaseSpellInformation("athar gar", "Wizard", "athar", 8200, 4);
            BaseSpells["athar lamh"] = new BaseSpellInformation("athar lamh", "Wizard", "athar", 320, 1);
            BaseSpells["beag breisleich"] = new BaseSpellInformation("beag breisleich", "Priest", "breisleich", 110, 1);
            BaseSpells["beag cradh"] = new BaseSpellInformation("beag cradh", "Priest", "cradh", 60, 2);
            BaseSpells["beag fas nadur"] = new BaseSpellInformation("beag fas nadur", "Wizard", "fas nadur", 20, 2);
            BaseSpells["beag ioc"] = new BaseSpellInformation("beag ioc", "Priest", "ioc", 40, 1);
            BaseSpells["beag ioc comlha"] = new BaseSpellInformation("beag ioc comlha", "Priest", "ioc", 100, 1);
            BaseSpells["beag ioc fein"] = new BaseSpellInformation("beag ioc fein", "Priest", "ioc", 40, 1);
            BaseSpells["beag naomh aite"] = new BaseSpellInformation("beag naomh aite", "Priest", "aite", 100, 3);
            BaseSpells["beag nochd"] = new BaseSpellInformation("beag nochd", "Monk", "nochd", 720, 5);
            BaseSpells["beag pramh"] = new BaseSpellInformation("beag pramh", "Priest", "pramh", 200, 4);
            BaseSpells["beag puinneag spiorad"] = new BaseSpellInformation("beag puinneag spiorad", "Wizard", "puinneag spiorad", 500, 4);
            BaseSpells["beag puinsein"] = new BaseSpellInformation("beag puinsein", "Priest", "puinsein", 110, 4);
            BaseSpells["beag seun"] = new BaseSpellInformation("beag seun", "Priest", "seun", 100, 3);
            BaseSpells["beag slan"] = new BaseSpellInformation("beag slan", "Priest", "seun", 100, 3);
            BaseSpells["beannaich"] = new BaseSpellInformation("beannaich", "Priest", "beannaich", 30, 2);
            BaseSpells["breisleich"] = new BaseSpellInformation("breisleich", "Priest", "breisleich", 110, 1);
            BaseSpells["Bubble Block"] = new BaseSpellInformation("Bubble Block", "Wizard", "none", 700, 0, 22);
            BaseSpells["Bubble Shield"] = new BaseSpellInformation("Bubble Shield", "Wizard", "none", 100, 1, 30);
            BaseSpells["Cat's Hearing"] = new BaseSpellInformation("Cat's Hearing", "Monk", "none", 120, 5);
            BaseSpells["Counter Attack 1"] = new BaseSpellInformation("Counter Attack 1", "Priest", "CA", 1500, 2);
            BaseSpells["Counter Attack 2"] = new BaseSpellInformation("Counter Attack 2", "Priest", "CA", 1500, 2);
            BaseSpells["Counter Attack 3"] = new BaseSpellInformation("Counter Attack 3", "Priest", "CA", 1500, 2);
            BaseSpells["Counter Attack 4"] = new BaseSpellInformation("Counter Attack 4", "Priest", "CA", 2720, 2);
            BaseSpells["Counter Attack 5"] = new BaseSpellInformation("Counter Attack 5", "Priest", "CA", 3000, 2);
            BaseSpells["Counter Attack 6"] = new BaseSpellInformation("Counter Attack 6", "Priest", "CA", 3500, 2);
            BaseSpells["Counter Attack 7"] = new BaseSpellInformation("Counter Attack 7", "Priest", "CA", 4000, 2);
            BaseSpells["Counter Attack 8"] = new BaseSpellInformation("Counter Attack 8", "Priest", "CA", 4500, 2);
            BaseSpells["cradh"] = new BaseSpellInformation("cradh", "Priest", "cradh", 120, 3);
            BaseSpells["creag"] = new BaseSpellInformation("creag", "Wizard", "creag", 200, 2);
            BaseSpells["creag gar"] = new BaseSpellInformation("creag gar", "Wizard", "creag", 8200, 4);
            BaseSpells["creag lamh"] = new BaseSpellInformation("creag lamh", "Wizard", "creag", 320, 1);
            BaseSpells["creag neart"] = new BaseSpellInformation("creag neart", "Priest", "none", 30, 0);
            BaseSpells["Cursed Tune 1"] = new BaseSpellInformation("Cursed Tune 1", "Priest", "CT", 5160, 0, 10);
            BaseSpells["Cursed Tune 2"] = new BaseSpellInformation("Cursed Tune 2", "Priest", "CT", 5160, 0, 10);
            BaseSpells["Cursed Tune 3"] = new BaseSpellInformation("Cursed Tune 3", "Priest", "CT", 5160, 0, 10);
            BaseSpells["Cursed Tune 4"] = new BaseSpellInformation("Cursed Tune 4", "Priest", "CT", 5160, 0, 10);
            BaseSpells["Cursed Tune 5"] = new BaseSpellInformation("Cursed Tune 5", "Priest", "CT", 5160, 0, 10);
            BaseSpells["Cursed Tune 6"] = new BaseSpellInformation("Cursed Tune 6", "Priest", "CT", 5160, 0, 10);
            BaseSpells["Cursed Tune 7"] = new BaseSpellInformation("Cursed Tune 7", "Priest", "CT", 5160, 0, 10);
            BaseSpells["Cyclone"] = new BaseSpellInformation("Cyclone", "Wizard", "none", 1200, 0, 12);
            BaseSpells["dachaidh"] = new BaseSpellInformation("dachaidh", "Priest and wizard", "none", 30, 2);
            BaseSpells["dall"] = new BaseSpellInformation("dall", "Priest", "dall", 220, 4);
            BaseSpells["Dark Seal"] = new BaseSpellInformation("Dark Seal", "Priest", "Dark Seal", 450, 6);
            BaseSpells["Darker Seal"] = new BaseSpellInformation("Darker Seal", "Priest", "none", 450, 6);
            BaseSpells["deireas faileas"] = new BaseSpellInformation("deireas faileas", "Priest", "none", 980, 3);
            BaseSpells["deo lamh"] = new BaseSpellInformation("deo lamh", "Priest", "deo", 330, 1);
            BaseSpells["deo saighead"] = new BaseSpellInformation("deo saighead", "Priest", "deo", 240, 3);
            BaseSpells["deo searg"] = new BaseSpellInformation("deo searg", "Priest", "deo", 1800, 4);
            BaseSpells["deo searg gar"] = new BaseSpellInformation("deo searg gar", "Priest", "deo", 8000, 4);
            BaseSpells["dion"] = new BaseSpellInformation("dion", "Monk", "dion", 600, 0);
            BaseSpells["Disenchanter"] = new BaseSpellInformation("Disenchanter", "Wizard", "none", 3000, 4);
            BaseSpells["Draco Stance"] = new BaseSpellInformation("Draco Stance", "Monk", "dion", 200, 0);
            BaseSpells["eisd creutair"] = new BaseSpellInformation("eisd creutair", "Rogue", "none", 120, 0);
            BaseSpells["fas deireas"] = new BaseSpellInformation("fas deireas", "Priest", "none", 30, 1);
            BaseSpells["fas nadur"] = new BaseSpellInformation("fas nadur", "Wizard", "fas nadur", 80, 3);
            BaseSpells["fas spiorad"] = new BaseSpellInformation("fas spiorad", "Wizard", "none", 9, 0);
            BaseSpells["Firey Defender"] = new BaseSpellInformation("Firey Defender", "Wizard", "none", 3000, 4);
            BaseSpells["Frost Arrow 1"] = new BaseSpellInformation("Frost Arrow 1", "Rogue", "FA", 900, 1);
            BaseSpells["Frost Arrow 2"] = new BaseSpellInformation("Frost Arrow 2", "Rogue", "FA", 900, 1);
            BaseSpells["Frost Arrow 3"] = new BaseSpellInformation("Frost Arrow 3", "Rogue", "FA", 900, 1);
            BaseSpells["Frost Arrow 4"] = new BaseSpellInformation("Frost Arrow 4", "Rogue", "FA", 900, 1);
            BaseSpells["Frost Arrow 5"] = new BaseSpellInformation("Frost Arrow 5", "Rogue", "FA", 1000, 1);
            BaseSpells["Frost Arrow 6"] = new BaseSpellInformation("Frost Arrow 6", "Rogue", "FA", 1100, 1);
            BaseSpells["Gentle Touch"] = new BaseSpellInformation("Gentle Touch", "Monk", "none", 360, 1);
            BaseSpells["Great Blind Snare"] = new BaseSpellInformation("Great Blind Snare", "Rogue", "dall", 30, 0);
            BaseSpells["Great Poison Snare"] = new BaseSpellInformation("Great Poison Snare", "Rogue", "puinsein", 30, 0);
            BaseSpells["Great Sleep"] = new BaseSpellInformation("Great Sleep", "Rogue", "suain", 30, 0);
            BaseSpells["Groo 1"] = new BaseSpellInformation("Groo 1", "Wizard", "Groo", 2600, 1);
            BaseSpells["Groo 2"] = new BaseSpellInformation("Groo 2", "Wizard", "Groo", 2650, 1);
            BaseSpells["Groo 3"] = new BaseSpellInformation("Groo 3", "Wizard", "Groo", 2675, 1);
            BaseSpells["Groo 4"] = new BaseSpellInformation("Groo 4", "Wizard", "Groo", 2700, 2);
            BaseSpells["Groo 5"] = new BaseSpellInformation("Groo 5", "Wizard", "Groo", 2850, 2);
            BaseSpells["Groo 6"] = new BaseSpellInformation("Groo 6", "Wizard", "Groo", 2775, 2);
            BaseSpells["Groo 7"] = new BaseSpellInformation("Groo 7", "Wizard", "Groo", 2600, 2);
            BaseSpells["Groo 8"] = new BaseSpellInformation("Groo 8", "Wizard", "Groo", 2650, 2);
            BaseSpells["Groo 9"] = new BaseSpellInformation("Groo 9", "Wizard", "Groo", 2775, 2);
            BaseSpells["Groo 10"] = new BaseSpellInformation("Groo 10", "Wizard", "Groo", 2900, 2);
            BaseSpells["Groo 11"] = new BaseSpellInformation("Groo 11", "Wizard", "Groo", 2950, 2);
            BaseSpells["Groo 12"] = new BaseSpellInformation("Groo 12", "Wizard", "Groo", 2975, 2);
            BaseSpells["Hail of Feathers 1"] = new BaseSpellInformation("Hail of Feathers 1", "Monk", "HoF", 800, 0);
            BaseSpells["Hail of Feathers 2"] = new BaseSpellInformation("Hail of Feathers 2", "Monk", "HoF", 1000, 0);
            BaseSpells["Hail of Feathers 3"] = new BaseSpellInformation("Hail of Feathers 3", "Monk", "HoF", 1200, 0);
            BaseSpells["Hail of Feathers 4"] = new BaseSpellInformation("Hail of Feathers 4", "Monk", "HoF", 1400, 0);
            BaseSpells["Hail of Feathers 5"] = new BaseSpellInformation("Hail of Feathers 5", "Monk", "HoF", 1600, 0);
            BaseSpells["Hail of Feathers 6"] = new BaseSpellInformation("Hail of Feathers 6", "Monk", "HoF", 1800, 0);
            BaseSpells["Hail of Feathers 7"] = new BaseSpellInformation("Hail of Feathers 7", "Monk", "HoF", 2000, 0);
            BaseSpells["Hide"] = new BaseSpellInformation("Hide", "Rogue", "none", 200, 0);
            BaseSpells["Howl"] = new BaseSpellInformation("Howl", "Monk", "none", 360, 5);
            BaseSpells["Inner Fire"] = new BaseSpellInformation("Inner Fire", "Monk", "none", 360, 5);
            BaseSpells["Insult"] = new BaseSpellInformation("Insult", "Warrior", "none", 1, 2);
            BaseSpells["ioc"] = new BaseSpellInformation("ioc", "Priest", "ioc", 200, 1);
            BaseSpells["ioc comlha"] = new BaseSpellInformation("ioc comlha", "Priest", "ioc", 500, 2);
            BaseSpells["Iron Skin"] = new BaseSpellInformation("Iron Skin", "Monk", "dion", 250, 2);
            BaseSpells["Keeter 1"] = new BaseSpellInformation("Keeter 1", "Wizard", "Keeter", 2600, 1);
            BaseSpells["Keeter 2"] = new BaseSpellInformation("Keeter 2", "Wizard", "Keeter", 2650, 1);
            BaseSpells["Keeter 3"] = new BaseSpellInformation("Keeter 3", "Wizard", "Keeter", 2675, 1);
            BaseSpells["Keeter 4"] = new BaseSpellInformation("Keeter 4", "Wizard", "Keeter", 2700, 1);
            BaseSpells["Keeter 5"] = new BaseSpellInformation("Keeter 5", "Wizard", "Keeter", 2750, 1);
            BaseSpells["Keeter 6"] = new BaseSpellInformation("Keeter 6", "Wizard", "Keeter", 2775, 2);
            BaseSpells["Keeter 7"] = new BaseSpellInformation("Keeter 7", "Wizard", "Keeter", 2600, 2);
            BaseSpells["Keeter 8"] = new BaseSpellInformation("Keeter 8","Wizard", "Keeter", 2650, 2);
            BaseSpells["Keeter 9"] = new BaseSpellInformation("Keeter 9", "Wizard", "Keeter", 2775, 2);
            BaseSpells["Keeter 10"] = new BaseSpellInformation("Keeter 10", "Wizard", "Keeter", 2700, 2);
            BaseSpells["Keeter 11"] = new BaseSpellInformation("Keeter 11", "Wizard", "Keeter", 2750, 2);
            BaseSpells["Keeter 12"] = new BaseSpellInformation("Keeter 12", "Wizard", "Keeter", 2775, 2);
            BaseSpells["Kelberoth Stance"] = new BaseSpellInformation("Kelberoth Stance", "Monk", "none", 30, 0);
            BaseSpells["Lyliac Plant"] = new BaseSpellInformation("Lyliac Plant", "Wizard", "flower", 100, 0);
            BaseSpells["Lyliac Vineyard"] = new BaseSpellInformation("Lyliac Vineyard", "Wizard", "flower", 100, 0, 12);
            BaseSpells["Maiden Trap"] = new BaseSpellInformation("Maiden Trap", "Rogue", "none", 300, 7);
            BaseSpells["Mana Ward"] = new BaseSpellInformation("Mana Ward", "Wizard", "none", 4600, 0, 120);
            BaseSpells["Master Feral Form"] = new BaseSpellInformation("Master Feral Form", "Monk", "feral", 360, 0);
            BaseSpells["Master Karura Form"] = new BaseSpellInformation("Master Karura Form", "Monk", "karura", 360, 0);
            BaseSpells["Master Komodas Form"] = new BaseSpellInformation("Master Komodas Form", "Monk", "komodas", 360, 0);
            BaseSpells["Mermaid 1"] = new BaseSpellInformation("Mermaid 1", "Wizard", "Mermaid", 2600, 1);
            BaseSpells["Mermaid 2"] = new BaseSpellInformation("Mermaid 2", "Wizard", "Mermaid", 2650, 1);
            BaseSpells["Mermaid 3"] = new BaseSpellInformation("Mermaid 3", "Wizard", "Mermaid", 2675, 1);
            BaseSpells["Mermaid 4"] = new BaseSpellInformation("Mermaid 4", "Wizard", "Mermaid", 2700, 1);
            BaseSpells["Mermaid 5"] = new BaseSpellInformation("Mermaid 5", "Wizard", "Mermaid", 2750, 1);
            BaseSpells["Mermaid 6"] = new BaseSpellInformation("Mermaid 6", "Wizard", "Mermaid", 2775, 2);
            BaseSpells["Mermaid 7"] = new BaseSpellInformation("Mermaid 7", "Wizard", "Mermaid", 2600, 2);
            BaseSpells["Mermaid 8"] = new BaseSpellInformation("Mermaid 8", "Wizard", "Mermaid", 2650, 2);
            BaseSpells["Mermaid 9"] = new BaseSpellInformation("Mermaid 9", "Wizard", "Mermaid", 2775, 2);
            BaseSpells["Mermaid 10"] = new BaseSpellInformation("Mermaid 10", "Wizard", "Mermaid", 2700, 2);
            BaseSpells["Mermaid 11"] = new BaseSpellInformation("Mermaid 11", "Wizard", "Mermaid", 2750, 2);
            BaseSpells["Mermaid 12"] = new BaseSpellInformation("Mermaid 12", "Wizard", "Mermaid", 2775, 2);
            BaseSpells["Mist"] = new BaseSpellInformation("Mist", "Monk", "none", 60, 0);
            BaseSpells["mor athar"] = new BaseSpellInformation("mor athar", "Wizard", "athar", 800, 4);
            BaseSpells["mor beannaich"] = new BaseSpellInformation("mor beannaich", "Priest", "beannaich", 90, 2);
            BaseSpells["mor breisleich"] = new BaseSpellInformation("mor breisleich", "Priest", "breisleich", 110, 1);
            BaseSpells["mor cradh"] = new BaseSpellInformation("mor cradh", "Priest", "cradh", 300, 3);
            BaseSpells["mor creag"] = new BaseSpellInformation("mor creag", "Wizard", "creag", 800, 4);
            BaseSpells["mor dion"] = new BaseSpellInformation("mor dion", "Priest", "dion", 1000, 2);
            BaseSpells["mor dion comlha"] = new BaseSpellInformation("mor dion comlha", "Priest", "dion", 300, 4);
            BaseSpells["mor fas nadur"] = new BaseSpellInformation("mor fas nadur", "Wizard", "fas nadur", 150, 4);
            BaseSpells["mor ioc"] = new BaseSpellInformation("mor ioc", "Priest", "ioc", 500, 2);
            BaseSpells["mor ioc comlha"] = new BaseSpellInformation("mor ioc comlha", "Priest", "ioc", 1100, 2);
            BaseSpells["mor naomh aite"] = new BaseSpellInformation("mor naomh aite", "Priest", "aite", 400, 4);
            BaseSpells["mor pian na dion"] = new BaseSpellInformation("mor pian na dion", "Wizard", "none", 3000, 1);
            BaseSpells["mor puinneag spiorad"] = new BaseSpellInformation("mor puinneag spiorad", "Wizard", "puinneag spiorad", 180, 1);
            BaseSpells["mor sal"] = new BaseSpellInformation("mor sal", "Wizard", "sal", 800, 4);
            BaseSpells["mor slan"] = new BaseSpellInformation("mor slan", "Priest", "slan", 500, 0);
            BaseSpells["mor srad"] = new BaseSpellInformation("mor srad", "Wizard", "srad", 800, 4);
            BaseSpells["mor strioch pian gar"] = new BaseSpellInformation("mor strioch pian gar", "Wizard", "strioch", 100, 1, 5);
            BaseSpells["mor strioch bais"] = new BaseSpellInformation("mor strioch bais", "Wizard", "strioch", 100, 4);
            BaseSpells["mor strioch bais lamh"] = new BaseSpellInformation("mor strioch bais lamh", "Wizard", "strioch", 0, 0);
            BaseSpells["mor strioch bais mealll"] = new BaseSpellInformation("mor strioch bais mealll", "Wizard", "strioch", 0, 0);
            BaseSpells["Mud Wall"] = new BaseSpellInformation("Mud Wall", "Wizard", "none", 700, 0, 22);
            BaseSpells["naomh aite"] = new BaseSpellInformation("naomh aite", "Priest", "aite", 200, 2);
            BaseSpells["nuadhaich"] = new BaseSpellInformation("nuadhaich", "Priest", "ioc", 1360, 2);
            BaseSpells["pian na dion"] = new BaseSpellInformation("pian na dion", "Wizard", "none", 1500, 1);
            BaseSpells["pramh"] = new BaseSpellInformation("pramh", "Priest", "pramh", 300, 4);
            BaseSpells["puinneag breatha"] = new BaseSpellInformation("puinneag breatha", "Monk", "none", 320, 0);
            BaseSpells["puinneag spiorad"] = new BaseSpellInformation("puinneag spiorad", "Priest", "puinneag spiorad", 320, 0);
            BaseSpells["puinsein"] = new BaseSpellInformation("puinsein", "Priest", "puinsein", 160, 3);
            BaseSpells["Reflection"] = new BaseSpellInformation("Reflection", "Priest", "none", 1080, 8);
            BaseSpells["Regeneration 1"] = new BaseSpellInformation("Regeneration 1", "Priest", "regen", 1220, 0);
            BaseSpells["Regeneration 2"] = new BaseSpellInformation("Regeneration 2", "Priest", "regen", 1520, 0);
            BaseSpells["Regeneration 3"] = new BaseSpellInformation("Regeneration 3", "Priest", "Regen", 2200, 0);
            BaseSpells["Regeneration 4"] = new BaseSpellInformation("Regeneration 4", "Priest", "Regen", 2720, 0);
            BaseSpells["Regeneration 5"] = new BaseSpellInformation("Regeneration 5", "Priest", "Regen", 3000, 0);
            BaseSpells["Regeneration 6"] = new BaseSpellInformation("Regeneration 6", "Priest", "Regen", 4020, 0);
            BaseSpells["Regeneration 7"] = new BaseSpellInformation("Regeneration 7", "Priest", "Regen", 5000, 0);
            BaseSpells["sal"] = new BaseSpellInformation("sal", "Wizard", "sal", 200, 2);
            BaseSpells["sal gar"] = new BaseSpellInformation("sal gar", "Wizard", "sal", 8200, 4);
            BaseSpells["sal lamh"] = new BaseSpellInformation("sal lamh", "Wizard", "sal", 320, 1);
            BaseSpells["seun"] = new BaseSpellInformation("seun", "Priest", "seun", 200, 2);
            BaseSpells["Shock Arrow"] = new BaseSpellInformation("Shock Arrow", "Rogue", "none", 710, 0);
            BaseSpells["slan"] = new BaseSpellInformation("slan", "Priest", "slan", 350, 0);
            BaseSpells["Snort"] = new BaseSpellInformation("Snort", "Monk", "none", 1, 1);
            BaseSpells["sonruich nadur"] = new BaseSpellInformation("sonruich nadur", "Priest", "sonruich nadur", 450, 5);
            BaseSpells["spion beathach"] = new BaseSpellInformation("spion beathach", "Priest", "none", 2800, 4);
            BaseSpells["spit"] = new BaseSpellInformation("spit", "Warrior", "none", 1, 1);
            BaseSpells["srad"] = new BaseSpellInformation("srad", "Wizard", "srad", 200, 2);
            BaseSpells["srad gar"] = new BaseSpellInformation("srad gar", "Wizard", "srad", 8200, 4);
            BaseSpells["srad lamh"] = new BaseSpellInformation("srad lamh", "Wizard", "srad", 320, 1);
            BaseSpells["Star Arrow 1"] = new BaseSpellInformation("Star Arrow 1", "Rogue", "SA", 400, 0);
            BaseSpells["Star Arrow 2"] = new BaseSpellInformation("Star Arrow 2", "Rogue", "SA", 500, 0);
            BaseSpells["Star Arrow 3"] = new BaseSpellInformation("Star Arrow 3", "Rogue", "SA", 600, 0);
            BaseSpells["Star Arrow 4"] = new BaseSpellInformation("Star Arrow 4", "Rogue", "SA", 700, 0);
            BaseSpells["Star Arrow 5"] = new BaseSpellInformation("Star Arrow 5", "Rogue", "SA", 800, 0);
            BaseSpells["Star Arrow 6"] = new BaseSpellInformation("Star Arrow 6", "Rogue", "SA", 900, 0);
            BaseSpells["Star Arrow 7"] = new BaseSpellInformation("Star Arrow 7", "Rogue", "SA", 1000, 0);
            BaseSpells["Star Arrow 8"] = new BaseSpellInformation("Star Arrow 8", "Rogue", "SA", 1100, 0);
            BaseSpells["Stone Skin"] = new BaseSpellInformation("Stone Skin", "All", "none", 1100, 0);
            BaseSpells["suain"] = new BaseSpellInformation("suain", "Priest", "suain", 330, 4);
            BaseSpells["taunt"] = new BaseSpellInformation("taunt", "Warrior", "none", 1, 4);
            BaseSpells["Torch 1"] = new BaseSpellInformation("Torch 1", "Wizard", "Torch", 2600, 1);
            BaseSpells["Torch 2"] = new BaseSpellInformation("Torch 2", "Wizard", "Torch", 2650, 1);
            BaseSpells["Torch 3"] = new BaseSpellInformation("Torch 3", "Wizard", "Torch", 2675, 1);
            BaseSpells["Torch 4"] = new BaseSpellInformation("Torch 4", "Wizard", "Torch", 2700, 1);
            BaseSpells["Torch 5"] = new BaseSpellInformation("Torch 5", "Wizard", "Torch", 2750, 1);
            BaseSpells["Torch 6"] = new BaseSpellInformation("Torch 6", "Wizard", "Torch", 2775, 2);
            BaseSpells["Torch 7"] = new BaseSpellInformation("Torch 7", "Wizard", "Torch", 2600, 2);
            BaseSpells["Torch 8"] = new BaseSpellInformation("Torch 8", "Wizard", "Torch", 2650, 2);
            BaseSpells["Torch 9"] = new BaseSpellInformation("Torch 9", "Wizard", "Torch", 2775, 2);
            BaseSpells["Torch 10"] = new BaseSpellInformation("Torch 10", "Wizard", "Torch", 2700, 2);
            BaseSpells["Torch 11"] = new BaseSpellInformation("Torch 11", "Wizard", "Torch", 2750, 2);
            BaseSpells["Torch 12"] = new BaseSpellInformation("Torch 12", "Wizard", "Torch", 2775, 2);
            BaseSpells["White Bat Stance"] = new BaseSpellInformation("White Bat Stance", "Monk", "none", 110, 0);
            BaseSpells["Wild Feral Form"] = new BaseSpellInformation("Wild Feral Form", "Monk", "feral", 360, 0);
            BaseSpells["Wild Komodas Form"] = new BaseSpellInformation("Wild Komodas Form", "Monk", "komodas", 360, 0);
            BaseSpells["Wild Karura Form"] = new BaseSpellInformation("Wild Karura Form", "Monk", "karura", 360, 0);
            BaseSpells["Wings of Protection"] = new BaseSpellInformation("Wings Of Protection", "Wizard", "dion", 900, 0);
            BaseSpells["Wraith Touch"] = new BaseSpellInformation("Wraith Touch", "Monk", "none", 1420, 4);
            BaseSpells["Zombie Defender"] = new BaseSpellInformation("Zombie Defender", "All", "none", 3000, 4);
            BaseSpells["Fierce Karura Form"] = new BaseSpellInformation("Fierce Karura Form", "Monk", "kaura", 360, 0);
            BaseSpells["Karura Form"] = new BaseSpellInformation("Karura Form", "Monk", "karura", 360, 0);
            BaseSpells["Fierce Feral Form"] = new BaseSpellInformation("Fierce Feral Form", "Monk", "feral", 360, 0);
            BaseSpells["Feral Form"] = new BaseSpellInformation("Feral Form", "Monk", "feral", 360, 0);
            BaseSpells["Fierce Komodas Form"] = new BaseSpellInformation("Fierce Komodas Form", "Monk", "komodas", 360, 0);
            BaseSpells["Komodas Form"] = new BaseSpellInformation("Komodas Form", "Monk", "komodas", 360, 0);
            BaseSpells["ard ioc comlha"] = new BaseSpellInformation("ard ioc comlha", "Priest", "ioc", 1720, 2);
        }
        #endregion
    }

    public class BaseSkillInformation
    {
        public string sName { get; set; }
        public string sClass { get; set; }
        public string sGroup { get; set; }
        public int sCooldown { get; set; }

        public BaseSkillInformation(string name, string @class, string group, int cooldown = 0)
        {
            sName = name;
            sClass = @class;
            sGroup = group;
            sCooldown = cooldown;
        }

    }

    public class BaseSpellInformation
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public string Group { get; set; }
        public byte BaseLines { get; set; }
        public byte Lines { get; set; }
        public int Mana { get; set; }
        public int Cooldown { get; set; }

        public BaseSpellInformation(string name, string @class, string group, int mana, byte baselines = 0, int cooldown = 0)
        {
            Name = name;
            Class = @class;
            Group = group;
            BaseLines = baselines;
            Mana = mana;
            Cooldown = cooldown;
            Lines = 0;
        }
    }

    public class TargetCondition
    {
        public Func<MapObject, bool> predicate { get; set; }
        public int Priority { get; set; }
    }
}