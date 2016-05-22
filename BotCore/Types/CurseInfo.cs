using System;

namespace BotCore.Types
{
    public class CurseInfo
    {
        public Curse Type
        {
            get; set;
        }

        public DateTime Applied { get; set; }
        public bool CurseElapsed
        {
            get
            {
                return (DateTime.Now - Applied).TotalMilliseconds > Duration;
            }
        }

        /// <summary>
        /// value in seconds
        /// </summary>
        public int Duration { get; set; }

        public CurseInfo()
        {
            Type = Curse.none;
        }

        public enum Curse
        {
            none,
            beagcradh,
            cradh,
            morcradh,
            ardcradh,
            darkseal,            
            darkerseal
        }
    }
}
