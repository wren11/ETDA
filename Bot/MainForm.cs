using BotCore;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

using Collections = BotCore.Collections;

namespace Bot
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is MdiClient)
                {
                    ctrl.BackColor = Color.White;
                }
            }
        }

        int idx = 0, previd = 0;

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x004A)
            {
                var ptr = (Copydatastruct)Marshal.PtrToStructure(m.LParam, typeof(Copydatastruct));
                if (ptr.CbData <= 0)
                    return;

                var buffer = new byte[ptr.CbData];
                var id = CheckTouring(ref m, ref ptr);

                if (!Collections.AttachedClients.ContainsKey(id))
                    return;

                Marshal.Copy(ptr.LpData, buffer, 0, ptr.CbData);

                var packet = new Packet();
                packet.Date = DateTime.Now;
                packet.Data = buffer;
                packet.Type = (int)ptr.DwData;
                packet.Client = Collections.AttachedClients[id];

                if (packet.Type == 1)
                    Collections.AttachedClients[id].OnPacketRecevied(id, packet);
                if (packet.Type == 2)
                    Collections.AttachedClients[id].OnPacketSent(id, packet);

                Intercept(ptr, packet, id);
            }
        }

        private int CheckTouring(ref Message m, ref Copydatastruct ptr)
        {
            var id = m.WParam.ToInt32();
            if (id > 0x7FFFF && idx++ % 2 == 0)
            {
                if (ptr.DwData == 2)
                    if (Collections.AttachedClients.ContainsKey(previd))
                        Collections.AttachedClients[previd].SendPointer = id;
                if (ptr.DwData == 1)
                    if (Collections.AttachedClients.ContainsKey(previd))
                        Collections.AttachedClients[previd].RecvPointer = id;
            }
            else
            {
                previd = id;
            }
            return id;
        }


#if TESTMODE

#endif
        private static void Intercept(Copydatastruct ptr, Packet packet, int id)
        {
            if (packet.Data.Length > 0 && packet.Data.Length == ptr.CbData)
            {
                var c = Collections.AttachedClients[id];
                if (c.ServerPacketHandler == null)
                    return;
                if (c.ClientPacketHandler == null)
                    return;
                if (packet.Type == 2 &&
                    c.ClientPacketHandler[packet.Data[0]] != null)
                    c.ClientPacketHandler[packet.Data[0]](id, packet);
                else if (packet.Type == 1 &&
                    c.ServerPacketHandler[packet.Data[0]] != null)
                    c.ServerPacketHandler[packet.Data[0]](id, packet);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Copydatastruct
        {
            public readonly uint DwData;
            public readonly int CbData;
            public readonly IntPtr LpData;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            CleanupMemory();
        }

        private static void CleanupMemory()
        {
            foreach (Client client in Collections.AttachedClients.Values)
            {
                client.CleanUpMememory();
                client.DestroyResources();
            }

            new Thread(delegate ()
            {
                Thread.Sleep(1000);
                Process.GetCurrentProcess().Kill();
            }).Start();
        }
    }
}
