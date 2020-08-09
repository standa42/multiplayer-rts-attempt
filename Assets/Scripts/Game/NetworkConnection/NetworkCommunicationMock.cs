using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkObjects;
using NetworkObjects.Commands;

namespace Assets.Scripts.Game.NetworkConnection
{
    /// <summary>
    /// Mock of network communication for testing in editor
    /// </summary>
    public class NetworkCommunicationMock : NetworkCommunication
    {
        public new event ProtoByteDelegate ReceivedMessage;
        public delegate void ProtoByteDelegate(int id, byte[] data);
        

        public NetworkCommunicationMock( uint opponents) : base(opponents)
        {
            this.Receiver = new ProtoBuffCommandReceiver(this);
        }

        public override void SendCommands(List<Command> commands)
        {

        }


        public override void SendMessages(List<Packet> packetList)
        {

        }
    }
}
