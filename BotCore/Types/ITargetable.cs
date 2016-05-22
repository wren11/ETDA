using System;

namespace BotCore.Types
{
    public interface  ITargetable 
    {
        int TargetPriority { get; set; }

        Func<MapObject, bool> CanTarget { get; set; }
    }
}
