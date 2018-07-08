using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Common;
using Assets.Scripts.Game.Entities;
using NetworkObjects.Commands;
using UnityEngine;
using UnityEngine.UI;
using PlayerWorker = Assets.Scripts.Game.Entities.PlayerWorker;

namespace Assets.Scripts.Game
{
    public class InputCommandAutomata
    {
        public PlayerProxies.PlayerProxies proxies;
        private Map map;
        private TouchInput touchInput;
        private int playerId;

        private Plane groundPlane;

        private GameObject dragPlane;

        private List<Command> commands = new List<Command>();
        private List<PlayerWorker> selectedUnits;
        private GameObject cancelButtonGO;

        public InputCommandAutomata(int playerId, PlayerProxies.PlayerProxies proxies,Map map, TouchInput touchInput, GameManager gameManager)
        {
            this.playerId = playerId;
            this.map = map;
            this.touchInput = touchInput;
            this.proxies = proxies;

            touchInput.DragEvent += DragFromInput;
            touchInput.DragEndEvent += DragEndFromInput;
            touchInput.TapEvent += TapFromInput;

            groundPlane.Set3Points(new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 1, 0));

            cancelButtonGO = GameObject.Instantiate(Resources.Load(@"UI/CancelButton") as GameObject);
            cancelButtonGO.transform.SetParent(GameObject.Find(@"CanvasFullHdDimensions").transform,false);
            cancelButtonGO.SetActive(false);
            cancelButtonGO.GetComponent<Button>().onClick.AddListener(CancelClicked);
        }

        public void CancelClicked()
        {
            cancelButtonGO.SetActive(false);

            if (selectedUnits != null)
            {
                foreach (var selectedUnit in selectedUnits)
                {
                    selectedUnit.Unselect();
                }
            }

            selectedUnits = null;
        }

        public virtual void SendMyCommandsToPlayers()
        {
            proxies.AddCommandsToLocalPlayer(commands);
            commands = new List<Command>();
        }

        public void TapFromInput(Vector2 position)
        {
            position = RaycastFromScreenToGround(position);
            var pos = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));

            if (!PosValidInMap(pos))
            {
                return;
            }

            // first selection
            if (selectedUnits == null)
            {
                if (map.Entities[pos.x][pos.y] != null)
                {
                    if (map.Entities[pos.x][pos.y].PlayerId == playerId)
                    {
                        selectedUnits = new List<PlayerWorker>() { (PlayerWorker)map.Entities[pos.x][pos.y] };
                        SelectSelectedUnits();
                        cancelButtonGO.SetActive(true);
                    }
                }
            }
            // second (move) field selection
            else
            {
                foreach (var selectedUnit in selectedUnits)
                {
                    var movecmd = new MoveCommand()
                    {
                        EntityId = selectedUnit.EntityId,
                        X = pos.x,
                        Y = pos.y
                    };

                    commands.Add(movecmd);
                }   
                UnselectSelectedUnits();
                cancelButtonGO.SetActive(false);
            }

            
        }

        private bool PosValidInMap(Vector2Int position)
        {
            if (position.x < 0 || position.x >= map.Size.X || position.y < 0 || position.y >= map.Size.Y)
            {
                return false;
            }

            return true;
        }

        private void SelectSelectedUnits()
        {
            foreach (var selectedUnit in selectedUnits)
            {
                selectedUnit.Select();
            }
        }

        private void UnselectSelectedUnits()
        {
            foreach (var selectedUnit in selectedUnits)
            {
                selectedUnit.Unselect();
            }

            selectedUnits = null;
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

               DragSelect(begin,end);
            }
        }

        public void DragSelect(Vector2 begin, Vector2 end)
        {
            begin = RaycastFromScreenToGround(begin);
            end = RaycastFromScreenToGround(end);

            float lowFloatX = begin.x > end.x ? end.x : begin.x;
            float lowFloatY = begin.y > end.y ? end.y : begin.y;
            float highFloatX = begin.x > end.x ? begin.x : end.x;
            float highFloatY = begin.y > end.y ? begin.y : end.y;

            int lowX = Mathf.CeilToInt(lowFloatX - 0.5f);
            int lowY = Mathf.CeilToInt(lowFloatY -0.5f);
            int highX = Mathf.FloorToInt(highFloatX + 0.5f);
            int highY = Mathf.FloorToInt(highFloatY + 0.5f);

            lowX = FitIntoInterval(lowX, 0, map.Size.X - 1);
            lowY = FitIntoInterval(lowY, 0, map.Size.Y - 1);
            highX = FitIntoInterval(highX, 0, map.Size.X - 1);
            highY = FitIntoInterval(highY, 0, map.Size.Y - 1);

            if (Math.Abs(lowX - highX) == 0 || Math.Abs(lowY - highY) == 0)
            {
                return;
            }

            if (selectedUnits != null)
            {
                UnselectSelectedUnits();
            }


            for (int x = lowX; x <= highX; x++)
            {
                for (int y = lowY; y <= highY; y++)
                {
                    if (map.Entities[x][y] != null && map.Entities[x][y].PlayerId == playerId)
                    {
                        if (selectedUnits == null)
                        {
                            selectedUnits = new List<PlayerWorker>();
                            cancelButtonGO.SetActive(true);
                        }
                        selectedUnits.Add((PlayerWorker)map.Entities[x][y]);
                    }
                }
            }

            SelectSelectedUnits();
        }

        private int FitIntoInterval(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }

        private Vector3 RaycastFromScreenToGround(Vector3 screenPosition)
        {
            // Raycast to find coordinates in map
            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
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
        public InputCommandAutomataMock(int playerId,PlayerProxies.PlayerProxies proxies, Map map, TouchInput touchInput, GameManager gameManager) : base(playerId,proxies, map, touchInput, gameManager)
        {
        }

        public override void SendMyCommandsToPlayers()
        {
            base.SendMyCommandsToPlayers();
            proxies.AddCommmandsFromMultiplayerPlayer(1, new List<Command>() { });
        }
    }
}
