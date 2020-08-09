using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Game.NetworkConnection;

namespace Assets.Scripts.Game.Initialization
{
    /// <summary>
    /// Creates match and returns device player network game id
    /// </summary>
    public class MatchCreator
    {
        private NetworkCommunication networkCommunication;

        public event RoomReadyDelegate RoomReady;

        public delegate void RoomReadyDelegate(int myId);

        public MatchCreator(NetworkCommunication networkCommunication)
        {
            this.networkCommunication = networkCommunication;
            networkCommunication.RoomConnected += RoomInCommunicationIsReady;
        }

        public void CreateMatch()
        {
            networkCommunication.CreateQuickMatch();
        }

        private void RoomInCommunicationIsReady(int myId)
        {
            networkCommunication.RoomConnected -= RoomInCommunicationIsReady;
            RoomReady(myId);
        }
    }
}
