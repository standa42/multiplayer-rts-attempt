using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Game
{
    public static class Config
    {
        public static int InitialResourceAmount { get; } = 100;
        public static int InitialWoodAmount { get; } = 100;
    }

    public enum RaceEnum : byte
    {
        Universal,
        Second
    }
}
