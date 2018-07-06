using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.UI
{
    public class ResourceDisplay : MonoBehaviour
    {
        public Text WoodAmount;
        public Text ResourceAmount;

        public void ActualAmount(int resourceAmount, int woodAmount)
        {
            WoodAmount.text = woodAmount.ToString();
            ResourceAmount.text = resourceAmount.ToString();
        }
    }
}
