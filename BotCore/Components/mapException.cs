using System;
using System.Runtime.Serialization;

namespace BotCore.Components
{
    [Serializable]
    internal class mapException : Exception
    {
        public mapException()
        {

        }

        public mapException(string message) : base(message)
        {

        }
    }
}