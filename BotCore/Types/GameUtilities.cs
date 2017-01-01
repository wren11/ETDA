using BotCore.Actions;
using BotCore.States;
using BotCore.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BotCore.PathFinding;

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
            //add other conditions that may prevent walking.
            return !IsFrozen();
        }

        public bool IsFrozen()
        {
            return ((Client.SpellBar.Contains((short)SpellBar.palsy) 
                || Client.SpellBar.Contains((short)SpellBar.skulled)
                || Client.SpellBar.Contains((short)SpellBar.pramh)));
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
            {
                return;
            }

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

        public void ComputeStep(List<PathSolver.PathFinderNode> path, int Distance = 2)
        {
            var pos = new Position(path[Distance].X, path[Distance].Y);
            var abx = pos.X - Client.Attributes.ServerPosition.X;
            var aby = pos.Y - Client.Attributes.ServerPosition.Y;

            if (abx == Distance && aby == 0)
                GameActions.Walk(Client, Direction.East);
            else if ((abx == 0 && aby == -Distance)
                || (abx == 1 && aby == -1)
                || (abx == -1 && aby == -1))
                GameActions.Walk(Client, Direction.North);
            else if (abx == -Distance && aby == 0)
                GameActions.Walk(Client, Direction.West);
            else if ((abx == 0 && aby == Distance)
                || (abx == 1 && aby == 1)
                || (abx == -1 && aby == 1))
                GameActions.Walk(Client, Direction.South);
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
            if (string.IsNullOrEmpty(spellName))
            {
                return;
            }

            if (Client.SpellBar.Contains((short)SpellBar.palsy) && HaveSpell("ao suain"))
                spellName = "ao suain";

            if (!SafeToCast())
            {
                return;
            }

            var basespell = Collections.BaseSpells[spellName];

            if (Client.Attributes.CurrentMP() < basespell.Mana)
            {
                return;
            }

            var spell = Client.GameMagic[basespell.Name];
            if (spell == null)
            {
                return;
            }

            if (spell.OptimalStaff == null)
                CalculateBaseSpellLines(basespell);
            if (spell.OptimalStaff == null)
            {
                return;
            }

            var weapon = Client.ActiveEquipment.CurrentWeaponName();
            while (weapon == string.Empty)
            {
                weapon = Client.ActiveEquipment.CurrentWeaponName();
                Thread.Sleep(100);
            }

            if (!Collections.BaseStaffs.ContainsKey(weapon))
            {
                return;
            }

            var staff = Collections.BaseStaffs[weapon].Id;
            var item = Client.GameInventory.Items.FirstOrDefault(i =>
                i != null && i.ItemName.StartsWith(spell.OptimalStaff.Name));

            if (spell.OptimalStaff.Id == staff)
            {
                if (target == null || spell == null)
                {
                    return;
                }

                Cast(target, spell);
                return;
            }
            else
            {
                if (item != null)
                {
                    Swap(target, spell, item);
                    Thread.Sleep(650);

                    while (true)
                    {
                        weapon = Client.ActiveEquipment.CurrentWeaponName();

                        if (!Collections.BaseStaffs.ContainsKey(weapon))
                        {
                            return;
                        }

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

                        Thread.Sleep(10);
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

          
            if (target.Sprite == 0 || target.Sprite == ushort.MaxValue)
            {
                return;
            }

            if (!SafeToCast())
            {
                return;
            }

            if (spell.EquipedLines > 10)
                spell.EquipedLines = 0;

            Client.LastCastStarted = DateTime.Now;
            Client.LastCastLines = spell.EquipedLines;

            GameActions.BeginSpell(Client, spell.EquipedLines);

            if (spell.EquipedLines > 0)
            {
                Client.ApplyMovementLock();
                for (int i = 0; i < spell.EquipedLines; i++)
                {
                    GameActions.SendSpellLines(Client, (i + 1).ToString());
                    for (int n = 0; n < 10; n++)
                    {
                        if (Client.SpellBar.Contains((short)SpellBar.palsy) && spell.CastName != "ao suain")
                        {
                            return;
                        }

                        Thread.Sleep(101);
                    }
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

        public List<PathSolver.PathFinderNode> FindPath(Position Target)
        {
            try
            {

                Client.FieldMap?.SetPassable(Client.Attributes.ServerPosition);
                Client.FieldMap?.SetPassable(Target);

                var results =
                    Client.FieldMap.Search(Client.Attributes.ServerPosition, Target);

                return results;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public void WalkTowardsClient(GameClient Entity)
        {
            if (Client.IsRefreshing)
                Thread.Sleep(1000);

            try
            {
                var EndLocation = new Position { X = Entity.Attributes.ServerPosition.X,
                    Y = Entity.Attributes.ServerPosition.Y };

                List<PathSolver.PathFinderNode> PathNodes = null;

                for (int i = 0; i < 4; i++)
                {
                    if (Client.IsRefreshing)
                        Thread.Sleep(1000);

                    PathNodes = FindPath(EndLocation);

                    if (PathNodes == null)
                    {
                        if (i == 3)
                            throw new WalkingException();
                        continue;
                    }
                    if (PathNodes.Count == 1)
                    {
                        if (i == 3)
                            throw new WalkingException();

                        GameActions.Refresh(Client);
                        continue;
                    }
                    break;
                }

                for (int i = 1; i < PathNodes.Count - 1; i++)
                {

                    if (Entity.Attributes.ServerPosition.X != EndLocation.X || Entity.Attributes.ServerPosition.Y != EndLocation.Y)
                    {
                        EndLocation = new Position { X = Entity.Attributes.ServerPosition.X, Y = Entity.Attributes.ServerPosition.Y };

                        for (int j = 0; j < 4; j++)
                        {
                            PathNodes = FindPath(EndLocation);
                            if (PathNodes == null)
                                throw new WalkingException();
                            if (PathNodes.Count == 1)
                                GameActions.Refresh(Client);
                            if (PathNodes.Count == 2)
                                return;
                            if (j == 3)
                                throw new WalkingException();
                        }

                        i = 1;
                    }


                    foreach (MapObject Ent in Client.ObjectSearcher.VisibleObjects)
                    {
                        if (Ent.Type == MapObjectType.Monster || Ent.Type == MapObjectType.Aisling)
                        {
                            if (Ent.ServerPosition.X == PathNodes[i].X && Ent.ServerPosition.Y == PathNodes[i].Y)
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    if (Client.IsRefreshing)
                                        Thread.Sleep(1000);

                                    PathNodes = FindPath(Entity.Attributes.ServerPosition);
                                    if (PathNodes == null)
                                        throw new WalkingException();
                                    if (PathNodes.Count == 1)
                                        GameActions.Refresh(Client);
                                    if (PathNodes.Count == 2)
                                        return;
                                    if (j == 3)
                                        throw new WalkingException();
                                }
                                i = 1;
                            }
                        }
                    }

                    Direction Direction;
                    var MyPosition = Client.Attributes.ServerPosition;

                    if (PathNodes[i].X == MyPosition.X && PathNodes[i].Y == MyPosition.Y - 1)
                        Direction = Direction.North;
                    else if (PathNodes[i].X == MyPosition.X && PathNodes[i].Y == MyPosition.Y + 1)
                        Direction = Direction.South;
                    else if (PathNodes[i].X == MyPosition.X - 1 && PathNodes[i].Y == MyPosition.Y)
                        Direction = Direction.West;
                    else if (PathNodes[i].X == MyPosition.X + 1 && PathNodes[i].Y == MyPosition.Y)
                        Direction = Direction.East;
                    else throw new WalkingException();

                    GameActions.Walk(Client, Direction);
                }
                throw new WalkingException();
            }
            catch (Exception e)
            {
            }
        }

        public void WalkTowardsObj(MapObject Entity)
        {
            if (Client.IsRefreshing)
                Thread.Sleep(1000);

            try
            {
                var EndLocation = new Position { X = Entity.ServerPosition.X, Y = Entity.ServerPosition.Y };

                List<PathSolver.PathFinderNode> PathNodes = null;

                for (int i = 0; i < 4; i++)
                {
                    if (Client.IsRefreshing)
                        Thread.Sleep(1000);

                    PathNodes = FindPath(EndLocation);

                    if (PathNodes == null)
                    {
                        if (i == 3)
                            throw new WalkingException();
                        continue;
                    }
                    if (PathNodes.Count == 1)
                    {
                        if (i == 3)
                            throw new WalkingException();

                        GameActions.Refresh(Client);
                        continue;
                    }
                    break;
                }

                for (int i = 1; i < PathNodes.Count - 1; i++)
                {

                    if (Entity.ServerPosition.X != EndLocation.X || Entity.ServerPosition.Y != EndLocation.Y)
                    {
                        EndLocation = new Position { X = Entity.ServerPosition.X, Y = Entity.ServerPosition.Y };

                        for (int j = 0; j < 4; j++)
                        {
                            PathNodes = FindPath(EndLocation);
                            if (PathNodes == null)
                                throw new WalkingException();
                            if (PathNodes.Count == 1)
                                GameActions.Refresh(Client);
                            if (PathNodes.Count == 2)
                                return;
                            if (j == 3)
                                throw new WalkingException();
                        }

                        i = 1;
                    }


                    foreach (MapObject Ent in Client.ObjectSearcher.VisibleObjects)
                    {
                        if (Ent.Type == MapObjectType.Monster || Ent.Type == MapObjectType.Aisling)
                        {
                            if (Ent.ServerPosition.X == PathNodes[i].X && Ent.ServerPosition.Y == PathNodes[i].Y)
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    if (Client.IsRefreshing)
                                        Thread.Sleep(1000);

                                    PathNodes = FindPath(Entity.ServerPosition);
                                    if (PathNodes == null)
                                        throw new WalkingException();
                                    if (PathNodes.Count == 1)
                                        GameActions.Refresh(Client);
                                    if (PathNodes.Count == 2)
                                        return;
                                    if (j == 3)
                                        throw new WalkingException();
                                }
                                i = 1;
                            }
                        }
                    }

                    Direction Direction;
                    var MyPosition = Client.Attributes.ServerPosition;

                    if (PathNodes[i].X == MyPosition.X && PathNodes[i].Y == MyPosition.Y - 1)
                        Direction = Direction.North;
                    else if (PathNodes[i].X == MyPosition.X && PathNodes[i].Y == MyPosition.Y + 1)
                        Direction = Direction.South;
                    else if (PathNodes[i].X == MyPosition.X - 1 && PathNodes[i].Y == MyPosition.Y)
                        Direction = Direction.West;
                    else if (PathNodes[i].X == MyPosition.X + 1 && PathNodes[i].Y == MyPosition.Y)
                        Direction = Direction.East;
                    else throw new WalkingException();

                    GameActions.Walk(Client, Direction);
                }
                throw new WalkingException();
            }
            catch (Exception e)
            {
            }
        }
    }
}
