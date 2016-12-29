using System;
using System.Collections.Generic;
using System.Linq;

namespace BotCore.States
{
    public class GameStateEngine : UpdateableComponent
    {
        GameClient _client { get; set; }
        public List<GameState> States { get; private set; }

        public GameStateEngine(GameClient client)
        {
            Timer = new UpdateTimer(TimeSpan.FromMilliseconds(1));
            _client = client;
            States = new List<GameState>();
            States.Sort();
        }

        public void UpdateStates(TimeSpan Elapsed)
        {
            //if we are paused, do nothing.
            if (_client.Paused)
            {
                return;
            }

            List<GameState> copy;
            lock (States)
            {
                copy = new List<GameState>(States);
                copy.Sort();
            }

            var duplicates = copy
                .GroupBy(s => s.Priority)
                .SelectMany(grp => grp.Skip(1));
            foreach (GameState state in duplicates)
            {
                if (state.Enabled && state.NeedToRun && _client.ClientReady && _client.IsInGame())
                {
                    _client.RunningState = state;
                    state.timer.Start();
                    state.Run(Elapsed);
                    state.InTransition = false;
                    state.timer.Stop();
                    break;
                }
            }

            foreach (GameState state in copy.Except(duplicates))
            {
                if (state.Enabled && state.NeedToRun && _client.ClientReady && _client.IsInGame())
                {
                    _client.RunningState = state;
                    state.timer.Start();
                    state.Run(Elapsed);
                    state.InTransition = false;
                    state.timer.Stop();
                    break;
                }
            }
        }

        public override void Update(TimeSpan tick)
        {
            Timer.Update(tick);

            if (Timer.Elapsed)
            {
                UpdateStates(tick);

                Pulse();
                Timer.Reset();
            }
        }
    }
}
