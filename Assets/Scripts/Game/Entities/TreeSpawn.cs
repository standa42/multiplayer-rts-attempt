using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts.Game.Entities
{
    public class TreeSpawn : Entity
    {
        public TreeSpawn(int playerId, Vector2Int position, Map map, GameManager gameManager) : base(playerId, position, map, gameManager)
        {
        }

        /// <summary>
        /// With certain probability - creates tree on given location
        /// </summary>
        public void GrowTree()
        {
            if (Map.Entities[Position.x][Position.y] == null)
            {
                if (CommonRandom.RandomGenerator.Next(0,2000) == 0)
                {
                    var tree = new Tree(-1, new Vector2Int(Position.x, Position.y), Map, GameManager);
                    Map.AddEntityToPosition(Position.x, Position.y, tree);
                    GameManager.NaturalEntities.Add(tree);
                }
            }
        }
    }
}
