using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts.Game.Managers
{
    public class Simulation
    {
        // in seconds
        private readonly double commandsUpdateTimestep = 0.2f; 
        private readonly double simulationUpdateTimestep = 0.01f;

        public bool IsRunning { get; set; } = false;

        private readonly int howManyTimesSimulateBeforeControlUpdate;

        private double commandsUpdateWatch = 0;
        private double simulationUpdateWatch = 0;
        private int simulationInControlCounter = 0;

        // TODO not initialized
        private Game game;
        private GameCommandsHolder commandsHolder;
        private InputCommandAutomata inputAutomata;

        public Simulation()
        {
            howManyTimesSimulateBeforeControlUpdate = (int)Math.Floor(commandsUpdateTimestep / simulationUpdateTimestep);
        }

        public void Run()
        {
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        void Update()
        {
            if (IsRunning)
            {
                commandsUpdateWatch += Time.deltaTime;
                simulationUpdateWatch += Time.deltaTime;

                if (
                    simulationUpdateWatch > simulationUpdateTimestep &&
                    simulationInControlCounter < howManyTimesSimulateBeforeControlUpdate
                    )
                {
                    Simulate();

                    simulationUpdateWatch -= simulationUpdateTimestep;
                    simulationInControlCounter++;
                }

                if (
                    commandsUpdateWatch > commandsUpdateTimestep &&
                    simulationInControlCounter == howManyTimesSimulateBeforeControlUpdate &&
                    commandsHolder.AreCommandsForNexRoundAvailable()
                    )
                {
                    SetCommands();

                    commandsUpdateWatch -= commandsUpdateTimestep;
                    simulationInControlCounter = 0;
                }
            }
        }

        private void Simulate()
        {
            Log.LogMessage("Simulation tick");
            game.Simulate();
        }

        private void SetCommands()
        {
            Log.LogMessage("Commands tick");
            inputAutomata.SendMyCommandsToOtherPlayers();
            game.ApplyCommands();
        }
    }
}
