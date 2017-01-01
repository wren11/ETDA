using BotCore.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace BotCore.States
{
    public delegate void SystemMessageHandler(GameClient client);
    public delegate void RegexSystemMessageHandler(GameClient client, Match match);

    public class MessageStateMachine
    {
        private Dictionary<string, SystemMessageHandler> _systemMessageHandlers;
        private Dictionary<string, RegexSystemMessageHandler> _regexSystemMessageHandlers;

        public MessageStateMachine()
        {
            _systemMessageHandlers = new Dictionary<string, SystemMessageHandler>();
            _regexSystemMessageHandlers = new Dictionary<string, RegexSystemMessageHandler>();

            SetStates();
        }
        internal void SetStates()
        {
            RegisterMessageState("Group disbanded.", new SystemMessageHandler(this.GroupDisbanded));
            RegisterMessageState("You can't cast a spell.", new SystemMessageHandler(this.CastError));
            RegisterMessageState("Something went wrong.", new SystemMessageHandler(this.CastError));
            RegisterMessageState("Failed.", new SystemMessageHandler(this.CastFailed));
            RegisterMessageState("The magic has been deflected.", new SystemMessageHandler(this.CastMissed));
            RegisterMessageState("You can't cast that spell right now.", new SystemMessageHandler(this.CastError));
            RegisterMessageState("You are stuck.", new SystemMessageHandler(this.GmPorted));
            RegisterMessageState("You can't use skills here.", new SystemMessageHandler(this.NoSkills));
            RegisterMessageState("That doesn't work here.", new SystemMessageHandler(this.NoSpells));
            RegisterMessageState("You already cast that spell.", new SystemMessageHandler(this.AlreadyCasted));
            RegisterMessageState("^([a-zA-Z]+) is (?:joining|leaving) this group\\.$", new RegexSystemMessageHandler(this.GroupUpdate));
            RegisterMessageState("^You cast (.*?)\\.$", new RegexSystemMessageHandler(this.CastedSpell));
            RegisterMessageState("^Another curse afflicts thee\\. \\[(.*?)\\]$", new RegexSystemMessageHandler(this.AlreadyCursed));
            RegisterMessageState("Nothing happened.", new RegexSystemMessageHandler(this.NothingHappened));
            RegisterMessageState("Your Will is too weak.", new SystemMessageHandler(this.CastError));
            RegisterMessageState("Another spell of that nature is in effect", new RegexSystemMessageHandler(AlreadyCursed));
            RegisterMessageState("You failed to concentrate.", new SystemMessageHandler(this.CastError));
            RegisterMessageState("Your Path has forbidden itself from this vulgar implement.", new SystemMessageHandler(this.CantEquip));
            RegisterMessageState("You must be in your beast form.", new SystemMessageHandler(this.BeastForm));
            RegisterMessageState("You can't push it.", new RegexSystemMessageHandler(this.NothingHappened));
            RegisterMessageState("You can't use the weapon when holding it.", new SystemMessageHandler(this.TakeShieldOff));
            RegisterMessageState("No Target", new SystemMessageHandler(this.redMissed));
            RegisterMessageState("This feature is for registered users.", new SystemMessageHandler(Unreg));
            RegisterMessageState("You must wait for your combo move to recover.", new SystemMessageHandler(ComboScroll));
            RegisterMessageState("You can't do that while transformed.", new SystemMessageHandler(Unform));
            RegisterMessageState("beag cradh end.", (cb) => { cb.Attributes.HasBC = false; });
            RegisterMessageState("cradh end.", (cb) => { cb.Attributes.HasCradh = false; });
            RegisterMessageState("mor cradh end.", (cb) => { cb.Attributes.HasMorCradh = false; });
            RegisterMessageState("ard cradh end.", (cb) => { cb.Attributes.HasArdCradh = false; });
        }

        private void CastFailed(GameClient client)
        {
        }

        private void CastMissed(GameClient client)
        {
        }

        private void GmPorted(GameClient client)
        {
            client.Paused = true;
        }

        public void RegisterMessageState(string message, SystemMessageHandler handler)
        {
            _systemMessageHandlers[message] = handler;
        }
        public void RegisterMessageState(string pattern, RegexSystemMessageHandler handler)
        {
            _regexSystemMessageHandlers[pattern] = handler;
        }
        public void HandleSystemMessage(GameClient client, string input)
        {
            if (_systemMessageHandlers.ContainsKey(input))
            {
                _systemMessageHandlers[input](client);
                return;
            }
            foreach (KeyValuePair<string, RegexSystemMessageHandler> current in _regexSystemMessageHandlers)
            {
                var match = Regex.Match(input, current.Key);
                if (match.Success)
                {
                    current.Value(client, match);
                    break;
                }
            }
        }
        private void GroupDisbanded(GameClient client)
        {

        }

        private void CastError(GameClient client)
        {
            client.LastCastedSpell = null;
            client.LastCastTarget = null;
        }

        private void NoSkills(GameClient client)
        {
            client.FieldMap.CanUseSkills = false;
        }
        private void NoSpells(GameClient client)
        {
            client.FieldMap.CanCastSpells = false;
            client.LastCastedSpell = null;
            client.LastCastTarget = null;
        }

        private void AlreadyCasted(GameClient client)
        {
            if (client.LastCastTarget == null || client.LastCastedSpell == null)
                return;

            #region curse
            if (client.LastCastedSpell.Name.StartsWith("ard cradh"))
                SetIsCursed(client, "ard cradh");
            #endregion
            #region fas
            if (client.LastCastedSpell.Name.StartsWith("mor fas nadur"))
                SetIsFas(client, "mor fas nadur");
            #endregion

        }

        private void CantEquip(GameClient client)
        {

        }

        private void BeastForm(GameClient client)
        {

        }

        private void TakeShieldOff(GameClient client)
        {

        }

        private void redMissed(GameClient client)
        {
            //refresh this client, not facing the target
            Actions.GameActions.Refresh(client);
        }

        private void Unreg(GameClient client)
        {

        }

        private void ComboScroll(GameClient client)
        {

        }

        private void Unform(GameClient client)
        {

        }

        private void GroupUpdate(GameClient client, Match match)
        {

        }

        private void CastedSpell(GameClient client, Match match)
        {
            if (client.LastCastTarget == null || client.LastCastedSpell == null)
                return;

            string value = match.Groups[1].Value;
            switch (value)
            {
                #region fas
                case "beag fas nadur":
                    SetIsFas(client, value);
                    break;
                case "fas nadur":
                    SetIsFas(client, value);
                    break;
                case "mor fas nadur":
                    SetIsFas(client, value);
                    break;
                case "ard fas nadur":
                    SetIsFas(client, value);
                    break;
                #endregion
                #region curse
                case "beag cradh":
                    SetIsCursed(client, value);
                    break;
                case "cradh":
                    SetIsCursed(client, value);
                    break;
                case "mor cradh":
                    SetIsCursed(client, value);
                    break;
                case "ard cradh":
                    SetIsCursed(client, value);
                    break;
                case "Dark Seal":
                    SetIsCursed(client, value);
                    break;
                case "Darker Seal":
                    SetIsCursed(client, value);
                    break;

                #endregion
                #region aite
                case "beag naomh aite":
                case "naomh aite":
                case "mor naomh aite":
                case "ard naomh aite":
                #endregion
                #region Ao
                case "ao beag cradh":
                    if (client.Attributes.Serial == client.LastCastTarget.Serial)
                        client.Attributes.HasBC = false;
                    break;
                case "ao cradh":
                    if (client.Attributes.Serial == client.LastCastTarget.Serial)
                        client.Attributes.HasCradh = false;
                    break;
                case "ao mor cradh":
                    if (client.Attributes.Serial == client.LastCastTarget.Serial)
                        client.Attributes.HasMorCradh = false;
                    break;
                case "ao ard cradh":
                    if (client.Attributes.Serial == client.LastCastTarget.Serial)
                        client.Attributes.HasArdCradh = false;
                    break;

                #endregion

                default:
                    break;
            }
        }

        private void AlreadyCursed(GameClient client, Match match)
        {
            if (client.LastCastTarget == null || client.LastCastedSpell == null)
                return;

            string value = match.Groups[1].Value;
            if (!string.IsNullOrWhiteSpace(value))
                SetIsCursed(client, value);
        }


        private static void SetIsCursed(GameClient client, string value)
        {
            var target = client.FieldMap.MapObjects.FirstOrDefault(obj => obj.Serial == client.LastCastTarget.Serial);
            if (target != null)
            {
                target.CurseInfo = new CurseInfo();
                target.CurseInfo.Applied = DateTime.Now;
                target.IsCursed = true;


                switch (value)
                {
                    case "ard cradh":
                        target.CurseInfo.Type = CurseInfo.Curse.ardcradh;
                        target.CurseInfo.Duration = 240000; // 4minutes
                        if (target.Serial == client.Attributes.Serial)
                            client.Attributes.HasArdCradh = true;
                        break;
                    case "mor cradh":
                        target.CurseInfo.Type = CurseInfo.Curse.morcradh;
                        target.CurseInfo.Duration = 210000; // 3 minutes 30 seconds
                        if (target.Serial == client.Attributes.Serial)
                            client.Attributes.HasMorCradh = true;
                        break;
                    case "cradh":
                        target.CurseInfo.Type = CurseInfo.Curse.cradh;
                        target.CurseInfo.Duration = 180000; // 3 minutes
                        if (target.Serial == client.Attributes.Serial)
                            client.Attributes.HasCradh = true;
                        break;
                    case "beag cradh":
                        target.CurseInfo.Type = CurseInfo.Curse.beagcradh;
                        target.CurseInfo.Duration = 150000; // 2 minutes 30 seconds
                        if (target.Serial == client.Attributes.Serial)
                            client.Attributes.HasBC = true;
                        break;
                    case "Dark Seal":
                        target.CurseInfo.Type = CurseInfo.Curse.darkseal;
                        target.CurseInfo.Duration = 153000; // 2 minutes 33 seconds
                        if (target.Serial == client.Attributes.Serial)
                            client.Attributes.HasSeal = true;
                        break;
                    case "Darker Seal":
                        target.CurseInfo.Duration = 153000; // 2 minutes 33 seconds
                        target.CurseInfo.Applied = DateTime.Now;
                        if (target.Serial == client.Attributes.Serial)
                            client.Attributes.HasSeal = true;
                        break;
                }
            }           
        }

        private static void SetIsFas(GameClient client, string value)
        {
            var target = client.FieldMap.MapObjects.FirstOrDefault(obj => obj.Serial == client.LastCastTarget.Serial);
            if (target != null)
            {
                target.FasInfo = new FasInfo();
                target.FasInfo.Applied = DateTime.Now;

                switch (value)
                {
                    case "ard fas nadur":
                        target.FasInfo.Type = FasInfo.Fas.ardfasnadur;
                        target.FasInfo.Duration = 225000; // 3 minutes 45 seconds
                        break;
                    case "mor fas nadur":
                        target.FasInfo.Type = FasInfo.Fas.morfasnadur;
                        target.FasInfo.Duration = 450000; // 7 minutes 30 seconds
                        break;
                    case "fas nadur":
                        target.FasInfo.Type = FasInfo.Fas.fasnadur;
                        target.FasInfo.Duration = 450000; // 7 minutes 30 seconds
                        break;
                    case "beag fas nadur":
                        target.FasInfo.Type = FasInfo.Fas.beagfasnadur;
                        target.FasInfo.Duration = 450000; // 7 minutes 30 seconds
                        break;
                    default:
                        break;                      
                }
            }
        }

        private void NothingHappened(GameClient client, Match match)
        {

        }
    }
}
