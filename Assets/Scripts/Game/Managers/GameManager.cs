using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Game.Entities;
using Assets.Scripts.Game.Managers;
using Assets.Scripts.Menu;
using NetworkObjects.Commands;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class GameManager
    {
        public ResourcesManager ResourcesManager { get; }
        public Map map { get; set; }
        private readonly int playerId;

        public List<OtherNaturalEntity> NaturalEntities;
        public List<TreeSpawn> TreeSpawns;
        public List<PlayerWorker> PlayerWorkers;

        public GameManager(int playerId, NumberOfPlayersInGame numberOfPlayers)
        {
            this.playerId = playerId;

            ResourcesManager = new ResourcesManager(playerId, numberOfPlayers);
            
            NaturalEntities = new List<OtherNaturalEntity>();
            TreeSpawns = new List<TreeSpawn>();
            PlayerWorkers = new List<PlayerWorker>();
        }

        public void Simulate()
        {
            foreach (var treeSpawn in TreeSpawns)
            {
                treeSpawn.GrowTree();
            }

            foreach (var playerWorker in PlayerWorkers)
            {
                playerWorker.Move();
            }
        }

        public void ApplyCommands(List<Command> commands)
        {
            foreach (var command in commands)
            {
                var moveCommand = (MoveCommand) command;

                PlayerWorkers.Where(x => x.EntityId == moveCommand.EntityId).First().MoveCmd(new Vector2(moveCommand.X,moveCommand.Y));
            }
        }

        
    }
}
