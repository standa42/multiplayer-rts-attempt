﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Game.NetworkConnection;
using NetworkObjects.Commands;
using NetworkObjects;

namespace Assets.Scripts.Game.PlayerProxies
{
    /// <summary>
    /// Handles forwarding of commands form local(device) player
    /// </summary>
    public class LocalPlayerProxy : PlayerProxy
    {
        private NetworkCommunication networkCommunication;
        private CommandsHolder commandsHolder;

        public LocalPlayerProxy(int id, NetworkCommunication networkCommunication, CommandsHolder commandsHolder)
        {
            this.commandsHolder = commandsHolder;
            this.networkCommunication = networkCommunication;
            this.Id = id;
        }

        public override void ProcessIncomingCommands(List<Command> commands)
        {
            commandsHolder.AddCommandToPlayer(Id,commands);
            networkCommunication.SendCommands(commands);
        }
    }
}
