using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using Assets.Scripts.Game.Initialization;
using Assets.Scripts.Menu;
using Boo.Lang;
using NetworkObjects;

namespace Assets.Scripts.Game.NetworkConnection
{
    interface INetworkCreator
    {
        event Initializer.NetworkCreationVariablesDelegate CreationVariablesAvailable;
        void Run(int myId);
    }

    public class AutoritativeNetworkCreator : INetworkCreator
    {
        public event Initializer.NetworkCreationVariablesDelegate CreationVariablesAvailable;

        private NetworkCommunication networkCommunication;

        private Random rnd = new Random();
        private byte mapId;
        private int randomSeed;
        private System.Collections.Generic.List<Tuple<int, byte>> listPlayerRace = new System.Collections.Generic.List<Tuple<int, byte>>();

        public AutoritativeNetworkCreator(NetworkCommunication networkCommunication)
        {
            this.networkCommunication = networkCommunication;
            networkCommunication.Receiver.RaceReceived += IncomingRace;


        }

        public void Run(int myId)
        {
            Log.LogMessage("Autoritative running");

            listPlayerRace.Add(new Tuple<int, byte>(myId, (byte)IntersceneData.MenuChoicesInstance.RaceEnum));

            mapId = (byte)rnd.Next(0, 1);
            randomSeed = rnd.Next(1, 100000);

            Log.LogMessage("Sending message");


            try
            {
                networkCommunication.SendMessages(new System.Collections.Generic.List<Packet>
            {
                new MapPacket { MapId = mapId},
                new RandomSeedPacket { RandomSeed = randomSeed},
                new RacePacket { RaceId = (byte)IntersceneData.MenuChoicesInstance.RaceEnum}
            });
            }
            catch (Exception e)
            {
                Log.LogMessage(e.Message);
            }

            Log.LogMessage("Autoritative message send succed");

            TryToCreateTheGame();
        }

        private void IncomingRace(byte raceId, int playerId)
        {
            listPlayerRace.Add(new Tuple<int, byte>(playerId, raceId));

            TryToCreateTheGame();
        }

        private void TryToCreateTheGame()
        {
            if (listPlayerRace.Count == (int)IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame)
            {
                CreationVariablesAvailable(listPlayerRace, mapId, randomSeed);
            }
        }
    }

    public class NonAutoritativeNetworkCreator : INetworkCreator
    {
        public event Initializer.NetworkCreationVariablesDelegate CreationVariablesAvailable;

        private NetworkCommunication networkCommunication;

        private byte? mapId = null;
        private int? randomSeed = null;
        private System.Collections.Generic.List<Tuple<int, byte>> listPlayerRace = new System.Collections.Generic.List<Tuple<int, byte>>();

        public NonAutoritativeNetworkCreator(NetworkCommunication networkCommunication)
        {
            this.networkCommunication = networkCommunication;
            networkCommunication.Receiver.RaceReceived += IncomingRace;
            networkCommunication.Receiver.RandomSeedReceived += IncomingRandomSeed;
            networkCommunication.Receiver.MapReceived += IncomingMap;
        }

        private void IncomingRace(byte raceId, int playerId)
        {
            listPlayerRace.Add(new Tuple<int, byte>(playerId, raceId));

            TryToCreateTheGame();
        }

        private void IncomingMap(byte mapId)
        {
            this.mapId = mapId;

            TryToCreateTheGame();
        }

        private void IncomingRandomSeed(int seed)
        {
            this.randomSeed = seed;

            TryToCreateTheGame();
        }

        public void Run(int myId)
        {
            Log.LogMessage("Non-Autoritative running");
            listPlayerRace.Add(new Tuple<int, byte>(myId, (byte)IntersceneData.MenuChoicesInstance.RaceEnum));

            Log.LogMessage("Sending message");

            try
            {
                networkCommunication.SendMessages(new System.Collections.Generic.List<Packet>
                {
                    new RacePacket { RaceId = (byte)IntersceneData.MenuChoicesInstance.RaceEnum}
                });
            }
            catch (Exception e)
            {
                Log.LogMessage(e.Message);
            }


            Log.LogMessage("NonAutoritative message send succed");

            TryToCreateTheGame();
        }

        private void TryToCreateTheGame()
        {
            if (
                listPlayerRace.Count == (int)IntersceneData.MenuChoicesInstance.NumberOfPlayersInGame &&
                mapId != null &&
                randomSeed != null
                )
            {
                CreationVariablesAvailable(listPlayerRace, mapId.Value, randomSeed.Value);
            }
        }
    }

}
