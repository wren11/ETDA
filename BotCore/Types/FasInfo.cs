using System;

namespace BotCore.Types
{
    public class FasInfo
    {
        public Fas Type
        {
            get; set;
        }

        public DateTime Applied { get; set; }
        public bool FasElapsed
        {
            get
            {
                return (DateTime.Now - Applied).TotalMilliseconds > Duration;
            }
        }

        /// <summary>
        /// value in milliseonds seconds
        /// </summary>
        public int Duration { get; set; }

        public FasInfo()
        {
            Type = Fas.none;
        }

        public enum Fas
        {
            none,
            beagfasnadur,
            fasnadur,
            morfasnadur,
            ardfasnadur
        }
    }
}