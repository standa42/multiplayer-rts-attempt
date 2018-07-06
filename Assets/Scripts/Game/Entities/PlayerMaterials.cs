using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game.Entities
{
    public static class PlayerMaterials
    {
        public static Material GetPlayerMaterialByPlayerId(int playerId)
        {
            switch (playerId)
            {
                case 0:
                    return Resources.Load(@"PlayerColors/BluePlayer") as Material;
                case 1:
                    return Resources.Load(@"PlayerColors/OrangePlayer") as Material;
                case 2:
                    return Resources.Load(@"PlayerColors/TurqoisePlayer") as Material;
                case 3:
                    return Resources.Load(@"PlayerColors/YellowPlayer") as Material;
                default:
                    throw new ArgumentException("Players id was too high in getting player material");
            }
        }
    }
}
