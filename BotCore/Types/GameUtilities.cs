using BotCore.Actions;
using BotCore.States;
using BotCore.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BotCore
{
    public class GameUtilities
    {
        private GameClient Client;

        public GameUtilities(GameClient client)
        {
            Client = client;
        }

        public bool CanWalk()
        {

            return (!Client.SpellBar.Contains((short)SpellBar.palsy)
                        || Client.SpellBar.Contains((short)SpellBar.skulled)
                        || Client.SpellBar.Contains((short)SpellBar.pramh));
        }
        public void CastSpell(string spellName, Client client)
        {
            CastSpell(spellName,
                new MapObject()
                {
                    Serial = client.Attributes.Serial,
                    ServerPosition = Client.Attributes.ServerPosition,
                    Sprite = 1,
                });
        }

        public bool HaveSpell(string SpellName)
        {
            var in_spellBook = Client.GameMagic[SpellName] != null;
            return in_spellBook;
        }

        public bool HaveStaff(string name)
        {
            var in_inventory = (Client.GameInventory.Items.FirstOrDefault(i => i != null && i.ItemName == name) != null);
            var is_equipped = (Client.ActiveEquipment.Items.FirstOrDefault(i => i != null && i.Name == name) != null);

            if (in_inventory || is_equipped)
                return true;

            return false;
        }

        public void CalculateLines()
        {
            foreach (var item in Collections.BaseSpells)
            {
                var basespell = Collections.BaseSpells[item.Key];
                CalculateBaseSpellLines(basespell);
            }
        }

        private void CalculateBaseSpellLines(BaseSpellInformation basespell)
        {
            var spell = Client.GameMagic[basespell.Name];
            if (spell == null)
                return;

            spell.UnequipedLines = basespell.BaseLines;

            var Computed = new Dictionary<string, Tuple<string, byte>>();

            foreach (var staff in Collections.BaseStaffs)
            {
                staff.Value.Name = staff.Key;

                var mods = staff.Value.Modifer;
                if (mods.Action == ActionModifier.Decrease)
                    spell.EquipedLines = (byte)(spell.UnequipedLines - mods.Value);
                else if (mods.Action == ActionModifier.Increase)
                    spell.EquipedLines = (byte)(spell.UnequipedLines + mods.Value);
                else if (mods.Action == ActionModifier.Set)
                    spell.EquipedLines = (byte)mods.Value;

                if (spell.EquipedLines == 255)
                    spell.EquipedLines = 0;



                if (basespell.Group == "fas nadur" && staff.Key.ToLower() == "assassin's cross")
                    spell.EquipedLines += 1;
                
                if (staff.Value.Modifer.Scope == SpellScope.All || staff.Value.Modifer.Scope == SpellScope.AllExcept)
                    Computed[staff.Key] = new Tuple<string, byte>(spell.Name, spell.EquipedLines);

                if (staff.Value.Modifer.Scope == SpellScope.Single && basespell.Group == "cradh"
                    && staff.Value.Modifer.Name.Contains("cradh") && staff.Key.ToLower().Contains("gnarl"))
                {
                    spell.EquipedLines = 0;
                    Computed[staff.Key] = new Tuple<string, byte>(spell.Name, spell.EquipedLines);
                }
            }

            Dictionary<byte, StaffTable> Available = new Dictionary<byte, StaffTable>();
            foreach (var computed in Computed)
            {
                var staffname = computed.Key;
                if (!HaveStaff(staffname))
                    continue;
                if (!Available.ContainsKey(computed.Value.Item2))
                    Available[computed.Value.Item2] = Collections.BaseStaffs[staffname];
            }

            if (Available.Count > 0)
            {
                var bestStaff = Available.OrderBy(i => i.Key).FirstOrDefault();
                spell.OptimalStaff = bestStaff.Value;
                spell.EquipedLines = bestStaff.Key;
                spell.CastName = basespell.Name;
            }
        }

        public bool CanSwitchWeapon()
        {
            return
                (!(
                    Client.SpellBar.Contains((short)SpellBar.palsy) ||
                    Client.SpellBar.Contains((short)SpellBar.skulled) ||
                    Client.SpellBar.Contains((short)SpellBar.pramh) ||
                    Client.SpellBar.Contains((short)SpellBar.lizardform) ||
                    Client.SpellBar.Contains((short)SpellBar.wolfform) ||
                    Client.SpellBar.Contains((short)SpellBar.birdform) ||
                    Client.SpellBar.Contains((short)SpellBar.transform)
                    )
                );
        }

        public void CastSpell(string spellName, MapObject target)
        {
            if (Client.SpellBar.Contains((short)SpellBar.palsy))
                spellName = "ao suain";

            if (!SafeToCast())
                return;

            if (string.IsNullOrEmpty(spellName))
                return;

            var basespell = Collections.BaseSpells[spellName];

            if (Client.Attributes.CurrentMP() < basespell.Mana)
                return;

            var spell = Client.GameMagic[basespell.Name];
            if (spell == null)
                return;

            if (spell.OptimalStaff == null)
                CalculateBaseSpellLines(basespell);
            if (spell.OptimalStaff == null)
                return;

            var weapon = Client.ActiveEquipment.CurrentWeaponName();
            while (weapon == string.Empty)
            {
                weapon = Client.ActiveEquipment.CurrentWeaponName();
                Thread.Sleep(1);
            }

            if (!Collections.BaseStaffs.ContainsKey(weapon))
                return;

            var staff = Collections.BaseStaffs[weapon].Id;
            var item = Client.GameInventory.Items.FirstOrDefault(i =>
                i != null && i.ItemName.StartsWith(spell.OptimalStaff.Name));

            if (spell.OptimalStaff.Id == staff)
            {
                if (target == null || spell == null)
                    return;

                Cast(target, spell);
                return;
            }
            else
            {
                if (item != null)
                {
                    Swap(target, spell, item);
                    Thread.Sleep(500);
                    while (true)
                    {
                        weapon = Client.ActiveEquipment.CurrentWeaponName();

                        if (!Collections.BaseStaffs.ContainsKey(weapon))
                            return;

                        if (weapon != string.Empty)
                        {
                            staff = Collections.BaseStaffs[weapon].Id;

                            if (staff == spell.OptimalStaff.Id)
                                break;
                            else
                            {
                                Swap(target, spell, item);
                                Thread.Sleep(1000);
                            }
                        }

                        Thread.Sleep(1);
                    }

                    item = null;
                }
            }
        }

        private void Swap(MapObject target, Spell spell, InventoryItem item)
        {
            if (!CanSwitchWeapon())
            {
                GameActions.BeginSpell(Client, spell.EquipedLines);
                spell.EquipedLines = 1;
                Cast(target, spell);
                return;
            }

            GameActions.UseInventorySlot(Client, (byte)(item.Slot + 1));
        }

        public bool SafeToCast()
        {
            var hiddens = 
                Client.ObjectSearcher.RetreivePlayerTargets(
                i => i != null && (i as Aisling) != null && (i as Aisling).IsHidden && i.Serial != Client.Attributes.Serial);

            if (hiddens.Count > 0)
            {
                return false;
            }

            if (Client.FieldMap != null && Client.FieldMap.Ready && !Client.FieldMap.CanCastSpells)
                return false;

            return true;
        }

        private void Cast(MapObject target, Spell spell)
        {
            if (target.Sprite == 0)
                return;

            if (!SafeToCast())
            {
                return;
            }

            if (Client.IsCurrentlyCasting)
            {
                return;
            }

            if (spell.EquipedLines > 10)
                spell.EquipedLines = 0;

            GameActions.BeginSpell(Client, spell.EquipedLines);

            if (spell.EquipedLines > 0)
            {

                if (Client.StateMachine.States.OfType<FollowTarget>().Count() > 0)
                {
                    var state = Client.StateMachine.States.OfType<FollowTarget>().FirstOrDefault();
                    if (state != null && state.NeedToRun)
                        return;
                }


                Client.ApplyMovementLock();
                for (int i = 0; i < spell.EquipedLines; i++)
                {
                    if (Client.SpellBar.Contains((short)SpellBar.palsy) && spell.CastName != "ao suain")
                        return;

                    GameActions.SendSpellLines(Client, (i + 1).ToString());
                    Thread.Sleep(1050);
                }
            }

            if (spell.EquipedLines > 0)
            {
                GameActions.SendSpellLines(Client, spell.CastName);
                Client.ReleaseMovementLock();
            }

            GameActions.EndSpell(Client, spell.Slot, target);

            Client.LastCastedSpell = spell;
        }
    }
}
