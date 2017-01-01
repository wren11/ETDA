using System.ComponentModel;

namespace BotCore.Shared.Memory
{
    public enum DAStaticPointers : int
    {
        [Description("DA 7.41 No Blind Address, Write 0x75 to disable blind effects")]
        NoBlind = 0x005D2DD4,
        [Description(@"DA 7.41 Character Base Object Pointer, A commonly used pointer to common information, such as map info, location, ect")]
        ObjectBase = 0x00882E68,
        [Description("DA 7.41 ETDA custom Option Table Pointer, Used to hide gold, filter sprites, animations, ect")]
        OptionTable = 0x00750000,
        [Description("DA 7.41 Attributes Pointer, HP, MP ect")]
        Attributes = 0x00755AA4,
        [Description("DA 7.41 User ID (Unique Serial)")]
        UserID = 0x0073D944,
        [Description("DA 7.41 Movement Lock Pointer (Write 0x74 to Lock Movement, 75 to resume movement)")]
        Movement = 0x005F0ADE,
        [Description("DA 7.41 Pointer where ETDA is injected.")]
        ETDA = 0x00567FB0,
        [Description("DA 7.41 Pointer to the clients Send Buffer")]
        SendBuffer = 0x006FD000,
        [Description("DA 7.41 Pointer to the clients Recv Buffer")]
        RecvBuffer = 0x00721000,
        [Description("DA 7.41 Pointer My Players Username")]
        Username = 0x0073D910,
        [Description("DA 7.41 Pointer My Players ActiveBar ")]
        ActiveBar = 0x006887EC,
        [Description("DA 7.41 Is Player Currently Casting something? 0 = no, 1 = yes ")]
        IsCasting = 0x0073FD78
    }
}
