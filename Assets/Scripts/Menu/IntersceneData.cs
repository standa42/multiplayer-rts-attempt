using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Game;
using UnityEngine;

namespace Assets.Scripts.Menu
{
    public static class IntersceneData
    {
        // Menu choice "singleton"
        public static MenuChoices MenuChoicesInstance { get; set; }
    }


    /// <summary>
    /// Wrapper ckass for information that should be passed to the game scene
    /// </summary>
    public class MenuChoices
    {
        public NumberOfPlayersInGame NumberOfPlayersInGame { get; }
        public RaceEnum RaceEnum { get; }

        public MenuChoices(NumberOfPlayersInGame numberNumberOfPlayersInGame, RaceEnum raceEnum)
        {
            NumberOfPlayersInGame = numberNumberOfPlayersInGame;
            RaceEnum = raceEnum;
        }
    }

    public enum NumberOfPlayersInGame : int
    {
        Two = 2,
        Four = 4
    }
    
}
