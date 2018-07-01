using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkObjects.Commands;

namespace Assets.Scripts.Game
{
    public class InputCommandAutomata
    {
        public PlayerProxies.PlayerProxies proxies;

        public InputCommandAutomata(PlayerProxies.PlayerProxies proxies)
        {
            this.proxies = proxies;
        }

        public virtual void SendMyCommandsToPlayers()
        {
            proxies.AddCommandsToLocalPlayer(new List<Command>(){new Command()});
        }
    }

    public class InputCommandAutomataMock : InputCommandAutomata
    {
        public InputCommandAutomataMock(PlayerProxies.PlayerProxies proxies) : base(proxies)
        {
        }

        public override void SendMyCommandsToPlayers()
        {
            base.SendMyCommandsToPlayers();
            proxies.AddCommmandsFromMultiplayerPlayer(1, new List<Command>(){});
        }
    }
}
