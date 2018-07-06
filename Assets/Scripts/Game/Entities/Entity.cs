using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts.Game.Entities
{
    public abstract class Entity
    {
        public int EntityId { get; }
        public int PlayerId { get; }
        public Vector2Int Position { get; set; }

        protected Map Map;
        protected GameManager GameManager;

        public Entity(int playerId, Vector2Int position, Map map, GameManager gameManager)
        {
            this.EntityId = EntitySequence.GetNewEntityId();

            this.PlayerId = playerId;
            Position = position;
            this.Map = map;
            this.GameManager = gameManager;
        }
    }
}
