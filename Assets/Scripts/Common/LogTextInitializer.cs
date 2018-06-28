using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Common
{
    public class LogTextInitializer : MonoBehaviour
    {
        void Awake()
        {
            Log.LogTextInstance = this.gameObject.GetComponent(typeof(Text)) as Text;
        }
    }
}
