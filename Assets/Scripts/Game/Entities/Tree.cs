using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game.Entities
{
    public class Tree : OtherNaturalEntity
    {
        public Tree(int playerId, Vector2Int position, Map map, GameManager gameManager) : base(playerId, position, map, gameManager)
        {
            go = GameObject.Instantiate( Resources.Load(@"Map/Entities/TreeEntity") as GameObject);
            go.transform.position = new Vector3(position.x + 0.5f, position.y + 0.5f, go.transform.position.z);
        }
    }
}
