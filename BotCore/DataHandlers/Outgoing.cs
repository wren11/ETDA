using System;
using System.Threading;

namespace BotCore.DataHandlers
{
    public static class Outgoing
    {

        //this must be a background capture.
        //because we will wait for user to login.
        internal static void LoggingIn(object sender, Packet e)
        {
            var client = Collections.AttachedClients[(int)sender];

            new Thread(delegate()
                {
                    //user logging in, don't transition until client is fully loaded up.
                    while (DateTime.Now - e.Date < new TimeSpan(0, 0, 0, 10, 0))
                    {
                        if (client.MapLoaded)
                            break;

                        Thread.Sleep(1000);
                    }

                    client.OnClientStateUpdated(true);
                }) { IsBackground = true }.Start();
        }

        internal static void LoggingOut(object sender, Packet e)
        {
            var client = Collections.AttachedClients[(int)sender];
            client.OnClientStateUpdated(false);
        }

        internal static void UseInventorySlot(object sender, Packet e)
        {
            var client = Collections.AttachedClients[(int)sender];
            var slot = e.ReadByte();

            client.GameInventory.Pulse();
            client.ActiveEquipment.Pulse();
            client.LastUseInvetorySlot = DateTime.Now;
        }

        internal static void SpellCasted(object sender, Packet e)
        {
            var client = Collections.AttachedClients[(int)sender];
        }

        internal static void SpellBegin(object sender, Packet e)
        {
            var client = Collections.AttachedClients[(int)sender];
        }
    }
}
