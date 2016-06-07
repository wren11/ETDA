using Binarysharp.MemoryManagement;
using BotCore.Components;
using BotCore.States;
using BotCore.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BotCore
{
    [Serializable]
    public abstract class GameClient : UpdateableComponent
    {
        #region GameClient Properties
        internal bool ShouldUpdate;

        public EventHandler<Packet> OnPacketRecevied = delegate { };
        public EventHandler<Packet> OnPacketSent = delegate { };

        public int SendPointer { get; set; }
        public int RecvPointer { get; set; }

        public Form BotForm { get; set; }

        public Map FieldMap { get; private set; }

        public MemorySharp Memory { get; set; }

        public GameUtilities Utilities { get; set; }

        public MessageStateMachine MessageMachine = new MessageStateMachine();
    
        public GameStateEngine StateMachine { get; set; }

        public TargetFinder ObjectSearcher
        {
            get
            {
                var obj = InstalledComponents.OfType<TargetFinder>()
                    .FirstOrDefault();

                if (obj != null)
                    return obj;

                throw new Exception("Error, Component TargetFinder is not installed.");
            }

        }

        public Magic GameMagic
        {
            get
            {
                var obj = InstalledComponents.OfType<Magic>()
                    .FirstOrDefault();

                if (obj != null)
                    return obj;

                throw new Exception("Error, Component Magic is not installed.");
            }
        }


        public GameEquipment ActiveEquipment
        {
            get
            {
                var obj = InstalledComponents.OfType<GameEquipment>()
                    .FirstOrDefault();

                if (obj != null)
                    return obj;

                throw new Exception("Error, Component GameEquipment is not installed.");
            }
        }


        public Inventory GameInventory
        {
            get
            {
                var obj = InstalledComponents.OfType<Inventory>()
                    .FirstOrDefault();

                if (obj != null)
                    return obj;

                throw new Exception("Error, Component Inventory is not installed.");
            }

        }

        public PlayerAttributes Attributes
        {
            get
            {
                var obj = InstalledComponents.OfType<PlayerAttributes>()
                    .FirstOrDefault();

                if (obj != null)
                    return obj;

                throw new Exception("Error, Component PlayerArrtibutes is not installed.");
            }

        }

        public Activebar Active
        {
            get
            {
                var obj = InstalledComponents.OfType<Activebar>()
                    .FirstOrDefault();

                if (obj != null)
                    return obj;

                throw new Exception("Error, Component ActiveBar is not installed.");
            }
        }

        public List<short> SpellBar { get; set; }

        public bool ClientReady = true;

        public bool MapLoaded = false;

        public bool MapLoading { get; set; }

        public string EquippedWeapon { get; set; }

        public int Steps { get; set; }

        public List<GameClient> OtherClients
        {
            get
            {
                List<GameClient> copy;
                lock (Collections.AttachedClients)
                {
                    copy = new List<GameClient>(Collections.AttachedClients.Values);
                }
                return copy.FindAll(i => i.Attributes.Serial != this.Attributes.Serial);
            }
        }
        #endregion

        #region Internal GameClient Properties
        public bool IsCurrentlyCasting { get; internal set; }
        public bool IsCursed { get; internal set; }
        public bool ShouldRemoveDebuffs { get; internal set; }
        public bool Paused { get; internal set; }
        public Spell LastCastedSpell { get; internal set; }
        public MapObject LastCastTarget { get; internal set; }
        public GameState RunningState { get; internal set; }
        public DateTime LastUseInvetorySlot { get; internal set; }
        public DateTime LastEquipmentUpdate { get; internal set; }
        public byte EquippedWeaponId { get; set; }
        public DateTime WhenLastCasted { get; internal set; }
        #endregion

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void ProgressCallback(int value);

        [DllImport("EtDA.dll")]
        public static extern void OnAction([MarshalAs(UnmanagedType.FunctionPtr)] ProgressCallback callbackPointer);

        internal static string Hack { get; set; }
        public int WalkOrdinal { get; internal set; }

        public void InitializeMemory(Process p, string Hack)
        {
            Memory = new MemorySharp(p);
            CleanUpMememory();

            var injected = Memory.Read<byte>((IntPtr)0x00567FB0, false);
            if (injected == 85)
            {
                var HackModule = Memory.Modules.Inject(Hack);
                if (HackModule.IsValid)
                {
                    Console.Beep();
                }

                GameClient.Hack = Hack;
            }      
        }

        public virtual void OnAttached()
        {
            Task.Run(() => ProcessOutQueue(this));
            Task.Run(() => ProcessInQueue(this));

            //setup the utilities
            Utilities = new GameUtilities(this);
        }

        #region Packet Hooks
        public EventHandler<Packet>[] ClientPacketHandler = new EventHandler<Packet>[256];
        public EventHandler<Packet>[] ServerPacketHandler = new EventHandler<Packet>[256];
        internal ConcurrentQueue<byte[]> InjectToServerQueue = new ConcurrentQueue<byte[]>();
        internal ConcurrentQueue<byte[]> InjectToClientQueue = new ConcurrentQueue<byte[]>();
        internal static int _Total;

        public static void InjectPacket<T>(GameClient client, Packet packet) where T : Packet
        {
            if (typeof(T) == typeof(ClientPacket))
                client.InjectToClientQueue.Enqueue(packet.Data);
            else if (typeof(T) == typeof(ServerPacket))
                client.InjectToServerQueue.Enqueue(packet.Data);
        }
        #endregion

        #region Packet Consumers
        
        internal static void ProcessInQueue(GameClient client)
        {
            while (true)
            {
                Thread.Sleep(1);
                
                if (client == null)
                    continue;
                if (client.Memory == null)
                    continue;
                if (!client.Memory.IsRunning || !client.IsInGame())
                    continue;
                
                byte[] activeBuffer;
                while (client.InjectToClientQueue.TryDequeue(out activeBuffer))
                {
                    Interlocked.Add(ref _Total, 1);
                    while (client.Memory.Read<byte>((IntPtr)0x00721000, false) == 1)
                    {
                        if (!client.Memory.IsRunning)
                            break;
                        Thread.Sleep(1);
                    }
                    try
                    {
                        client.Memory.Write((IntPtr)0x00721000, 1, false);
                        client.Memory.Write((IntPtr)0x00721004, 0, false);
                        client.Memory.Write((IntPtr)0x00721008, activeBuffer.Length, false);
                        client.Memory.Write((IntPtr)0x00721012, activeBuffer, false);
                    }
                    catch
                    {
                        client.CleanUpMememory();
                        return;
                    }
                }
            }
        }
        
        internal static void ProcessOutQueue(GameClient client)
        {
            while (true)
            {
                Thread.Sleep(1);
                
                if (client == null)
                    continue;
                if (client.Memory == null)
                    continue;
                if (!client.Memory.IsRunning || !client.IsInGame())
                    continue;
                
                byte[] activeBuffer;
                while (client.InjectToServerQueue.TryDequeue(out activeBuffer))
                {
                    Interlocked.Add(ref _Total, 1);
                    
                    while (client.Memory.Read<byte>((IntPtr)0x006FD000, false) == 1)
                    {
                        if (!client.Memory.IsRunning)
                            break;
                        Thread.Sleep(1);
                    }
                    client.Memory.Write((IntPtr)0x006FD000, 1, false);
                    client.Memory.Write((IntPtr)0x006FD004, 1, false);
                    client.Memory.Write((IntPtr)0x006FD008, activeBuffer.Length, false);
                    client.Memory.Write((IntPtr)0x006FD012, activeBuffer, false);
                }
            }
        }

        #endregion

        internal void AddClientHandler(byte action, EventHandler<Packet> data)
        {
            ClientPacketHandler[action] = data;
        }

        internal void AddServerHandler(byte action, EventHandler<Packet> data)
        {
            ServerPacketHandler[action] = data;
        }

        public abstract class RepeatableTimer : UpdateableComponent { }

        public enum MovementState : byte
        {
            [Description("Movement is Locked, and you cannot sent walk packets.")]
            Locked = 0x74,
            [Description("Movement is Free, and you can send walk packets.")]
            Free = 0x75
        }

        public void ApplyMovementLock()
        {
            if (!_memory.IsRunning || !IsInGame())
                return;
            var state = (MovementState)_memory.Read<byte>((IntPtr)0x005F0ADE, false);
            if (state == MovementState.Free)
                _memory.Write<byte>((IntPtr)0x005F0ADE, (byte)MovementState.Locked, false);
        }

        public void ReleaseMovementLock()
        {
            if (!_memory.IsRunning || !IsInGame())
                return;
            var state = (MovementState)_memory.Read<byte>((IntPtr)0x005F0ADE, false);
            if (state == MovementState.Locked)
                _memory.Write<byte>((IntPtr)0x005F0ADE, (byte)MovementState.Free, false);
        }

        public abstract void TransitionTo(GameState current, TimeSpan Elapsed);
                
        public GameClient()
        {
            Timer = new UpdateTimer(TimeSpan.FromMilliseconds(50));            
            PrepareComponents();

            ShouldUpdate = true;
        }

        public void LoadStates(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath))
                return;
            if (!File.Exists(assemblyPath))
                return;

            //ensure utils are initialized.
            if (Utilities == null)
                Utilities = new GameUtilities(this);

            try
            {
                Assembly asm = Assembly.LoadFrom(assemblyPath);
                Type[] types = asm.GetTypes();
                
                foreach (Type type in types)
                {
                    if (type.IsClass && type.IsSubclassOf(typeof(GameState)))
                    {
                        GameState tempState = (GameState)Activator.CreateInstance(type);
                        tempState.Client = this;
                        tempState.SettingsInterface = new StateSettings(tempState) { Dock = DockStyle.Fill };
                        tempState.SettingsInterface.OnSettingsUpdated += SettingsInterface_OnSettingsUpdated;
                        tempState.InitState();
                        
                        if (!StateMachine.States.Contains(tempState))
                            StateMachine.States.Add(tempState);
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        
        void SettingsInterface_OnSettingsUpdated(GameState state)
        {
            if (!Client.BotForm.IsDisposed)
                Client.BotForm.Invalidate();

            //refresh client anytime settings are changed.
            if (Client.IsInGame())
                Actions.GameActions.Refresh(Client);
        }
        
        public void CleanUpMememory()
        {
            if (_memory != null && _memory.IsRunning)
            {
                _memory.Write((IntPtr)0x006FD000, 0, false);
                _memory.Write((IntPtr)0x00721000, 0, false);
            }
            
            InjectToClientQueue = new ConcurrentQueue<byte[]>();
            InjectToServerQueue = new ConcurrentQueue<byte[]>();
            
            GC.Collect();
        }
        
        public void DestroyResources()
        {
            ShouldUpdate = false;

            foreach (var component in InstalledComponents)
                component.Dispose();

            InstalledComponents.Clear();
            
            if (BotForm != null)
            {
                BotForm = null;
            }
            
            ServerPacketHandler = null;
            ClientPacketHandler = null;

            GC.Collect();
        }

        public Collection<UpdateableComponent> InstalledComponents
            = new Collection<UpdateableComponent>();
        
        private void PrepareComponents()
        {
            //core components, endabled by default
            InstalledComponents.Add(new Inventory() { Client = this, Enabled = true });
            InstalledComponents.Add(new PlayerAttributes() { Client = this, Enabled = true });
            InstalledComponents.Add(new Magic() { Client = this, Enabled = true });
            InstalledComponents.Add(new GameEquipment() { Client = this, Enabled = true });
            InstalledComponents.Add(new Activebar() { Client = this, Enabled = true });
            InstalledComponents.Add(new TargetFinder() { Client = this, Enabled = true });

            //disabled by default components
            InstalledComponents.Add(new StressTest() { Client = this, Enabled = false });

            //mandatory components
            FieldMap = new Map();
            FieldMap.Enabled = true;
            FieldMap.Client = this;
            FieldMap.Init(0, 0, 0);
            InstalledComponents.Add(FieldMap);

            //init state machine.
            StateMachine = new GameStateEngine(this);
            LoadStates("BotCore.dll");

            callback = (value) =>
            {
            };
        }

        private ProgressCallback callback = null;

        public override void Update(TimeSpan tick)
        {
          
            Timer.Update(tick);
            
            if (Timer.Elapsed)
            {
                try
                {
                    UpdateComponents(tick);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error Updating Components. \n\r" + e.StackTrace + "\n\n" + e.Message);
                }
                finally
                {
                    Timer.Reset();
                }
            }
        }
        
        private void UpdateComponents(TimeSpan tick)
        {
            if (Client.ShouldUpdate)
            {
                var copy = default(List<UpdateableComponent>);
                lock (InstalledComponents)
                {
                    copy = new List<UpdateableComponent>(InstalledComponents);
                }

                foreach (UpdateableComponent component in copy)
                    if (component.Enabled)
                        component.Update(tick);

                var objs = ObjectSearcher.VisibleObjects.ToArray();
                foreach (var obj in objs)
                    obj.Update(tick);

                //pulse state machine
                StateMachine.Pulse(tick);
            }
        }
    }
}