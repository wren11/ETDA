using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace BotCore.States
{
    [Serializable]
    public abstract class GameState : IComparable<GameState>, IComparer<GameState>
    {
        [NonSerialized]
        [XmlIgnore]
        public Stopwatch timer = new Stopwatch();

        [NonSerialized]
        [Browsable(false)]
        public GameClient Client;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [NonSerialized]
        [Browsable(false)]
        public StateSettings SettingsInterface;

        [Browsable(false)]
        public abstract int Priority { get; set;  }

        [Browsable(false)]
        public abstract bool NeedToRun { get; set; }

        [Browsable(false)]
        public bool Enabled { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public bool InTransition { get; set; }

        public abstract void Run(TimeSpan Elapsed);

        public virtual void InitState()
        {
        }

        public int CompareTo(GameState other)
        {
            return -Priority.CompareTo(other.Priority);
        }

        public int Compare(GameState x, GameState y)
        {
            return -x.Priority.CompareTo(y.Priority);
        }
    }
}
