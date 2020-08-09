using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Holds player variables - resources, entitiesCount, etc..
    /// </summary>
    public class PlayerStats
    {
        public int PlayerId { get; }
        public int Resources { get; set; }
        public int Wood { get; set; }

        public PlayerStats(int playerId, int resources, int wood)
        {
            PlayerId = playerId;
            Resources = resources;
            Wood = wood;
        }
    }
}
