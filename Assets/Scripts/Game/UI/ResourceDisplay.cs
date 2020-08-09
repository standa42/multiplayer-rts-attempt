using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.UI
{
    /// <summary>
    /// Script supposed to be attached to GO's displaying current resources amount of player
    /// </summary>
    public class ResourceDisplay : MonoBehaviour
    {
        public Text WoodAmount;
        public Text ResourceAmount;

        /// <summary>
        /// Function binded to resource amount change
        /// </summary>
        /// <param name="resourceAmount"></param>
        /// <param name="woodAmount"></param>
        public void ActualAmount(int resourceAmount, int woodAmount)
        {
            WoodAmount.text = woodAmount.ToString();
            ResourceAmount.text = resourceAmount.ToString();
        }
    }
}
