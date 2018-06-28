using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace NetworkObjects
{
    [ProtoContract]
    public class RandomSeedPacket : Packet
    {
        [ProtoMember(1)]
        public int RandomSeed;

        public RandomSeedPacket()
        {

        }
    }
}
