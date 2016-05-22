namespace BotCore.Types
{
    public class ServerPacket : Packet { }
    public class ClientPacket : Packet { }

    public enum PacketType
    {
        ServerPacket = 1,
        ClientPacket = 2
    }
}
