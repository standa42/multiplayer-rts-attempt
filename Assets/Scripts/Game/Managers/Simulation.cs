using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using NetworkObjects.Commands;
using UnityEngine;

namespace Assets.Scripts.Game.Managers
{
    public class Simulation
    {
        // in seconds
        private readonly double commandsUpdateTimestep = 0.2f; 
        private readonly double simulationUpdateTimestep = 0.05f;

        public bool IsRunning { get; set; } = false;

        private readonly int howManyTimesSimulateBeforeControlUpdate;

        private double commandsUpdateWatch = 0;
        private double simulationUpdateWatch = 0;
        private int simulationInControlCounter = 0;

        private int commandsRoundCounter = 0; 

        // TODO not initialized
        private Game game;
        private InputCommandAutomata inputAutomata;
        private CommandsHolder commandsHolder;

        public Simulation(CommandsHolder commandsHolder, InputCommandAutomata inputAutomata, Game game)
        {
            howManyTimesSimulateBeforeControlUpdate = (int)Math.Floor(commandsUpdateTimestep / simulationUpdateTimestep);
            this.commandsHolder = commandsHolder;
            this.inputAutomata = inputAutomata;
            this.game = game;
        }

        public void Run()
        {
            Log.LogMessage("Simulation set to running");
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public void Update()
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

                List<Command> commands;

                if (
                    commandsUpdateWatch > commandsUpdateTimestep &&
                    simulationInControlCounter == howManyTimesSimulateBeforeControlUpdate &&
                    commandsHolder.TryGetCommandsForRound(commandsRoundCounter, out commands)
                    )
                {
                    //Log.LogMessage($"Command round: {commandsRoundCounter}");
                    SetCommands(commands);

                    commandsUpdateWatch -= commandsUpdateTimestep;
                    simulationInControlCounter = 0;
                    commandsRoundCounter++;
                }
            }
        }

        private void Simulate()
        {
            //Log.LogMessage("Simulation tick");
            game.Simulate();
        }

        private void SetCommands(List<Command> commands)
        {
            //Log.LogMessage("Commands tick");
            inputAutomata.SendMyCommandsToPlayers();
            game.ApplyCommands(commands);
        }
    }
}
