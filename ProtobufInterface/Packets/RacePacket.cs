using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace NetworkObjects
{
    [ProtoContract]
    public class RacePacket : Packet
    {
        [ProtoMember(1)]
        public byte RaceId;

        public RacePacket()
        {

        }
    }
}
