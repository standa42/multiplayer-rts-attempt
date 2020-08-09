using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Game.Entities;
using NetworkObjects;
using NetworkObjects.Commands;

namespace Assets.Scripts.Game
{
    /// <summary>
    /// Forward simulation and commands steps to gameManager
    /// </summary>
    public class Game
    {
        private GameManager Manager { get; }

        public Game(GameManager manager)
        {
            Manager = manager;
        }

        public void Simulate()
        {
            Manager.Simulate();
        }

        public void ApplyCommands(List<Command> commands)
        {
            Manager.ApplyCommands(commands);
        }
    }


}
