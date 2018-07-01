using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using Assets.Scripts.Game.Entities;
using Assets.Scripts.Game.Races;
using NetworkingWrapper;
using NetworkObjects;
using NetworkObjects.Commands;

namespace Assets.Scripts.Game.NetworkConnection
{
    public class ProtoBuffCommandReceiver
    {
        public event RaceReceivedDelegate RaceReceived;
        public event MapReceivedDelegate MapReceived;
        public event RandomSeedReceivedDelegate RandomSeedReceived;
        public event StartCommandReceivedDelegate StartCommandReceived;
        public event ReadyCommandReceivedDelegate ReadyCommandReceived;
        public event IncomingCommandsDelegate CommandsReceived;

        public delegate void RaceReceivedDelegate(byte raceId, int playerId);
        public delegate void MapReceivedDelegate(byte mapId);
        public delegate void RandomSeedReceivedDelegate(int seed);
        public delegate void StartCommandReceivedDelegate();
        public delegate void ReadyCommandReceivedDelegate();
        public delegate void IncomingCommandsDelegate(int playerId, List<Command> commands);



        public ProtoBuffCommandReceiver(NetworkCommunication networkCommunication)
        {
            networkCommunication.ReceivedMessage += ProcessReceivedMessage;
        }

        private void ProcessReceivedMessage(int playerId, byte[] data)
        {
            var listOfPackets = (List<Packet>)SerializationObjectWrapper.Deserialize(data).Value;

            foreach (var packet in listOfPackets.AsEnumerable().Reverse())
            {
                Log.LogMessage("Packet received");
                Log.LogMessage($"packet is typable to commandsPacket: {packet is CommandsPacket}");
                dynamic dPacket = packet;
                InvokeIncomingPacket(playerId, dPacket);
            }
        }

        private void InvokeIncomingPacket(int playerId, StartPacket sp)
        {
            Log.LogMessage("StartPacket");
            StartCommandReceived();
        }

        private void InvokeIncomingPacket(int playerId, ReadyPacket rp)
        {
            Log.LogMessage("ReadyPacket");
            ReadyCommandReceived();
        }

        private void InvokeIncomingPacket(int playerId, RandomSeedPacket rsp)
        {
            Log.LogMessage("RandomSeedPacket");
            RandomSeedReceived(rsp.RandomSeed);
        }

        private void InvokeIncomingPacket(int playerId, MapPacket mp)
        {
            Log.LogMessage("MapPacket");
            MapReceived(mp.MapId);
        }

        private void InvokeIncomingPacket(int playerId, RacePacket rp)
        {
            Log.LogMessage("RacePacket");
            RaceReceived(rp.RaceId, playerId);
        }

        private void InvokeIncomingPacket(int playerId, CommandsPacket cp)
        {
            Log.LogMessage("CommandsPacket");
            CommandsReceived(playerId, cp.Commands);
        }
    }
}
