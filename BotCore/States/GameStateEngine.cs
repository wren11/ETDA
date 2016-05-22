using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BotCore.States
{
    public class GameStateEngine
    {
        private GameClient Client { get; set; }
        public GameStateEngine(GameClient client)
        {
            Client = client;
            States = new List<GameState>();
            States.Sort();
        }

        public List<GameState> States { get; private set; }


        public virtual void Pulse(TimeSpan Elapsed)
        {
            //if we are paused, do nothing.
            if (Client.Paused)
            {
                return;
            }

            //pulse all components
            foreach (var m in Client.InstalledComponents)
                m.Pulse();


            List<GameState> copy;
            lock (States)
            {
                copy = new List<GameState>(States);
                copy.Sort();
            }

            var duplicates = copy.GroupBy(s => s.Priority).SelectMany(grp => grp.Skip(1));

            foreach (GameState state in duplicates)
            {
                if (state.Enabled && state.NeedToRun && Client.ClientReady && Client.IsInGame())
                {
                    Client.RunningState = state;
                    state.timer.Start();
                    state.Run(Elapsed);
                    state.InTransition = false;
                    state.timer.Stop();
                    break;
                }
            }

            foreach (GameState state in copy.Except(duplicates))
            {
                if (state.Enabled && state.NeedToRun && Client.ClientReady && Client.IsInGame())
                {
                    Client.RunningState = state;
                    state.timer.Start();
                    state.Run(Elapsed);
                    state.InTransition = false;
                    state.timer.Stop();
                    break;
                }
            }
        }
    }
}
