using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkObjects;
using NetworkObjects.Commands;
using ProtoBuf;

namespace NetworkObjects
{
    [ProtoContract]
    public class CommandsPacket : Packet
    {
        [ProtoMember(1)]
        public List<Command> Commands;

        public CommandsPacket()
        {

        }
    }
}
