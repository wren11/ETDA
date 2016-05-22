using System;
using Binarysharp.MemoryManagement;

namespace BotCore
{
    [Serializable]
    public abstract class UpdateableComponent : IDisposable
    {
        public GameClient Client
        {
            get; set;
        }

        public UpdateTimer Timer
        {
            get; set;
        }

        public MemorySharp _memory
        {
            get { return Client.Memory; }
        }

        public abstract void Update(TimeSpan tick);

        public virtual void Pulse()
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Client = null;
                Timer = null;

                Console.WriteLine("Destroying Component {0}", GetType().Name);
            }
        }

        public bool IsInGame()
        {
            try
            {
                if (_memory == null || !_memory.IsRunning)
                    return false;
                var x = _memory.Read<int>((IntPtr)0x00882E68, false);
                if (x > 0)
                    x = _memory.Read<int>((IntPtr)x + 0x23C, false);
                if (x > 0)
                    return true;
            }
            catch
            {
                return false;
            }

            return false;
        }
    }
}
