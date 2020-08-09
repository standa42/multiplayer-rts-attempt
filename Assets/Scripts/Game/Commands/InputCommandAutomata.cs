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
    /// <summary>
    /// Creates user commands based on events from touch input and the state of game map
    /// </summary>
    public class InputCommandAutomata
    {
        public PlayerProxies.PlayerProxies proxies;
        private Map map;
        private TouchInput touchInput;
        private int playerId;
        private GameObject cancelButtonGO;

        /// <summary>
        /// Used for computation of realworld coordinates (in plane of the map) of hitting the display
        /// </summary>
        private Plane groundPlane;

        private GameObject tapParticleSystem;

        /// <summary>
        /// Plane created by drag event to mark selected area
        /// </summary>
        private GameObject dragPlane;

        private List<Command> commands = new List<Command>();
        private List<PlayerWorker> selectedUnits;

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

            tapParticleSystem = GameObject.Instantiate(Resources.Load("Map/RuntimeObjects/TapParticleSystem") as GameObject);
            var m = tapParticleSystem.GetComponent<ParticleSystem>().main;
            m.startColor = PlayerMaterials.GetPlayerMaterialByPlayerId(playerId).color;
        }

        /// <summary>
        /// Callback for cancel button click
        /// </summary>
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

        /// <summary>
        /// Procedure sending commands collected in current command round by automata
        /// </summary>
        public virtual void SendMyCommandsToPlayers()
        {
            proxies.AddCommandsToLocalPlayer(commands);
            commands = new List<Command>();
        }

        /// <summary>
        /// Callback for tap event in touch input
        /// </summary>
        /// <param name="position">touch position in display coordinates</param>
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

                tapParticleSystem.transform.position = new Vector3(position.x, position.y, -0.1f);
                tapParticleSystem.GetComponent<ParticleSystem>().Play();
            }

            
        }

        /// <summary>
        /// Returns if position is valid position im map
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool PosValidInMap(Vector2Int position)
        {
            if (position.x < 0 || position.x >= map.Size.X || position.y < 0 || position.y >= map.Size.Y)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Makes entities in selected list selected by calling select on them
        /// </summary>
        private void SelectSelectedUnits()
        {
            foreach (var selectedUnit in selectedUnits)
            {
                selectedUnit.Select();
            }
        }

        /// <summary>
        /// Unselects entities in selected list
        /// </summary>
        private void UnselectSelectedUnits()
        {
            foreach (var selectedUnit in selectedUnits)
            {
                selectedUnit.Unselect();
            }

            selectedUnits = null;
        }

        /// <summary>
        /// Callback for drag event from touch input
        /// </summary>
        /// <param name="begin">position on diplay where drag began</param>
        /// <param name="current">curretn position of touch on display</param>
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

        /// <summary>
        /// Callback for dragEnd event
        /// </summary>
        /// <param name="begin">position on diplay where drag started</param>
        /// <param name="end">position on diplay where drag ended</param>
        public void DragEndFromInput(Vector2 begin, Vector2 end)
        {
            if (dragPlane != null)
            {
                UnityEngine.Object.Destroy(dragPlane);
                dragPlane = null;

               DragSelect(begin,end);
            }
        }

        /// <summary>
        /// Selection of units in drag rectangle
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        public void DragSelect(Vector2 begin, Vector2 end)
        {
            // definition of selection rectangle coordinates

            begin = RaycastFromScreenToGround(begin);
            end = RaycastFromScreenToGround(end);

            // solve, which coordinates are higher/lower
            float lowFloatX = begin.x > end.x ? end.x : begin.x;
            float lowFloatY = begin.y > end.y ? end.y : begin.y;
            float highFloatX = begin.x > end.x ? begin.x : end.x;
            float highFloatY = begin.y > end.y ? begin.y : end.y;

            // rounds them with some tolerance to fields that are not covered completely (fields with cover > 0.5 are selected too)
            int lowX = Mathf.CeilToInt(lowFloatX - 0.5f);
            int lowY = Mathf.CeilToInt(lowFloatY -0.5f);
            int highX = Mathf.FloorToInt(highFloatX + 0.5f);
            int highY = Mathf.FloorToInt(highFloatY + 0.5f);

            // validation of coordinates according to the size of the map
            lowX = FitIntoInterval(lowX, 0, map.Size.X - 1);
            lowY = FitIntoInterval(lowY, 0, map.Size.Y - 1);
            highX = FitIntoInterval(highX, 0, map.Size.X - 1);
            highY = FitIntoInterval(highY, 0, map.Size.Y - 1);

            // if rectangle is zero in at least one dimension 
            if (Math.Abs(lowX - highX) == 0 || Math.Abs(lowY - highY) == 0)
            {
                return;
            }

            // if there were selected units by previous gesture (another drag for example)
            if (selectedUnits != null)
            {
                UnselectSelectedUnits();
            }

            // checks if there are any player entities in rectangle and loads the tom selected units list
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

            if (selectedUnits != null)
            {
                SelectSelectedUnits();
            }
        }

        /// <summary>
        /// fits value into the bounds of given interval min <-> max
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Counts real world coordinates from diplay to the map ground plane
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <returns></returns>
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

    /// <summary>
    /// InputAutomata substitution used in editor -> creates multiplayer messages and sends them to proxies (fake network)
    /// </summary>
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
