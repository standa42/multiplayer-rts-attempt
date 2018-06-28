using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace NetworkObjects
{
    [ProtoContract]
    public class MapPacket : Packet
    {
        [ProtoMember(1)]
        public byte MapId;

        public MapPacket()
        {
            
        }
    }
}
