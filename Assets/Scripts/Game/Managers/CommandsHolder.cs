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

        public void AddCommandToPlayer(int playerId, List<Command> commands)
        {
            PlayerCommands[playerId].Add(commands);
        }

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
