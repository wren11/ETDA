using System;
using System.Runtime.Serialization;

namespace BotCore.Types
{
    [Serializable]
    internal class WalkingException : Exception
    {
        public WalkingException()
        {
        }

        public WalkingException(string message) : base(message)
        {
        }

        public WalkingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WalkingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}