using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkObjects.Commands;

namespace Assets.Scripts.Game.PlayerProxies
{
    public abstract class PlayerProxy
    {
        public int Id;

        public abstract void ProcessIncomingCommands(List<Command> commands);
    }
}
