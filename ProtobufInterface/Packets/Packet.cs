using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkObjects.Commands;
using ProtoBuf;

namespace NetworkObjects
{
    [ProtoContract]
    [ProtoInclude(150, typeof(StartPacket))]
    [ProtoInclude(151, typeof(RandomSeedPacket))]
    [ProtoInclude(152, typeof(ReadyPacket))]
    [ProtoInclude(153, typeof(RacePacket))]
    [ProtoInclude(154, typeof(MapPacket))]
    [ProtoInclude(155, typeof(CommandsPacket))]
    public class Packet
    {
    }
}
