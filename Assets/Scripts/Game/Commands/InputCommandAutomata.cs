using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using NetworkObjects.Commands;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class InputCommandAutomata
    {
        public PlayerProxies.PlayerProxies proxies;
        private Map map;
        private TouchInput touchInput;

        private Plane groundPlane;

        private GameObject dragPlane;

        public InputCommandAutomata(PlayerProxies.PlayerProxies proxies,Map map, TouchInput touchInput)
        {
            this.map = map;
            this.touchInput = touchInput;
            this.proxies = proxies;

            touchInput.DragEvent += DragFromInput;
            touchInput.DragEndEvent += DragEndFromInput;

            groundPlane.Set3Points(new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 1, 0));
        }

        public virtual void SendMyCommandsToPlayers()
        {
            proxies.AddCommandsToLocalPlayer(new List<Command>() { new Command() });
        }

        public void DragFromInput(Vector2 begin, Vector2 current)
        {
            if (dragPlane == null)
            {
                dragPlane = GameObject.Instantiate(Resources.Load(@"Map/RuntimeObjects/DragSelectionPlane")) as GameObject;
            }

            var beginToWorldCoor = RaycastFromScreenToGround(begin);
            var currentToWorldCoor = RaycastFromScreenToGround(current);
            
            dragPlane.transform.position = new Vector3((beginToWorldCoor.x + currentToWorldCoor.x) / 2, (beginToWorldCoor.y + currentToWorldCoor.y) / 2, -0.7f);
            dragPlane.transform.localScale = new Vector3(Mathf.Abs(beginToWorldCoor.x - currentToWorldCoor.x) / 10, 1, Mathf.Abs(beginToWorldCoor.y - currentToWorldCoor.y) / 10);

        }

        public void DragEndFromInput(Vector2 begin, Vector2 end)
        {
            if (dragPlane != null)
            {
                UnityEngine.Object.Destroy(dragPlane);
                dragPlane = null;
            }
        }

        private Vector3 RaycastFromScreenToGround(Vector3 screePosition)
        {
            // Raycast to find coordinates in map
            Ray ray = Camera.main.ScreenPointToRay(screePosition);
            float rayDistance;
            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 markerPoint;
                markerPoint = ray.GetPoint(rayDistance);
                return markerPoint;
            }
            return Vector3.zero;
        }
    }

    public class InputCommandAutomataMock : InputCommandAutomata
    {
        public InputCommandAutomataMock(PlayerProxies.PlayerProxies proxies, Map map, TouchInput touchInput) : base(proxies, map, touchInput)
        {
        }

        public override void SendMyCommandsToPlayers()
        {
            base.SendMyCommandsToPlayers();
            proxies.AddCommmandsFromMultiplayerPlayer(1, new List<Command>() { });
        }
    }
}
