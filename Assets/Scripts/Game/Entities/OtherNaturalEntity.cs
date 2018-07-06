using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game.Entities
{
    public abstract class OtherNaturalEntity : Entity
    {
        protected GameObject go;

        protected OtherNaturalEntity(int playerId, Vector2Int position, Map map, GameManager gameManager) : base(playerId, position, map, gameManager)
        {
        }
    }
}
