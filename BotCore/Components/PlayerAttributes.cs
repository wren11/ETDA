using BotCore.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotCore.Components
{
    public class PlayerAttributes : UpdateableComponent
    {
        public Direction Direction;
        public Position ServerPosition { get; set; }

        public List<PartyGroup> GroupMembers = new List<PartyGroup>();

        public class PartyGroup
        {
            public string PlayerName { get; set; }

            public PartyGroup(string player)
            {
                PlayerName = player;
            }
        }

        public int Serial
        {
            get
            {
                var id = UserId();
                return id;
            }
        }

        public int HP
        {
            get { return CurrentHP(); }
        }

        public int MaxHP
        {
            get { return MaximumHP(); }
        }

        public int MP
        {
            get { return CurrentMP(); }
        }

        public int MaxMP
        {
            get { return MaximumMP(); }
        }

        private string m_playername
        {
            get
            {
                return IsInGame() ? _memory.ReadString((IntPtr)0x0073D910, false, 20) : "NoName";
            }
            set
            {

            }
        }

        public string PlayerName
        {
            get
            {
                return IsInGame() ? m_playername : "Noname";
            }
            set
            {
                m_playername = value;
            }
        }

        public string Guild { get; set; }
        public byte Class { get; set; }
        public byte Nation { get; set; }
        public StringBuilder Group { get; set; }
        public bool HasArdCradh { get; internal set; }
        public bool HasCradh { get; internal set; }
        public bool HasBC { get; internal set; }
        public bool HasMorCradh { get; internal set; }
        public bool HasSeal { get; internal set; }

        public PlayerAttributes()
        {
            Timer = new UpdateTimer(TimeSpan.FromMilliseconds(1.0));
        }

        public int CurrentHP()
        {
            if (_memory == null || !_memory.IsRunning || !IsInGame())
                return 0;

            var ptr = _memory.Read<int>((IntPtr)0x00755AA4, false) + 0x19C;
            var hp = _memory.Read<int>((IntPtr)ptr, false);

            return hp;
        }

        public int MaximumHP()
        {
            if (_memory == null || !_memory.IsRunning || !IsInGame())
                return 0;

            var ptr = _memory.Read<int>((IntPtr)0x00755AA4, false) + 0x19C + 0x04;
            var hp = _memory.Read<int>((IntPtr)ptr, false);

            return hp;
        }

        public int CurrentMP()
        {
            if (_memory == null || !_memory.IsRunning || !IsInGame())
                return 0;

            var ptr = _memory.Read<int>((IntPtr)0x00755AA4, false) + 0x1A4;
            var hp = _memory.Read<int>((IntPtr)ptr, false);

            return hp;
        }

        public int MaximumMP()
        {
            if (_memory == null || !_memory.IsRunning || !IsInGame())
                return 0;

            var ptr = _memory.Read<int>((IntPtr)0x00755AA4, false) + 0x1A8;
            var hp = _memory.Read<int>((IntPtr)ptr, false);

            return hp;
        }

        private int UserId()
        {
            if (!IsInGame())
                return -1;
            if (_memory == null || !_memory.IsRunning)
                return -1;

            try
            {
                var id = 
                    _memory.Read<int>((IntPtr)
                        _memory.Read<int>((IntPtr)
                            _memory.Read<int>((IntPtr)
                                _memory.Read<int>((IntPtr)0x0073D944, false) + 0x4C, false) + 0x30, false) + 0x10, false);

                return id;
            }
            catch
            {
                return -1;
            }
        }

        public override void Update(TimeSpan tick)
        {
            Timer.Update(tick);

            if (Timer.Elapsed)
            {
                Timer.Reset();
                base.Pulse();
            }
        }

        internal void AddGroupMember(string v)
        {
            var grp = new PartyGroup(v);
            GroupMembers.Add(grp as PartyGroup);
        }
    }
}
