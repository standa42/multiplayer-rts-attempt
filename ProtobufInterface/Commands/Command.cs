using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace NetworkObjects.Commands
{
    [ProtoContract]
    [ProtoInclude(170, typeof(BuildCommand))]
    [ProtoInclude(171, typeof(AttackCommand))]
    [ProtoInclude(172, typeof(MineCommand))]
    [ProtoInclude(173, typeof(MoveCommand))]
    [ProtoInclude(174, typeof(CreateUnitCommand))]
    public class Command
    {
        public Command()
        {

        }
    }

    [ProtoContract]
    public class BuildCommand : Command
    {
        [ProtoMember(1)]
        public int BuilderId;
        [ProtoMember(2)]
        public int BuildingId;
        [ProtoMember(3)]
        public int X;
        [ProtoMember(4)]
        public int Y;

        public BuildCommand()
        {

        }
    }

    [ProtoContract]
    public class AttackCommand : Command
    {
        [ProtoMember(1)]
        public int Attacker;
        [ProtoMember(2)]
        public int Attacked;

        public AttackCommand()
        {

        }
    }

    [ProtoContract]
    public class MineCommand : Command
    {
        [ProtoMember(1)]
        public int MiningEntityId;
        [ProtoMember(2)]
        public int MinedEntityId;
        [ProtoMember(3)]
        public int X;
        [ProtoMember(4)]
        public int Y;

        public MineCommand()
        {

        }
    }

    [ProtoContract]
    public class MoveCommand : Command
    {
        [ProtoMember(1)]
        public int EntityId;
        [ProtoMember(2)]
        public int X;
        [ProtoMember(3)]
        public int Y;

        public MoveCommand()
        {

        }
    }

    [ProtoContract]
    public class CreateUnitCommand : Command
    {
        [ProtoMember(1)]
        public int BuildingId;
        [ProtoMember(2)]
        public int UnitId;

        public CreateUnitCommand()
        {

        }
    }
}
