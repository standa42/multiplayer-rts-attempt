using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Common
{
    /// <summary>
    /// Should be used as random generator in game procedures, so the game stays deterministic.
    /// is generated with seed shared at the beggining of the game
    /// </summary>
    public static class CommonRandom
    {
        public static Random RandomGenerator { get; set; }
    }
}
