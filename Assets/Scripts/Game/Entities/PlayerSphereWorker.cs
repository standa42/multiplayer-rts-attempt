using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game.Entities
{
    public class PlayerSphereWorker : PlayerWorker
    {
        public PlayerSphereWorker(int playerId, Vector2Int position, Map map, GameManager gameManager) : base(playerId, position, map, gameManager)
        {
            go = GameObject.Instantiate(Resources.Load(@"Map/Entities/SphereWorker") as GameObject);
            go.transform.position = new Vector3(position.x + 0.5f, position.y + 0.5f, go.transform.position.z);

            Renderer rend = go.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material = PlayerMaterials.GetPlayerMaterialByPlayerId(playerId);
            }
        }

    }
}
