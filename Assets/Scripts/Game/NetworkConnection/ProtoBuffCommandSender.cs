using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkingWrapper;
using NetworkObjects;

namespace Assets.Scripts.Game.NetworkConnection
{
    /// <summary>
    /// Wraps packets to byte[]
    /// </summary>
    public class ProtoBuffCommandSender
    {
        public byte[] WrapCommandMessages(List<Packet> packetList)
        {
            var wrappedObject = new SerializationObjectWrapper<List<Packet>>{TypedValue = packetList};
            var byteArray = wrappedObject.Serialize();
            return byteArray;
        }
    }
}
