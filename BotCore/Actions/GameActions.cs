using BotCore.Shared.Helpers;
using BotCore.Types;
using System;
using System.Threading;

namespace BotCore.Actions
{
    public class GameActions
    {

        //Base Functions Must be defined in here.
        //The callback is optional, But must be specified as final constructor.

        public static void Assail(GameClient client, Func<GameClient, Packet, bool> callback = null)
        {
            var p = new Packet();
            p.Write(new byte[] { 0x13, 0x01 });
            GameClient.InjectPacket<ServerPacket>(client, p);

            if (callback != null)
                callback(client, p);
        }


        public static void Refresh(GameClient client, Func<GameClient, Packet, bool> callback = null)
        {
            var p = new Packet();
            p.Write(new byte[] { 0x38, 0x00, 0x38 });
            GameClient.InjectPacket<ServerPacket>(client, p);
            GameClient.InjectPacket<ServerPacket>(client, p);

            if (callback != null)
                callback(client, p);
        }

        public static void PlayAnimation(GameClient client, short number, Position xy)
        {
            var packet = new Packet();
            packet.WriteByte(0x29);
            packet.WriteByte(0x00);
            packet.WriteByte(0x00);
            packet.WriteByte(0x00);
            packet.WriteByte(0x00);
            packet.WriteInt16(number);
            packet.WriteInt16(0x64);
            packet.WriteInt16(xy.X);
            packet.WriteInt16(xy.Y);
            packet.WriteByte(0x00);
            packet.WriteByte(0x00);

            GameClient.InjectPacket<ClientPacket>(client, packet);
        }

        public static DateTime LastUseInvetorySlot = DateTime.Now;



        public static void UseInventorySlot(GameClient client, byte slot)
        {
            var packet = new Packet();
            packet.WriteByte(0x1C);
            packet.WriteByte(slot);
            packet.WriteByte(0x00);

            GameClient.InjectPacket<ServerPacket>(client, packet);
        }

        public static void BeginSpell(GameClient client, byte SpellLines, 
            Func<GameClient, Packet, bool> callback = null)
        {
            var packet = new Packet();
            packet.WriteByte(0x4D);
            packet.WriteByte(SpellLines);
            packet.WriteByte(0x00);

            GameClient.InjectPacket<ServerPacket>(client, packet);
            if (callback != null)
                callback(client, packet);
        }

        public static void EndSpell(GameClient client, byte slot,
            Func<GameClient, Packet, bool> callback = null)
        {
            var packet = new Packet();
            packet.WriteByte(0x0F);
            packet.WriteByte(slot);
            packet.WriteByte(0x00);

            GameClient.InjectPacket<ServerPacket>(client, packet);
            if (callback != null)
                callback(client, packet);
        }

        public static void EndSpell(GameClient client, byte slot, MapObject obj,
            Func<GameClient, Packet, bool> callback = null)
        {
            //just a sanity check.
            if (obj == null || obj.ServerPosition == null)
                return;

            var packet = new Packet();
            packet.WriteByte(0x0F);
            packet.WriteByte(slot);
            packet.WriteInt32(obj.Serial);
            packet.WriteInt16(obj.ServerPosition.X);
            packet.WriteInt16(obj.ServerPosition.Y);

            GameClient.InjectPacket<ServerPacket>(client, packet);
            if (callback != null)
                callback(client, packet);

            Thread.Sleep(100);
        }

        public static void EndSpell(GameClient client, byte slot, GameClient obj,
            Func<GameClient, Packet, bool> callback = null)
        {
            EndSpell(client, slot, new MapObject() { Serial = obj.Attributes.Serial, ServerPosition = obj.Attributes.ServerPosition }, 
                callback);
        }

        public static void SendSpellLines(GameClient client, string msg,
            Func<GameClient, Packet, bool> callback = null)
        {
            var packet = new Packet();
            packet.WriteByte(0x4E);
            packet.WriteString8(msg);
            packet.WriteByte(0x00);

            GameClient.InjectPacket<ServerPacket>(client, packet);
            if (callback != null)
                callback(client, packet);
        }

        public static void RequestProfile(GameClient client,
            Func<GameClient, Packet, bool> callback = null)
        {
            var packet = new Packet();
            packet.WriteByte(0x2D);
            packet.WriteByte(0x00);

            GameClient.InjectPacket<ServerPacket>(client, packet);
            if (callback != null)
                callback(client, packet);
        }
    
        public static void Say(GameClient client, MapObject obj, string val)
        {
            var p = new Packet();
            p.WriteByte(0x0D);
            p.WriteByte(0x01);
            p.WriteInt32(obj.Serial);
            p.WriteString8(val);
            p.WriteByte(0x00);
            p.WriteByte(0x00);
            GameClient.InjectPacket<ClientPacket>(client, p);
        }
    }
}
