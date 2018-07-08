using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Game.Controls;
using UnityEngine;

namespace Assets.Scripts.Game.Entities
{
    public class PlayerWorker : Entity
    {
        protected GameObject go;
        protected GameObject selection;

        protected bool moving = false;
        protected Vector2Int destination;
        protected int WaitToStopMoving = 17;
        protected int WaitToStopMovingCounter;
        protected int WaitToMove = 3;
        protected int WaitToMoveCounter;

        public PlayerWorker(int playerId, Vector2Int position, Map map, GameManager gameManager) : base(playerId, position, map, gameManager)
        {
            selection = GameObject.Instantiate(Resources.Load(@"Map/Entities/SelectedCube") as GameObject);
            selection.transform.position = new Vector3(position.x + 0.5f, position.y + 0.5f, -0.37f);

            selection.SetActive(false);
        }

        public virtual void Move()
        {
            if (moving)
            {
                if (Position == destination)
                {
                    moving = false;
                    return;
                }

                if (WaitToMoveCounter < WaitToMove)
                {
                    WaitToMoveCounter++;
                }
                else
                {
                    var moveDirection = Pathfinder.FindPath(Map, Position, destination);
                    switch (moveDirection)
                    {
                        case MoveDirection.Left:
                            Map.Entities[Position.x][Position.y] = null;
                            Map.Entities[Position.x-1][Position.y] = this;
                            Position = new Vector2Int(Position.x - 1, Position.y);
                            MoveGOToCurrentLocation();
                            break;
                        case MoveDirection.Right:
                            Map.Entities[Position.x][Position.y] = null;
                            Map.Entities[Position.x+1][Position.y] = this;
                            Position = new Vector2Int(Position.x + 1, Position.y);
                            MoveGOToCurrentLocation();
                            break;
                        case MoveDirection.Up:
                            Map.Entities[Position.x][Position.y] = null;
                            Map.Entities[Position.x][Position.y+1] = this;
                            Position = new Vector2Int(Position.x, Position.y + 1);
                            MoveGOToCurrentLocation();
                            break;
                        case MoveDirection.Down:
                            Map.Entities[Position.x][Position.y] = null;
                            Map.Entities[Position.x][Position.y-1] = this;
                            Position = new Vector2Int(Position.x, Position.y - 1);
                            MoveGOToCurrentLocation();
                            break;
                        case MoveDirection.None:
                            WaitToStopMovingCounter++;
                            if (WaitToStopMovingCounter >= WaitToStopMoving)
                            {
                                moving = false;
                            }
                            break;
                    }
                }
            }
        }

        public virtual void MoveCmd(Vector2Int position)
        {
            destination = position;
            WaitToMoveCounter = 0;
            WaitToStopMovingCounter = 0;
            moving = true;
        }

        public virtual void MoveGOToCurrentLocation()
        {
            WaitToStopMovingCounter = 0;
            WaitToMoveCounter = 0;

            go.transform.position = new Vector3(Position.x + 0.5f, Position.y + 0.5f, go.transform.position.z);
            selection.transform.position = new Vector3(Position.x + 0.5f, Position.y + 0.5f, -0.37f);
        }

        public virtual void Select()
        {
            selection.SetActive(true);
        }

        public virtual void Unselect()
        {
            selection.SetActive(false);
        }
    }
}
