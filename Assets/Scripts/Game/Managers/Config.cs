using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game
{
    /// <summary>
    /// Holds important constants for application
    /// </summary>
    public static class Config
    {
        public static int InitialResourceAmount { get; } = 100;
        public static int InitialWoodAmount { get; } = 100;
    }

    /// <summary>
    /// Races that can be choosed by players
    /// </summary>
    public enum RaceEnum : byte
    {
        Cubes,
        Spheres
    }
}
