using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotCore.Components
{
    public class StressTest : UpdateableComponent
    {
        public StressTest()
        {
            Timer = new UpdateTimer(TimeSpan.FromMilliseconds(1.0));        
        }

        public override void Pulse()
        {
            if (IsInGame() && Enabled)
            {
                var points = Client.FieldMap.GetNearByTiles(Client.Attributes.ServerPosition.X, Client.Attributes.ServerPosition.Y, 8);
                Random n = new Random();
                foreach (var pt in points)
                    Actions.GameActions.PlayAnimation(Client, (short)(n.Next() % 256), pt);
            }
            else
            {
                if (Client.InjectToClientQueue.Count > 100)
                    Client.CleanUpMememory();
            }
        }

        public override void Update(TimeSpan tick)
        {
            Timer.Update(tick);

            if (Timer.Elapsed)
            {
                Pulse();
                Timer.Reset();
            }
        }
    }
}
