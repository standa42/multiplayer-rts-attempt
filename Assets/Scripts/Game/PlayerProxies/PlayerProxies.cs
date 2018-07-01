using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Game.NetworkConnection;
using Assets.Scripts.Menu;
using NetworkObjects.Commands;

namespace Assets.Scripts.Game.PlayerProxies
{
    public class PlayerProxies
    {
        private LocalPlayerProxy localPlayer;
        private int localPlayerId;

        private NetworkCommunication networkCommunication;
        private CommandsHolder commandsHolder;
        private List<PlayerProxy> allProxies = new List<PlayerProxy>();

        public PlayerProxies(NetworkCommunication networkCommunication, CommandsHolder commandsHolder, int localPlayerId, NumberOfPlayersInGame numberOfPlayers)
        {
            this.commandsHolder = commandsHolder;
            this.networkCommunication = networkCommunication;
            this.localPlayerId = localPlayerId;

            localPlayer = new LocalPlayerProxy(localPlayerId, networkCommunication,commandsHolder);

            var proxies = new List<PlayerProxy>();
            proxies.Add(localPlayer);

            for (int i = 0; i < (int)numberOfPlayers; i++)
            {
                if (i != localPlayerId)
                {
                    proxies.Add( new MultiplayerPlayerProxy(i,networkCommunication,commandsHolder) );
                }
            }

            allProxies = proxies.OrderBy(x => x.Id).ToList();

            networkCommunication.Receiver.CommandsReceived += AddCommmandsFromMultiplayerPlayer;
        }

        public void AddCommandsToLocalPlayer(List<Command> commands)
        {
            localPlayer.ProcessIncomingCommands(commands);
        }

        public void AddCommmandsFromMultiplayerPlayer(int playerId, List<Command> commands)
        {
            //Log.LogMessage($"Commands from network came: null {commands == null} {playerId} {commands}");
            //allProxies[playerId].ProcessIncomingCommands(commands);
            allProxies[playerId].ProcessIncomingCommands(commands ?? new List<Command>());
        }
    }
}
