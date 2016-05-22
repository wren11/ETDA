using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotCore.Shared.Helpers
{
    public sealed class CasterHelper
    {
        private static HashSet<int> g_YetCalled = new HashSet<int>();
        private static readonly object g_SyncRoot = new object();

        public static void Reset()
        {
            g_YetCalled.Clear();
        }

        public static bool EnsureOnce(Type type, Action a, params object[] arguments)
        {
            int hash = 17;
            hash = hash * 41 + type.GetHashCode();
            hash = hash * 41 + a.GetHashCode();
            for (int i = 0; i < arguments.Length; i++)
            {
                hash = hash * 41 + (arguments[i] ?? 0).GetHashCode();
            }

            if (!g_YetCalled.Contains(hash))
            {
                lock (g_SyncRoot)
                {
                    if (!g_YetCalled.Contains(hash))
                    {
                        a();
                        g_YetCalled.Add(hash);
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public sealed class CallHelper
    {
        private static HashSet<int> g_YetCalled = new HashSet<int>();
        private static readonly object g_SyncRoot = new object();

        public static int Casting
        {
            get { return g_YetCalled.Count; }
        }

        public static void Reset()
        {
            g_YetCalled.Clear();
        }

        public static bool EnsureOnce(Type type, Action a, params object[] arguments)
        {
            int hash = 17;
            hash = hash * 41 + type.GetHashCode();
            hash = hash * 41 + a.GetHashCode();
            for (int i = 0; i < arguments.Length; i++)
            {
                hash = hash * 41 + (arguments[i] ?? 0).GetHashCode();
            }

            if (!g_YetCalled.Contains(hash))
            {
                lock (g_SyncRoot)
                {
                    if (!g_YetCalled.Contains(hash))
                    {
                        a();
                        g_YetCalled.Add(hash);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
