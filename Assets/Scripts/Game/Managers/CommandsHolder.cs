using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using Assets.Scripts.Menu;
using NetworkObjects.Commands;

namespace Assets.Scripts.Game.Managers
{
    /// <summary>
    /// Holds commands for all players and all command rounds
    /// </summary>
    public class CommandsHolder
    {
        public List<List<Command>>[] PlayerCommands;

        public CommandsHolder(NumberOfPlayersInGame numberOfPlayers)
        {
            PlayerCommands = new List<List<Command>>[(int)numberOfPlayers];

            for (int i = 0; i < PlayerCommands.Length; i++)
            {
                PlayerCommands[i] = new List<List<Command>>();

                for (int j = 0; j < SimulationConfig.HowManyRoundsInFutureShouldCommandsBeExecuted; j++)
                {
                   PlayerCommands[i].Add(new List<Command>()); 
                }
            }
        }

        /// <summary>
        /// Add commands to next command round - relies on the fact, that communication over network is in order
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="commands"></param>
        public void AddCommandToPlayer(int playerId, List<Command> commands)
        {
            PlayerCommands[playerId].Add(commands);
        }

        /// <summary>
        /// Gives commands for given command round
        /// returns false if commands are not available -> also commands are null
        /// </summary>
        /// <param name="commandRound"></param>
        /// <param name="commands"></param>
        /// <returns></returns>
        public bool TryGetCommandsForRound(int commandRound, out List<Command> commands)
        {
            commands = null;

            bool everybodysCommandsArrived = true;

            // if somebody doesnt have commands for this round yet - skip this function and wait for next call(frame?)
            foreach (var player in PlayerCommands)
            {
                if (player.Count <= commandRound)
                {
                    everybodysCommandsArrived = false;
                    return false;
                }
            }

            if (everybodysCommandsArrived)
            {
                commands = new List<Command>();

                foreach (var player in PlayerCommands)
                {
                    try
                    {
                        if (player[commandRound].Count > 0)
                        {
                            commands.AddRange(player[commandRound]);
                        }
                    }
                    catch (Exception e)
                    {
                        //Log.LogMessage($"Exception {e.Message}#{commandRound}#{PlayerCommands[0][commandRound]}");
                        //Log.LogMessage($"{commandRound}#{PlayerCommands[0][commandRound]}");
                        //Log.LogMessage($"{PlayerCommands[1][commandRound]}");
                        throw;
                    }
                    
                }
            }
            
            return everybodysCommandsArrived;
        }
    }
}
