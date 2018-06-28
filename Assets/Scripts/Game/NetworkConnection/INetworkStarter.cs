using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Game.Initialization;
using NetworkObjects;

namespace Assets.Scripts.Game.NetworkConnection
{
    interface INetworkStarter
    {
        event Initializer.NetworkStartGameDelegate NetworkStartGame;
        void Run(int opponentCount);
    }

    public class AutoritativeNetworkStarter : INetworkStarter
    {
        public event Initializer.NetworkStartGameDelegate NetworkStartGame;
        private NetworkCommunication networkCommunication;
        private int arrivedReadyMessagesCount = 0;
        private int opponentCount;

        public AutoritativeNetworkStarter(NetworkCommunication networkCommunication)
        {
            this.networkCommunication = networkCommunication;
            networkCommunication.Receiver.ReadyCommandReceived += ReadyMessageArrived;
        }

        public void Run(int opponentCount)
        {
            this.opponentCount = opponentCount;

            TryToStartTheGame();
        }

        private void ReadyMessageArrived()
        {
            arrivedReadyMessagesCount++;

            TryToStartTheGame();
        }

        private void TryToStartTheGame()
        {
            if (arrivedReadyMessagesCount == opponentCount)
            {
                networkCommunication.SendMessages(new List<Packet>() {new StartPacket()});
                NetworkStartGame?.Invoke();
            }
        }
    }

    public class NonAutoritativeNetworkStarter : INetworkStarter
    {
        public event Initializer.NetworkStartGameDelegate NetworkStartGame;
        private NetworkCommunication networkCommunication;

        public NonAutoritativeNetworkStarter(NetworkCommunication networkCommunication)
        {
            this.networkCommunication = networkCommunication;
            networkCommunication.Receiver.StartCommandReceived += StartMessageArrived;
        }

        public void Run(int opponentCount)
        {
            networkCommunication.SendMessages(new List<Packet>{new ReadyPacket()});
        }

        private void StartMessageArrived()
        {
            NetworkStartGame?.Invoke();
        }

    }
}
